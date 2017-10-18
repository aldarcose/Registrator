using System;
using System.Collections.Generic;
using System.Xml.Linq;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// Классификатор видов контроля
    /// </summary>
    public class F006 : BaseObject
    {
        // пример записи в XML:
        // <rec1 IDVID="1" VIDNAME="МЭК (медико-экономический контроль)" DATEBEG="01.01.2011" DATEEND="" />
        // <rec2 IDVID="2" VIDNAME="МЭЭ (медико-экономическая экспертиза)" DATEBEG="01.01.2011" DATEEND="" />
        // <rec3 IDVID="3" VIDNAME="ЭКМП (экспертиза качества медицинской помощи)" DATEBEG="01.01.2011" DATEEND="" />

        public F006() : base() { }
        public F006(Session session) : base(session) { }

        /// <summary>
        /// Код
        /// </summary>
        public int Code {get;set;}

        /// <summary>
        /// Наименование
        /// </summary>
        [Size(255)]
        public string Name {get;set;}

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

            const string code_attr = "IDVID";
            const string name_attr = "VIDNAME";

            const string dateBeg_attr = "DATEBEG";
            const string dateEnd_attr = "DATEEND";

            foreach (var element in doc.Root.Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                F006 obj = objSpace.FindObject<F006>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<F006>();
                    obj.Code = int.Parse(element.Attribute(code_attr).Value);
                    obj.Name = element.Attribute(name_attr).Value;

                    obj.DateBeg = element.Attribute(dateBeg_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateBeg_attr).Value);
                    obj.DateEnd = element.Attribute(dateEnd_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateEnd_attr).Value);
                }
            }
        }
    }
}
