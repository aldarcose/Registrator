using System;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using System.Xml.Linq;
using DevExpress.ExpressApp.DC;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// Классификатор условий оказания мед. помощи V006
    /// </summary>
    [DefaultClassOptions]
    [XafDisplayName("Виды условий оказания мед. помощи")]
    public class VidUsloviyOkazMedPomoshi : BaseObject
    {
        // пример записи в XML:
        // <zap IDUMP="1" UMPNAME="Стационарно" DATEBEG="18.10.2012" DATEEND="" />
        // <zap IDUMP="2" UMPNAME="В дневном стационаре" DATEBEG="18.10.2012" DATEEND="" />

        public VidUsloviyOkazMedPomoshi() : base() { }
        public VidUsloviyOkazMedPomoshi(Session session) : base(session) { }

        /// <summary>
        /// Код
        /// </summary>
        [XafDisplayName("Код")]
        public int Code { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [Size(255)]
        [XafDisplayName("Наименование")]
        public string Name { get; set; }

        /// <summary>
        /// Дата начала действия
        /// </summary>
        [XafDisplayName("Дата начала действия")]
        public DateTime? DateBeg { get; set; }

        /// <summary>
        /// Дата окончания действия
        /// </summary>
        [XafDisplayName("Дата окончания действия")]
        public DateTime? DateEnd { get; set; }

        /// <summary>
        /// Добавляет в базу классификаторы из файла XML
        /// </summary>
        /// <param name="updater">Пространство объектов ObjectSpace</param>
        /// <param name="xmlPath">Путь до файла классификатора</param>
        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);

            const string elementNameStartsWith = "rec";

            const string code_attr = "IDUMP";
            const string name_attr = "UMPNAME";

            const string dateBeg_attr = "DATEBEG";
            const string dateEnd_attr = "DATEEND";

            foreach (var element in doc.Root.Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                VidUsloviyOkazMedPomoshi obj = objSpace.FindObject<VidUsloviyOkazMedPomoshi>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<VidUsloviyOkazMedPomoshi>();
                    obj.Code = int.Parse(element.Attribute(code_attr).Value);
                    obj.Name = element.Attribute(name_attr).Value;

                    obj.DateBeg = element.Attribute(dateBeg_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateBeg_attr).Value);
                    obj.DateEnd = element.Attribute(dateEnd_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateEnd_attr).Value);
                }
            }
        }
    }
}
