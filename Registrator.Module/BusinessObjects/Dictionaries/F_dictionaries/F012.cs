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
    /// Справочник ошибок форматно-логического контроля
    /// </summary>
    public class F012 : BaseObject
    {
        // пример записи в XML:
        // <item1 DATEBEG="01.01.2011" DATEEND="" DopInfo="Нарушен порядок следования тегов, либо отсутствует обязательный тег." Kod="901" Opis="Ошибочный порядок тегов"/>
        // <item2 DATEBEG="01.01.2011" DATEEND="" DopInfo="Отсутствует значение в обязательном теге." Kod="902" Opis="Отсутствует обязательное поле"/>

        public F012() : base() { }
        public F012(Session session) : base(session) { }

        /// <summary>
        /// Код ошибки
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Описание ошибки
        /// </summary>
        [Size(450)]
        public string Name { get; set; }

        /// <summary>
        /// Дополнительная информация об ошибке
        /// </summary>
        [Size(255)]
        public string DopInfo { get; set; }

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

            const string elementNameStartsWith = "item";

            const string code_attr = "Kod";
            const string name_attr = "Opis";
            const string dopInfo_attr = "DopInfo";
            
            const string dateBeg_attr = "DATEBEG";
            const string dateEnd_attr = "DATEEND";

            foreach (var element in doc.Root.Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                F012 obj = objSpace.FindObject<F012>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<F012>();
                    obj.Code = int.Parse(element.Attribute(code_attr).Value);
                    obj.Name = element.Attribute(name_attr).Value;

                    obj.DopInfo = element.Attribute(dopInfo_attr).Value;

                    obj.DateBeg = element.Attribute(dateBeg_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateBeg_attr).Value);
                    obj.DateEnd = element.Attribute(dateEnd_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateEnd_attr).Value);
                }
            }
        }
    }
}