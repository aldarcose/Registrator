using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    [DefaultClassOptions]
    public class Constants : BaseObject
    {
        /// <summary>
        /// Стоматологическая специальность
        /// </summary>
        public static string Dentistry = "DENTISTRY";

        public Constants(Session session) : base(session) { }

        [XafDisplayName("Имя константы")]
        [Size(100)]
        public string Name { get; set; }

        [Size(200)]
        [XafDisplayName("Описание")]
        public string Description { get; set; }

        [XafDisplayName("Значение константы")]
        [Size(255)]
        public string Value { get; set; }

        /// <summary>Операнды свойств класса</summary>
        public static new readonly FieldsClass Fields = new FieldsClass();
        /// <summary>Операнды свойств класса</summary>
        public new class FieldsClass : BaseObject.FieldsClass
        {
            /// <summary>Конструктор</summary>
            public FieldsClass() { }
            /// <summary>Конструктор</summary>
            /// <param name="propertyName">Название вложенного свойства</param>
            public FieldsClass(string propertyName) : base(propertyName) { }

            /// <summary>Операнд свойства Name</summary>
            public OperandProperty Name { get { return new OperandProperty(GetNestedName("Name")); } }
            /// <summary>Операнд свойства Period</summary>
            public OperandProperty Value { get { return new OperandProperty(GetNestedName("Value")); } }
        }
    }
}
