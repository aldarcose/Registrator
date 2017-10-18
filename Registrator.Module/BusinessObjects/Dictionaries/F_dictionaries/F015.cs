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
    /// Классификатор по округам </summary>
    public class F015 : BaseObject
    {
        // пример записи в XML:
        // <rec1 KOD_OK="1" OKRNAME="Центральный федеральный округ" DATEBEG="01.01.2011" DATEEND="" />
        // <rec2 KOD_OK="2" OKRNAME="Южный федеральный округ" DATEBEG="01.01.2011" DATEEND="" />
        // <rec3 KOD_OK="3" OKRNAME="Северо-Западный федеральный округ" DATEBEG="01.01.2011" DATEEND="" />
        
        // ALERT!!: в XML нет крымского федерального округа.

        public F015() : base() { }
        public F015(Session session) : base(session) { }

        /// <summary>
        /// Код федерального округа
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Наименование федерального округа
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

            const string code_attr = "KOD_OK";
            const string name_attr = "OKRNAME";
            const string dateBeg_attr = "DATEBEG";
            const string dateEnd_attr = "DATEEND";

            foreach (var element in doc.Root.Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                F015 obj = objSpace.FindObject<F015>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<F015>();
                    obj.Code = int.Parse(element.Attribute(code_attr).Value);
                    obj.Name = element.Attribute(name_attr).Value;
                    obj.DateBeg = element.Attribute(dateBeg_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateBeg_attr).Value);
                    obj.DateEnd = element.Attribute(dateEnd_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateEnd_attr).Value);
                }
            }
        }
    }
}