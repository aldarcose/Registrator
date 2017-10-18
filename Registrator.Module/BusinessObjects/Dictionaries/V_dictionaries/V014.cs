using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System.Xml.Linq;
using System;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// Форма оказания медицинской помощи
    /// Классификатор форм оказания медицинской помощи. Спра-вочник V014 Приложения А
    /// </summary>
    public class V014 : BaseObject
    {
        public V014() { }
        public V014(Session session) : base(session) { }

        // пример записи из XML
        // <rec1 IDFRMMP="1" FRMMPNAME="Экстренная" DATEBEG="01.01.2013" DATEEND="" />
        // <rec2 IDFRMMP="2" FRMMPNAME="Неотложная" DATEBEG="01.01.2013" DATEEND="" />
        // <rec3 IDFRMMP="3" FRMMPNAME="Плановая" DATEBEG="01.01.2013" DATEEND="" />

        /// <summary>
        /// Код
        /// </summary>
        [Size(1)]
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

        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);

            foreach (XElement el in doc.Root.Elements())
            {
                string id = el.Attribute("IDFRMMP").Value;
                V014 obj = objSpace.FindObject<V014>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", id));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<V014>();
                    obj.Code = el.Attribute("IDFRMMP").Value;
                    obj.DateBeg = el.Attribute("DATEBEG").Value == "" ? null : (DateTime?)Convert.ToDateTime(el.Attribute("DATEBEG").Value);
                    obj.DateEnd = el.Attribute("DATEEND").Value == "" ? null : (DateTime?)Convert.ToDateTime(el.Attribute("DATEEND").Value);
                    obj.Name = el.Attribute("FRMMPNAME").Value;
                }
            }
        }
    }
}
