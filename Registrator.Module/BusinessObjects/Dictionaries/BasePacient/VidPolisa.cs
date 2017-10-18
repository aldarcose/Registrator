using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Registrator.Module.BusinessObjects.Dictionaries
{    /// <summary>
    /// Виды полисов ОМС
    /// </summary>
    [DefaultClassOptions]
    public class VidPolisa : BaseObject
    {
        public VidPolisa() { }
        public VidPolisa(Session session) : base(session) { }

        /// <summary>
        /// Код типа полиса (1 - полис старого образца, 2 - временное свидетельство, 3 - полис единого образца)
        /// </summary>
        [Size(10)]
        [XafDisplayName("Код типа полиса")]
        public string Code { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [Size(255)]
        [XafDisplayName("Наименование типа")]
        public string Name { get; set; }

        /// <summary>
        /// Регулярное выражение для серии
        /// </summary>
        [NonPersistent()]
        [Browsable(false)]
        public string MaskSerial
        {
            get
            {
                var mask = string.Empty;
                switch(Code)
                {
                    // ОМС старого образца
                        // опеределить маску
                    case "1":
                        mask = @"\w*";
                        break;
                    // Временное свидетельство
                        // Нет серии
                    case "2": break;
                    // ОМС единого образца
                        // Нет серии
                    case "3": break;
                    default:
                        break;
                }

                return mask;
            }
        }

        /// <summary>
        /// Регулярное выражение для номера
        /// </summary>
        [NonPersistent()]
        [Browsable(false)]
        public string MaskNumber
        {
            get
            {
                var mask = string.Empty;
                switch (Code)
                {
                    // ОМС старого образца
                    case "1":
                        mask = DocSerialNumberTemplate.GetRegexTemplate("000000"); // 6 цифр
                        break;
                    // Временное свидетельство
                    case "2": 
                        mask = DocSerialNumberTemplate.GetRegexTemplate("000000000"); // 9 цифр
                        break;
                    // ОМС единого образца
                    case "3":
                        mask = DocSerialNumberTemplate.GetRegexTemplate("9999999999999999");
                        break;
                    default:
                        break;
                }

                return mask;
            }
        }

        /// <summary>
        /// Дата начала действия
        /// </summary>
        [Browsable(false)]
        public DateTime? DateBeg { get; set; }

        /// <summary>
        /// Дата окончания действия
        /// </summary>
        [Browsable(false)]
        public DateTime? DateEnd { get; set; }

        
        /// <summary>
        /// Текстовое представление
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [PersistentAlias("Concat('(', Code, ') ', Name)")]
        public string Text { get { return (string)EvaluateAlias("Text"); } }

        public override string ToString()
        {
            return String.Format("({0}) {1}", Code, Name);
        }

        /// <summary>
        /// Добавляет в базу классификаторы из файла XML
        /// </summary>
        /// <param name="updater">Пространство объектов ObjectSpace</param>
        /// <param name="xmlPath">Путь до файла классификатора</param>
        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);

            const string code_attr = "IDDOC";
            const string name_attr = "DOCNAME";
            const string dateBeg_attr = "DATEBEG";
            const string dateEnd_attr = "DATEEND";

            foreach (XElement el in doc.Root.Elements())
            {
                string id = el.Attribute(code_attr).Value;
                VidPolisa obj = objSpace.FindObject<VidPolisa>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", id));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<VidPolisa>();
                    obj.Code = el.Attribute(code_attr).Value;
                    obj.Name = el.Attribute(name_attr).Value;

                    obj.DateBeg = el.Attribute(dateBeg_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(el.Attribute(dateBeg_attr).Value);
                    obj.DateEnd = el.Attribute(dateEnd_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(el.Attribute(dateEnd_attr).Value);
                }
            }
        }
    }
}

