using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;

namespace Registrator.Module.BusinessObjects
{
    /// <summary>
    /// Последовательность целых чисел
    /// </summary>
    public class Sequence : BaseObject
    {
        private static object locker = new object();

        private string code;
        private Resets reset;
        private int current;
        private DateTime lastChange;

        /// <summary>Условия сброса значения</summary>
        public enum Resets
        {
            /// <summary>Никогда</summary>
            Never = 0,
            /// <summary>В начале года</summary>
            Year = 1
        }

        public Sequence() { }
        public Sequence(Session session) : base(session) { }

        /// <summary>
        /// Код последовательности
        /// </summary>
        [Size(200)]
        [RuleRequiredField("Registrator.Module.BusinessObjects.Sequence.CodeRequired", DefaultContexts.Save)]
        public string Code
        {
            get { return code; }
            set { SetPropertyValue("Code", ref code, value); }
        }

        /// <summary>
        /// Условие сброса значения последовательности
        /// </summary>
        [RuleRequiredField("Registrator.Module.BusinessObjects.Sequence.ResetRequired", DefaultContexts.Save)]
        public Resets Reset
        {
            get { return reset; }
            set { SetPropertyValue("Reset", ref reset, value); }
        }

        /// <summary>
        /// Текущее значение последовательности
        /// </summary>
        /// <value>Последнее значение, полученное из этой последовательности. Если последовательность новая 
        /// или значение должно быть сброшено, то текущее значение устанавливается в 0.</value>
        [RuleRequiredField("Registrator.Module.BusinessObjects.Sequence.CurrentRequired", DefaultContexts.Save)]
        public int Current
        {
            get { return current; }
            set { SetPropertyValue("Current", ref current, value); }
        }

        /// <summary>
        /// Дата последнего изменения последовательности (получения очередного значения)
        /// </summary>
        [RuleRequiredField("Registrator.Module.BusinessObjects.Sequence.LastChangeRequired", DefaultContexts.Save)]
        public DateTime LastChange
        {
            get { return lastChange; }
            set { SetPropertyValue("LastChange", ref lastChange, value); }
        }

        /// <summary>
        /// Возвращает следующее значение последовательности
        /// </summary>
        /// <returns>Следующее значение последовательности</returns>
        public int GetNext()
        {
            int result = Current;

            // Условие сброса значения
            switch (Reset)
            {
                case Resets.Year: if (LastChange.Year < DateTime.Now.Year) result = 0; break;
            }

            // Следующее значение
            result++;
            Current = result;
            LastChange = DateTime.Now;

            return result;
        }

        // Возвращает последовательность из коллекции объектов
        // [DX:14.1.8] InTransaction не работает из-за поиска DevExpress в родительской сессии, в которой не было загрузки объектов
        private static Sequence Find(System.Collections.ICollection coll, string code)
        {
            if (coll == null || string.IsNullOrEmpty(code)) return null;
            foreach (object obj in coll)
                if (obj is Sequence)
                {
                    Sequence sequence = (Sequence)obj;
                    if (sequence.Code == code)
                        return sequence;
                }
            return null;
        }

        /// <summary>
        /// Возвращает последовательность с указанным кодом
        /// </summary>
        /// <param name="session">Сессия для загрузки и хранения объектов</param>
        /// <param name="code">Код последовательности</param>
        /// <returns>Последовательность с кодом <b>code</b> или null, если последовательность не найдена</returns>
        public static Sequence Find(Session session, string code)
        {
            if (session == null || string.IsNullOrEmpty(code)) return null;
            CriteriaOperator criteria = Fields.Code == code;
            return Find(session.GetObjectsToSave(false), code) ?? session.FindObject<Sequence>(criteria);
        }

        /// <summary>
        /// Возвращает последовательность с указанным кодом
        /// </summary>
        /// <param name="objectSpace">Пространство объектов для загрузки и хранения объектов</param>
        /// <param name="code">Код последовательности</param>
        /// <returns>Последовательность с кодом <b>code</b> или null, если последовательность не найдена</returns>
        public static Sequence Find(IObjectSpace objectSpace, string code)
        {
            if (objectSpace == null || string.IsNullOrEmpty(code)) return null;
            CriteriaOperator criteria = Fields.Code == code;
            return objectSpace.FindObject<Sequence>(criteria);
        }

