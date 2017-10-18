using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using Registrator.Module.BusinessObjects.Interfaces;
using System.Xml.Linq;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// V010 - Классификатор способов оплаты мед. помощи
    /// </summary>
    [DefaultClassOptions]
    [XafDisplayName("Способ оплаты мед. помощи")]
    public class SposobOplatiMedPom : BaseObject
    {
        // пример записи в XML:
        // <rec IDSP="1" SPNAME="Посещение в поликлинике" DATEBEG="01.01.2013" DATEEND="" />
        // <rec IDSP="2" SPNAME="Оперативное вмешательство" DATEBEG="01.01.2013" DATEEND="" />
        // <rec IDSP="3" SPNAME="Анестезиологическое пособие" DATEBEG="01.01.2013" DATEEND="" />

        public SposobOplatiMedPom(Session session) : base(session) { }

        /// <summary>
        /// Код способа
        /// </summary>
        [XafDisplayName("Код")]
        public int Code { get; set; }

        /// <summary>
        /// Имя способа
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

            const string code_attr = "IDSP";
            const string name_attr = "SPNAME";

            const string dateBeg_attr = "DATEBEG";
            const string dateEnd_attr = "DATEEND";

            foreach (var element in doc.Root.Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                SposobOplatiMedPom obj = objSpace.FindObject<SposobOplatiMedPom>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));

                if (obj == null)
                {
                    obj = objSpace.CreateObject<SposobOplatiMedPom>();
                    obj.Code = int.Parse(element.Attribute(code_attr).Value);
                    obj.Name = element.Attribute(name_attr).Value;

                    obj.DateBeg = element.Attribute(dateBeg_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateBeg_attr).Value);
                    obj.DateEnd = element.Attribute(dateEnd_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateEnd_attr).Value);
                }
            }
        }
    }
}
