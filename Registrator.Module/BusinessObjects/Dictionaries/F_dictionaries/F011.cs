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
    /// Классификатор типов документа, удостоверяющих личность
    /// </summary>
    public class F011 : BaseObject
    {
        // пример записи в XML:
        // <ZAP IDDoc="1" DocName="Паспорт гражданина СССР" DocSer="R-ББ" DocNum="999999" DATEBEG="01.01.2011" DATEEND="" />
        // <ZAP IDDoc="2" DocName="Загранпаспорт гражданина СССР" DocSer="S" DocNum="00000009" DATEBEG="01.01.2011" DATEEND="" />
        // <ZAP IDDoc="3" DocName="Свидетельство о рождении, выданное в Российской Федерации" DocSer="R-ББ" DocNum="999999" DATEBEG="01.01.2011" DATEEND="" />

        public F011() : base() { }
        public F011(Session session) : base(session) { }

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
        /// Серия документа
        /// todo: для каждого типа документа задать шаблон серии
        /// </summary>
        [Size(20)]
        public string DocumentSerial { get; set; }

        /// <summary>
        /// Номер документа
        /// todo: для каждого типа документа задать шаблон номера
        /// </summary>
        [Size(20)]
        public string DocumentNumber { get; set; }

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

            const string elementNameStartsWith = "ZAP";

            const string code_attr = "IDDoc";
            const string name_attr = "DocName";
            const string serial_attr = "DocSer";
            const string number_attr = "DocNum";

            const string dateBeg_attr = "DATEBEG";
            const string dateEnd_attr = "DATEEND";

            foreach (var element in doc.Root.Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                F011 obj = objSpace.FindObject<F011>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<F011>();
                    obj.Code = int.Parse(element.Attribute(code_attr).Value);
                    obj.Name = element.Attribute(name_attr).Value;

                    obj.DocumentSerial = element.Attribute(serial_attr).Value;
                    obj.DocumentNumber = element.Attribute(number_attr).Value;

                    obj.DateBeg = element.Attribute(dateBeg_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateBeg_attr).Value);
                    obj.DateEnd = element.Attribute(dateEnd_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateEnd_attr).Value);
                }
            }
        }
    }
}