using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// Список объектов
    /// </summary>
    [DefaultProperty("Name")]
    public abstract class ObjectList : BaseObject
    {
        private string name;
        private Type itemType;

        /// <summary>Конструктор</summary>
        public ObjectList() { }

        /// <summary>Конструктор</summary>
        /// <param name="session">Сессия хранимых объектов</param>
        public ObjectList(Session session) : base(session) { }

        /// <summary>
        /// Название списка
        /// </summary>
        [RuleRequiredField("Registrator.Module.BusinessObjects.Dictionaries.ObjectList.NameRequired", DefaultContexts.Save)]
        public string Name
        {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }

        /// <summary>
        /// Тип объектов
        /// </summary>
        [Persistent, Size(200)]
        [ValueConverter(typeof(TypeToStringConverter))]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        [VisibleInDetailView(false)]
        public Type ItemType
        {
            get { return itemType; }
            protected set { SetPropertyValue("ItemType", ref itemType, value); }
        }

        /// <summary>
        /// Описание типа объектов
        /// </summary>
        [NonPersistent]
        [VisibleInListView(true), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string ObjectsTypeDescription
        {
            get
            {
                if (ItemType != null)
                {
                    string classCaption = CaptionHelper.GetClassCaption(ItemType.FullName);
                    return string.IsNullOrEmpty(classCaption) ? ItemType.Name : classCaption;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Элементы списка (ссылки на объекты списка)
        /// </summary>
        [Association, Aggregated]
        [Browsable(false)]
        public XPCollection<ObjectListItem> Items
        {
            get { return GetCollection<ObjectListItem>("Items"); }
        }

        /// <summary>
        /// Количество объектов в списке
        /// </summary>
        [VisibleInDetailView(false), VisibleInLookupListView(false)]
        public virtual int ObjectsCount
        {
            get { return Items.Count; }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>Операнды свойств класса</summary>
        public static new readonly FieldsClass Fields = new FieldsClass();
        /// <summary>Операнды свойств класса</summary>
        public new class FieldsClass : BaseObject.FieldsClass
        {
            /// <summary>Конструктор</summary>
            public FieldsClass() { }
            /// <summary>Конструктор</summary>
            /// <param name="propertyName">Свойство, возвращающее объект класса</param>
            public FieldsClass(string propertyName) : base(propertyName) { }

            /// <summary>Операнд свойства Name</summary>
            public OperandProperty Name { get { return new OperandProperty(GetNestedName("Name")); } }
            /// <summary>Операнд свойства ItemType</summary>
            public OperandProperty ItemType { get { return new OperandProperty(GetNestedName("ItemType")); } }
            /// <summary>Операнд свойства Items</summary>
            public OperandProperty Items { get { return new OperandProperty(GetNestedName("Items")); } }
        }
    }

    /// <summary>
    /// Список хранимых объектов заданного типа
    /// </summary>
    /// <typeparam name="T">Тип хранимых объектов</typeparam>
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class ObjectList<T> : ObjectList
        where T : BaseObject
    {
        private ObjectListObjects<T> list;

        /// <summary>Конструктор</summary>
        public ObjectList() { }

        /// <summary>Конструктор</summary>
        /// <param name="session">Сессия хранимых объектов</param>
        public ObjectList(Session session) : base(session) { }

        /// <inheritdoc/>
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            ItemType = typeof(T);
        }

        /// <summary>
        /// Объекты списка
        /// </summary>
        [ImmediatePostData]
        [DataSourceCriteriaProperty("ObjectsCriteria")]
        public IList<T> Objects
        {
            get
            {
                if (list == null) list = new ObjectListObjects<T>(this);
                return list;
            }
        }

        /// <summary>Критерий выбора объектов по умолчанию</summary>
        public static CriteriaOperator DefaultObjectsCriteria;

        /// <summary>
        /// Критерий выбора объектов
        /// </summary>
        [NonPersistent]
        [Browsable(false)]
        public virtual CriteriaOperator ObjectsCriteria
        {
            get
            {
                if (ReferenceEquals(DefaultObjectsCriteria, null))
                {
                    DefaultObjectsCriteria = CriteriaOperator.Parse("1=1");
                }
                return DefaultObjectsCriteria;
            }
            set { SetPropertyValue<CriteriaOperator>("ObjectsCriteria", ref DefaultObjectsCriteria, value); }
        }
    }

    class ObjectListObjects<T> : IList<T>
    {
        private ObjectList parent;
        private List<T> list;

        public ObjectListObjects(ObjectList parent)
        {
            this.parent = parent;
            this.list = new List<T>();
            this.list.AddRange(parent.Items.Select<ObjectListItem, T>(item => parent.Session.GetObjectByKey<T>(item.ObjRef)));
        }

        private Guid CreateReference(T obj)
        {
            Guid key = (obj as BaseObject).Oid;
            return key;
        }

        private ObjectListItem CreateItem(T obj)
        {
            ObjectListItem item = new ObjectListItem(parent.Session);
            item.List = parent;
            item.ObjRef = (obj as BaseObject).Oid;
            return item;
        }

        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            Add(item);
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < list.Count) Remove(list[index]);
        }

        public T this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                if (value == null)
                {
                    Remove(value);
                }
                else if (index >= 0 && index < list.Count)
                {
                    Guid reference = CreateReference(list[index]);
                    ObjectListItem listItem = parent.Items.FirstOrDefault(item => item.ObjRef == reference);
                    if (listItem != null) listItem.ObjRef = CreateReference(value);
                    list[index] = value;
                }
            }
        }

        public void Add(T item)
        {
            CreateItem(item);
            list.Add(item);
        }

        public void Clear()
        {
            List<ObjectListItem> items = new List<ObjectListItem>(parent.Items);
            foreach (ObjectListItem item in items) item.Delete();
            list.Clear();
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            Guid reference = CreateReference(item);
            ObjectListItem listItem = parent.Items.FirstOrDefault(itm => itm.ObjRef == reference);
            if (listItem != null) listItem.Delete();
            return list.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }

    /// <summary>
    /// Элемент списка объектов, хранящий ссылку на объект
    /// </summary>
    [RuleCombinationOfPropertiesIsUnique("Registrator.Module.BusinessObjects.Dictionaries.ObjectListItem.UniqueItems",
        DefaultContexts.Save, "List, ObjRef")]
    public class ObjectListItem : BaseObject
    {
        private ObjectList list;
        private Guid objRef;

        /// <summary>Конструктор</summary>
        public ObjectListItem() { }

        /// <summary>Конструктор</summary>
        /// <param name="session">Сессия хранимых объектов</param>
        public ObjectListItem(Session session) : base(session) { }

        /// <summary>
        /// Список объектов, которому принадлежит элемент
        /// </summary>
        [Association]
        public ObjectList List
        {
            get { return list; }
            set { SetPropertyValue("List", ref list, value); }
        }

        /// <summary>
        /// Ссылка на объект в списке
        /// </summary>
        [Persistent]
        public Guid ObjRef
        {
            get { return objRef; }
            set { SetPropertyValue("ObjRef", ref objRef, value); }
        }

        /// <summary>
        /// Создает элемент списка объектов
        /// </summary>
        /// <param name="objectSpace">Пространство хранимых объектов</param>
        /// <param name="value">Объект, на который указывает ссылка</param>
        /// <returns>Элемент списка объектов со ссылкой на <paramref name="value"/></returns>
        /// <exception cref="ArgumentNullException">Не указаны пространство хранимых объектов или объект.</exception>
        /// <exception cref="ArgumentException">Объект не является хранимым или еще не сохранен.</exception>
        public static ObjectListItem Create(IObjectSpace objectSpace, object value)
        {
            if (objectSpace == null)
                throw new ArgumentNullException("objectSpace");
            if (value == null)
                throw new ArgumentNullException("value");

            //ObjectListItem result = objectSpace.CreateObject<ObjectListItem>();
            //Reference objRef = new Reference();
            //objRef.SetObject(objectSpace, value);
            //result.ObjRef = objRef;
            //return result;
            return null;
        }

        /// <summary>Операнды свойств класса</summary>
        public static new readonly FieldsClass Fields = new FieldsClass();
        /// <summary>Операнды свойств класса</summary>
        public new class FieldsClass : BaseObject.FieldsClass
        {
            /// <summary>Конструктор</summary>
            public FieldsClass() { }
            /// <summary>Конструктор</summary>
            /// <param name="propertyName">Свойство, возвращающее объект класса</param>
            public FieldsClass(string propertyName) : base(propertyName) { }

            /// <summary>Операнд свойства List</summary>
            public ObjectList.FieldsClass List { get { return new ObjectList.FieldsClass(GetNestedName("List")); } }
            ///// <summary>Операнд свойства ObjRef</summary>
            //public Reference.FieldsClass ObjRef { get { return new Reference.FieldsClass(GetNestedName("ObjRef")); } }
        }
    }
}
