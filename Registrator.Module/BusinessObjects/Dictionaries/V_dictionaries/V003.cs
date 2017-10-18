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
    /// Классификатор работ (услуг) при лицензировании мед. помощи
    /// </summary>
    public class V003 : BaseObject
    {
        // пример записи в XML:
        // <rec2 IDRL="2" LICNAME="акушерскому делу;" IERARH="1" PRIM="2" DATEBEG="01.01.2011" DATEEND="24.05.2013" />
        // <rec3 IDRL="3" LICNAME="анестезиологии и реаниматологии;" IERARH="1" PRIM="2" DATEBEG="01.01.2011" DATEEND="24.05.2013" />
        // <rec4 IDRL="4" LICNAME="гистологии;" IERARH="1" PRIM="2" DATEBEG="01.01.2011" DATEEND="24.05.2013" />
        // <rec5 IDRL="5" LICNAME="диетологии;" IERARH="1" PRIM="2" DATEBEG="01.01.2011" DATEEND="24.05.2013" />

        public V003() : base() { }
        public V003(Session session) : base(session) { }

        /// <summary>
        /// Код услуги
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [Size(650)]
        public string Name { get; set; }

        /// <summary>
        /// Иерархия
        /// </summary>
        public int Ierarh { get; set; }

        /// <summary>
        /// Prim
        /// </summary>
        public int Prim { get; set; }

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

            const string code_attr = "IDRL";
            const string name_attr = "LICNAME";

            const string ierarh_attr = "IERARH";
            const string prim_attr = "PRIM";

            const string dateBeg_attr = "DATEBEG";
            const string dateEnd_attr = "DATEEND";

            foreach (var element in doc.Root.Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                V003 obj = objSpace.FindObject<V003>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<V003>();
                    obj.Code = int.Parse(element.Attribute(code_attr).Value);
                    obj.Name = element.Attribute(name_attr).Value;

                    obj.Ierarh = (element.Attribute(ierarh_attr).Value == "") ? 0 : int.Parse(element.Attribute(ierarh_attr).Value);
                    obj.Prim = int.Parse(element.Attribute(prim_attr).Value);

                    obj.DateBeg = element.Attribute(dateBeg_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateBeg_attr).Value);
                    obj.DateEnd = element.Attribute(dateEnd_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateEnd_attr).Value);
                }
            }
        }
    }
}