        /// <summary>
        /// Возвращает текущее значение последовательности с указанным кодом
        /// </summary>
        /// <param name="session">Сессия для загрузки и хранения объектов</param>
        /// <param name="code">Код последовательности</param>
        /// <returns>Текущее значение последовательности с кодом <b>code</b> или 0, если последовательность не найдена</returns>
        public static int GetCurrent(Session session, string code)
        {
            Sequence sequence = Find(session, code);
            return sequence != null ? sequence.Current : 0;
        }

        /// <summary>
        /// Возвращает текущее значение последовательности с указанным кодом
        /// </summary>
        /// <param name="objectSpace">Пространство объектов для загрузки и хранения объектов</param>
        /// <param name="code">Код последовательности</param>
        /// <returns>Текущее значение последовательности с кодом <b>code</b> или 0, если последовательность не найдена</returns>
        public static int GetCurrent(IObjectSpace objectSpace, string code)
        {
            Sequence sequence = Find(objectSpace, code);
            return sequence != null ? sequence.Current : 0;
        }

        /// <summary>
        /// Возвращает следующее значение последовательности с указанным кодом
        /// </summary>
        /// <param name="session">Сессия для загрузки и хранения объектов</param>
        /// <param name="code">Код последовательности</param>
        /// <param name="reset">Условие сброса значения последовательности</param>
        /// <returns>Следующее значение последовательности с кодом <b>code</b></returns>
        /// <remarks>Если значение последовательность не найдена, то создается новая последовательность. Если выполнено условие сброса 
        /// значения последовательности, то значение сбрасывается в 0 и возвращается следующее значение, т.е. 1.</remarks>
        /// <exception cref="ArgumentNullException">Не указаны сессия или код последовательности</exception>
        /// <exception cref="ArgumentException">Указано условие сброса значения, отличное от исходного</exception>
        public static int GetNext(Session session, string code, Resets reset = Resets.Never)
        {
            if (session == null) throw new ArgumentNullException("session");
            if (string.IsNullOrEmpty(code)) throw new ArgumentNullException("code");
            Sequence sequence = Find(session, code);
            if (sequence == null)
            {
                sequence = new Sequence(session);
                sequence.Code = code;
                sequence.Reset = reset;
                sequence.Current = 0;
                sequence.LastChange = DateTime.Now;
            }
            else if (sequence.Reset != reset)
                throw new ArgumentException(string.Format("Reset of sequence {0} is different (parameter = {1}, original = {2})", code, reset, sequence.Reset), "reset");
            return sequence.GetNext();
        }

        /// <summary>
        /// Возвращает следующее значение последовательности с указанным кодом
        /// </summary>
        /// <param name="objectSpace">Пространство объектов для загрузки и хранения объектов</param>
        /// <param name="code">Код последовательности</param>
        /// <param name="reset">Условие сброса значения последовательности</param>
        /// <returns>Следующее значение последовательности с кодом <b>code</b></returns>
        /// <remarks>Если значение последовательность не найдена, то создается новая последовательность. Если выполнено условие сброса 
        /// значения последовательности, то значение сбрасывается в 0 и возвращается следующее значение, т.е. 1.</remarks>
        /// <exception cref="ArgumentNullException">Не указаны пространство объектов или код последовательности</exception>
        /// <exception cref="ArgumentException">Указано условие сброса значения, отличное от исходного</exception>
        public static int GetNext(IObjectSpace objectSpace, string code, Resets reset = Resets.Never)
        {
            if (objectSpace == null) throw new ArgumentNullException("objectSpace");
            if (string.IsNullOrEmpty(code)) throw new ArgumentNullException("code");
            Sequence sequence = Find(objectSpace, code);
            if (sequence == null)
            {
                sequence = objectSpace.CreateObject<Sequence>();
                sequence.Code = code;
                sequence.Reset = reset;
                sequence.Current = 0;
                sequence.LastChange = DateTime.Now;
            }
            else
            {
                objectSpace.ReloadObject(sequence);
                if (sequence.Reset != reset)
                    throw new ArgumentException(string.Format("Reset of sequence {0} is different (parameter = {1}, original = {2})", code, reset, sequence.Reset), "reset");
            }
            var next = sequence.GetNext();
            return next;
        }

        /// <summary>Поля класса Sequence</summary>
        public static new readonly FieldsClass Fields = new FieldsClass();

        /// <summary>Поля класса Sequence</summary>
        public new class FieldsClass : BaseObject.FieldsClass
        {
            /// <inheritdoc/>
            public FieldsClass() { }
            /// <inheritdoc/>
            public FieldsClass(string propertyName) : base(propertyName) { }

            /// <summary>Операнд свойства Code</summary>
            public OperandProperty Code { get { return new OperandProperty(GetNestedName("Code")); } }
        }
    }
}
