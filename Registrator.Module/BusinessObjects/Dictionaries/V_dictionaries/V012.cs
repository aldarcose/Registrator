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
    /// Классификатор исходов заболевания
    /// </summary>
    public class V012 : BaseObject
    {
        // пример записи в XML:
        // <item1 IDIZ="101" IZNAME="Выздоровление" DL_USLOV="1" DATEBEG="01.01.2011" DATEEND="" />
        // <item2 IDIZ="102" IZNAME="Улучшение" DL_USLOV="1" DATEBEG="01.01.2011" DATEEND="" />
        // <item3 IDIZ="103" IZNAME="Без перемен" DL_USLOV="1" DATEBEG="01.01.2011" DATEEND="" />
        // <item4 IDIZ="104" IZNAME="Ухудшение" DL_USLOV="1" DATEBEG="01.01.2011" DATEEND="" />

        public V012() : base() { }
        public V012(Session session) : base(session) { }

        /// <summary>
        /// Код исхода
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Size(255)]
        public string Name { get; set; }

        /// <summary>
        /// Длительность условия
        /// </summary>
        public string Dl_Uslov { get; set; }

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

            const string code_attr = "IDIZ";
            const string name_attr = "IZNAME";

            const string dlUsl_attr = "DL_USLOV";

            const string dateBeg_attr = "DATEBEG";
            const string dateEnd_attr = "DATEEND";

            foreach (var element in doc.Root.Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                V012 obj = objSpace.FindObject<V012>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<V012>();
                    obj.Code = int.Parse(element.Attribute(code_attr).Value);
                    obj.Name = element.Attribute(name_attr).Value;

                    obj.Dl_Uslov = element.Attribute(dlUsl_attr).Value;

                    obj.DateBeg = element.Attribute(dateBeg_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateBeg_attr).Value);
                    obj.DateEnd = element.Attribute(dateEnd_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateEnd_attr).Value);
                }
            }
        }
    }
}