using System;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using System.Xml.Linq;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// Классификатор типов документа, удостоверяющих личность
    /// </summary>
    [DefaultClassOptions]
    public class VidDocumenta : BaseObject
    {
        // пример записи в XML:
        // <ZAP IDDoc="1" DocName="Паспорт гражданина СССР" DocSer="R-ББ" DocNum="999999" DATEBEG="01.01.2011" DATEEND="" />
        // <ZAP IDDoc="2" DocName="Загранпаспорт гражданина СССР" DocSer="S" DocNum="00000009" DATEBEG="01.01.2011" DATEEND="" />
        // <ZAP IDDoc="3" DocName="Свидетельство о рождении, выданное в Российской Федерации" DocSer="R-ББ" DocNum="999999" DATEBEG="01.01.2011" DATEEND="" />

        public VidDocumenta() : base() { }
        public VidDocumenta(Session session) : base(session) { }

        /// <summary>
        /// Код
        /// </summary>
        [XafDisplayName("Код типа")]
        public int Code { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [Size(255)]
        [XafDisplayName("Тип документа")]
        public string Name { get; set; }

        /*
         * Паспорт гражданина СССР: DocSer="R-ББ" DocNum="999999"
         * Загранпаспорт гражданина СССР: DocSer="S" DocNum="00000009"
         * Свидетельство о рождении, выданное в Российской Федерации: DocSer="R-ББ" DocNum="999999"
         * Удостоверение личности офицера: DocSer="ББ" DocNum="9999999"
         * Справка об освобождении из места лишения свободы: DocSer="S" DocNum="00000009"
         * Паспорт Минморфлота: DocSer="ББ" DocNum="999999"
         * Военный билет: DocSer="ББ" DocNum="9999990"
         * Дипломатический паспорт гражданина Российской Федерации: DocSer="99" DocNum="9999999"
         * Паспорт иностранного гражданина: DocSer="S" DocNum="0000000009"
         * Свидетельство о регистрации ходатайства о признании иммигранта беженцем на территории Российской Федерации: DocSer="S" DocNum="00000009"
         * Вид на жительство: DocSer="S1" DocNum="00000009"
         * Удостоверение беженца в Российской Федерации: DocSer="S" DocNum="00000009"
         * Временное удостоверение личности гражданина Российской Федерации: DocSer="S" DocNum="00000009"
         * Паспорт гражданина Российской Федерации: DocSer="99 99" DocNum="9999990"
         * Заграничный паспорт гражданина Российской Федерации: DocSer="99" DocNum="9999999"
         * Паспорт моряка: DocSer="ББ" DocNum="9999990"
         * Военный билет офицера запаса: DocSer="ББ" DocNum="999999"
         * Иные документы: DocSer="S1" DocNum="0000000009"
         * Документ иностранного гражданина: DocSer="S1" DocNum="000000000009"
         * Документ лица без гражданства: DocSer="S1" DocNum="000000000009"
         * Разрешение на временное проживание: DocSer="S1" DocNum="000000000009"
         * Свидетельство о рождении, выданное не в Российской Федерации: DocSer="S1" DocNum="000000000009"
         * Свидетельство о предоставлении временного убежища на территории Российской Федерации: DocSer="99" DocNum="9999999"
        */

        /// <summary>
        /// Регулярное выражения для серии
        /// </summary>
        [NonPersistent()]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string MaskSerial
        {
            get
            {
                var mask = string.Empty;
                
                

                switch (Code)
                {
                    // Паспорт гражданина СССР: DocSer="R-ББ" DocNum="999999"
                    case 1:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("R-ББ");
                        break;
                    // Загранпаспорт гражданина СССР: DocSer="S" DocNum="00000009"
                    case 2:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("S");;
                        break;
                    // Свидетельство о рождении, выданное в Российской Федерации: DocSer="R-ББ" DocNum="999999"
                    case 3:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("R-ББ");
                        break;
                    // Удостоверение личности офицера: DocSer="ББ" DocNum="9999999"
                    case 4:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("ББ");
                        break;
                    // Справка об освобождении из места лишения свободы: DocSer="S" DocNum="00000009"
                    case 5:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("S");
                        break;
                    // Паспорт Минморфлота: DocSer="ББ" DocNum="999999"
                    case 6:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("ББ");
                        break;
                    // Военный билет: DocSer="ББ" DocNum="9999990"
                    case 7:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("ББ");
                        break;
                    // Дипломатический паспорт гражданина Российской Федерации: DocSer="99" DocNum="9999999"
                    case 8:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("99");
                        break;
                    // Паспорт иностранного гражданина: DocSer="S" DocNum="0000000009"
                    case 9:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("S");
                        break;
                    // Свидетельство о регистрации ходатайства о признании иммигранта беженцем на территории Российской Федерации: DocSer="S" DocNum="00000009"
                    case 10:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("S");
                        break;
                    // Вид на жительство: DocSer="S1" DocNum="00000009"
                    case 11:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("S1");
                        break;
                    // Удостоверение беженца в Российской Федерации: DocSer="S" DocNum="00000009"
                    case 12:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("S");
                        break;
                    // Временное удостоверение личности гражданина Российской Федерации: DocSer="S" DocNum="00000009"
                    case 13:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("S");
                        break;
                    // Паспорт гражданина Российской Федерации: DocSer="99 99" DocNum="9999990"
                    case 14:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("99 99");
                        break;
                    // Заграничный паспорт гражданина Российской Федерации: DocSer="99" DocNum="9999999"
                    case 15:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("99");
                        break;
                    // Паспорт моряка: DocSer="ББ" DocNum="9999990"
                    case 16:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("ББ");
                        break;
                    // Военный билет офицера запаса: DocSer="ББ" DocNum="999999"
                    case 17:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("ББ");
                        break;
                    // Иные документы: DocSer="S1" DocNum="0000000009"
                    case 18:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("S1");
                        break;
                    // Документ иностранного гражданина: DocSer="S1" DocNum="000000000009"
                    case 19:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("S1");
                        break;
                    // Документ лица без гражданства: DocSer="S1" DocNum="000000000009"
                    case 20:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("S1");
                        break;
                    // Разрешение на временное проживание: DocSer="S1" DocNum="000000000009"
                    case 21:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("S1");
                        break;
                    //Свидетельство о рождении, выданное не в Российской Федерации: DocSer="S1" DocNum="000000000009"
                    case 22:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("S1");
                        break;
                    // Свидетельство о предоставлении временного убежища на территории Российской Федерации: DocSer="99" DocNum="9999999"
                    case 23:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("99");
                        break;
                }
                return mask;
            }
        }

        /// <summary>
        /// Регулярное выражения для номера
        /// </summary>
        [NonPersistent()]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string MaskNumber
        {
            get
            {
                var mask = string.Empty;
                switch (Code)
                {
                    // Паспорт гражданина СССР: DocSer="R-ББ" DocNum="999999"
                    case 1:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("999999");
                        break;
                    // Загранпаспорт гражданина СССР: DocSer="S" DocNum="00000009"
                    case 2:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("00000009");
                        break;
                    // Свидетельство о рождении, выданное в Российской Федерации: DocSer="R-ББ" DocNum="999999"
                    case 3:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("999999");
                        break;
                    // Удостоверение личности офицера: DocSer="ББ" DocNum="9999999"
                    case 4:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("9999999");
                        break;
                    // Справка об освобождении из места лишения свободы: DocSer="S" DocNum="00000009"
                    case 5:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("00000009");
                        break;
                    // Паспорт Минморфлота: DocSer="ББ" DocNum="999999"
                    case 6:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("999999");
                        break;
                    // Военный билет: DocSer="ББ" DocNum="9999990"
                    case 7:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("9999990");
                        break;
                    // Дипломатический паспорт гражданина Российской Федерации: DocSer="99" DocNum="9999999"
                    case 8:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("9999999");
                        break;
                    // Паспорт иностранного гражданина: DocSer="S" DocNum="0000000009"
                    case 9:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("0000000009");
                        break;
                    // Свидетельство о регистрации ходатайства о признании иммигранта беженцем на территории Российской Федерации: DocSer="S" DocNum="00000009"
                    case 10:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("00000009");
                        break;
                    // Вид на жительство: DocSer="S1" DocNum="00000009"
                    case 11:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("00000009");
                        break;
                    // Удостоверение беженца в Российской Федерации: DocSer="S" DocNum="00000009"
                    case 12:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("00000009");
                        break;
                    // Временное удостоверение личности гражданина Российской Федерации: DocSer="S" DocNum="00000009"
                    case 13:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("00000009");
                        break;
                    // Паспорт гражданина Российской Федерации: DocSer="99 99" DocNum="9999990"
                    case 14:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("9999990");
                        break;
                    // Заграничный паспорт гражданина Российской Федерации: DocSer="99" DocNum="9999999"
                    case 15:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("9999999");
                        break;
                    // Паспорт моряка: DocSer="ББ" DocNum="9999990"
                    case 16:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("9999990");
                        break;
                    // Военный билет офицера запаса: DocSer="ББ" DocNum="999999"
                    case 17:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("999999");
                        break;
                    // Иные документы: DocSer="S1" DocNum="0000000009"
                    case 18:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("0000000009");
                        break;
                    // Документ иностранного гражданина: DocSer="S1" DocNum="000000000009"
                    case 19:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("000000000009");
                        break;
                    // Документ лица без гражданства: DocSer="S1" DocNum="000000000009"
                    case 20:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("000000000009");
                        break;
                    // Разрешение на временное проживание: DocSer="S1" DocNum="000000000009"
                    case 21:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("000000000009");
                        break;
                    //Свидетельство о рождении, выданное не в Российской Федерации: DocSer="S1" DocNum="000000000009"
                    case 22:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("000000000009");
                        break;
                    // Свидетельство о предоставлении временного убежища на территории Российской Федерации: DocSer="99" DocNum="9999999"
                    case 23:
                        mask = DocSerialNumberTemplate.GetRegexTemplate("9999999");
                        break;
                }
                return mask;
            }
        }

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

            /*const string serial_attr = "DocSer";
            const string number_attr = "DocNum";
            */
            const string dateBeg_attr = "DATEBEG";
            const string dateEnd_attr = "DATEEND";

            foreach (var element in doc.Root.Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                VidDocumenta obj = objSpace.FindObject<VidDocumenta>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<VidDocumenta>();
                    obj.Code = int.Parse(element.Attribute(code_attr).Value);
                    obj.Name = element.Attribute(name_attr).Value;

                    /*
                    obj.DocumentSerial = element.Attribute(serial_attr).Value;
                    obj.DocumentNumber = element.Attribute(number_attr).Value;
                     */

                    obj.DateBeg = element.Attribute(dateBeg_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateBeg_attr).Value);
                    obj.DateEnd = element.Attribute(dateEnd_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateEnd_attr).Value);
                }
            }
        }
    }
}