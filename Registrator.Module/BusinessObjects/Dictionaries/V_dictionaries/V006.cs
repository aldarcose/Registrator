using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System.Xml.Linq;
using System;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// Условия оказания медицинской помощи
    /// Классификатор условий оказания медицинской помощи (V006 Приложения А).
    /// </summary>
    public class V006: BaseObject
    {
        public V006() { }
        public V006(Session session) : base(session) { }

        /// <summary>
        /// Код
        /// </summary>
        [Size(2)]
        public string Code { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [Size(255)]
        public string Name { get; set; }
        public DateTime? DateBeg { get; set; }
        public DateTime? DateEnd { get; set; }
        /// <summary>
        /// Текстовое представление
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [PersistentAlias("Concat('(', Code, ') ', Name)")]
        public string Text { get { return (string)EvaluateAlias("Text"); } }

        public override string ToString()
        {
            return String.Format("({0}) {1}", Code, Name);
        }

        // пример записи в XML
        // <rec1 IDUMP="1" UMPNAME="Стационаро" DATEBEG="18.10.2012" DATEEND="" />
        // <rec2 IDUMP="2" UMPNAME="В дневном стационаре" DATEBEG="18.10.2012" DATEEND="" />

        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);

            foreach (XElement el in doc.Root.Elements())
            {
                string id = el.Attribute("IDUMP").Value;
                V006 obj = objSpace.FindObject<V006>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", id));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<V006>();
                    obj.Code = el.Attribute("IDUMP").Value;
                    obj.DateBeg = el.Attribute("DATEBEG").Value == "" ? null : (DateTime?)Convert.ToDateTime(el.Attribute("DATEBEG").Value);
                    obj.DateEnd = el.Attribute("DATEEND").Value == "" ? null : (DateTime?)Convert.ToDateTime(el.Attribute("DATEEND").Value);
                    obj.Name = el.Attribute("UMPNAME").Value;
                }
            }
        }
    }
}
