using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Xml.Linq;
namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// Вид медицинской помощи
    /// Классификатор видов медицинской помощи. Справочник V008 Приложения А.
    /// </summary>
    public class V008 : BaseObject
    {
        public V008() { }
        public V008(Session session) : base(session) { }

        /// <summary>
        /// Код
        /// </summary>
        [Size(4)]
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

        // пример записи из XML
        // <rec1 IDVMP="1" VMPNAME="Первичная медико-санитарная помощь" DATEBEG="01.01.2011" DATEEND="" />
        // <rec2 IDVMP="2" VMPNAME="Скорая, в том числе специализированная медицинская помощь" DATEBEG="01.01.2011" DATEEND="" />
        // <rec3 IDVMP="3" VMPNAME="Специализированная медицинская помощь" DATEBEG="01.01.2011" DATEEND="" />

        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);

            foreach (XElement el in doc.Root.Elements())
            {
                string id = el.Attribute("IDVMP").Value;
                V008 obj = objSpace.FindObject<V008>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", id));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<V008>();
                    obj.Code = el.Attribute("IDVMP").Value;
                    obj.DateBeg = el.Attribute("DATEBEG").Value == "" ? null : (DateTime?)Convert.ToDateTime(el.Attribute("DATEBEG").Value);
                    obj.DateEnd = el.Attribute("DATEEND").Value == "" ? null : (DateTime?)Convert.ToDateTime(el.Attribute("DATEEND").Value);
                    obj.Name = el.Attribute("VMPNAME").Value;
                }
            }
        }
    }
}
