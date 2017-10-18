using System;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using System.Xml.Linq;
namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// Классификатор типов документов, подтверждающих факт страхования по ОМС
    /// </summary>
    public class F008 : BaseObject
    {
        // пример записи в XML:
        // <rec1 IDDOC="1" DOCNAME="Полис ОМС старого образца" DATEBEG="01.01.2011" DATEEND="" />
        // <rec2 IDDOC="2" DOCNAME="Временное свидетельство, подтверждающее оформление полиса обязательного медицинского страхования" DATEBEG="01.01.2011" DATEEND="" />
        // <rec3 IDDOC="3" DOCNAME="Полис ОМС единого образца" DATEBEG="01.01.2011" DATEEND="" />

        public F008() : base() { }
        public F008(Session session) : base(session) { }

        /// <summary>
        /// Код
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [Size(255)]
        public string Name { get; set; }

        /// <summary>
        /// Дата начала действия
        /// </summary>
        public DateTime? DateBeg { get; set; }

        /// <summary>
        /// Дата окончания действия
        /// </summary>
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

            const string code_attr = "IDDOC";
            const string name_attr = "DOCNAME";

            const string dateBeg_attr = "DATEBEG";
            const string dateEnd_attr = "DATEEND";

            foreach (var element in doc.Root.Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                F008 obj = objSpace.FindObject<F008>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<F008>();
                    obj.Code = int.Parse(element.Attribute(code_attr).Value);
                    obj.Name = element.Attribute(name_attr).Value;

                    obj.DateBeg = element.Attribute(dateBeg_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateBeg_attr).Value);
                    obj.DateEnd = element.Attribute(dateEnd_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateEnd_attr).Value);
                }
            }
        }
    }
}
