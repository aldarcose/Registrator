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
    /// Классификатор субъектов Российской Федерации
    /// </summary>
    public class F010 : BaseObject
    {
        // пример записи в XML:
        // <rec1 KOD_TF="01" KOD_OKATO="79000" SUBNAME="Республика Адыгея" OKRUG="2" DATEBEG="01.01.2011" DATEEND="" />
        // <rec2 KOD_TF="02" KOD_OKATO="80000" SUBNAME="Республика Башкортостан" OKRUG="7" DATEBEG="01.01.2011" DATEEND="" />
        // <rec3 KOD_TF="03" KOD_OKATO="81000" SUBNAME="Республика Бурятия" OKRUG="5" DATEBEG="01.01.2011" DATEEND="" />
        // <rec4 KOD_TF="04" KOD_OKATO="84000" SUBNAME="Республика Алтай" OKRUG="5" DATEBEG="01.01.2011" DATEEND="" />

        public F010() : base() { }
        public F010(Session session) : base(session) { }

        /// <summary>
        /// Код
        /// </summary>
        [Size(10)]
        public string Code { get; set; }

        /// <summary>
        /// Код ОКАТО
        /// </summary>
        public int CodeOKATO { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [Size(255)]
        public string Name { get; set; }

        /// <summary>
        /// Федеральный округ
        /// </summary>
        public int Okrug { get; set; }

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

            const string code_attr = "KOD_TF";
            const string name_attr = "SUBNAME";
            const string okato_attr = "KOD_OKATO";
            const string okrug_attr = "OKRUG";

            const string dateBeg_attr = "DATEBEG";
            const string dateEnd_attr = "DATEEND";

            foreach (var element in doc.Root.Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                F010 obj = objSpace.FindObject<F010>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<F010>();
                    obj.Code = element.Attribute(code_attr).Value;
                    obj.Name = element.Attribute(name_attr).Value;

                    obj.CodeOKATO = int.Parse(element.Attribute(okato_attr).Value);
                    obj.Okrug = int.Parse(element.Attribute(okrug_attr).Value);

                    obj.DateBeg = element.Attribute(dateBeg_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateBeg_attr).Value);
                    obj.DateEnd = element.Attribute(dateEnd_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateEnd_attr).Value);
                }
            }
        }
    }
}