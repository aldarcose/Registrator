using System;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using System.Xml.Linq;
using DevExpress.ExpressApp.DC;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// V002 Классификатор профилей оказанной медицинской помощи
    /// </summary>
    [DefaultClassOptions]
    public class MedProfil : BaseObject
    {
        // пример записи в XML:
        // <zap IDPR="1" PRNAME="хирургии (абдоминальной)" DATEBEG="01.01.2011" DATEEND="" />
        // <zap IDPR="2" PRNAME="акушерству и гинекологии" DATEBEG="01.01.2011" DATEEND="08.07.2013" />
        // <zap IDPR="3" PRNAME="акушерскому делу" DATEBEG="01.01.2011" DATEEND="" />
        // <zap IDPR="4" PRNAME="аллергологии и иммунологии" DATEBEG="01.01.2011" DATEEND="" />
        // <zap IDPR="5" PRNAME="анестезиологии и реаниматологии" DATEBEG="01.01.2011" DATEEND="" />
        // <zap IDPR="6" PRNAME="бактериологии" DATEBEG="01.01.2011" DATEEND="" />
        public MedProfil() : base() { }
        public MedProfil(Session session) : base(session) { }

        /// <summary>
        /// Код
        /// </summary>
        [XafDisplayName("Код профиля")]
        public int Code { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [Size(450)]
        [XafDisplayName("Имя профиля")]
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
        /// Специальности профиля
        /// </summary>
        [Association("MedProfil-DoctorSpecs")]
        public XPCollection<DoctorSpecTree> Specs
        {
            get
            {
                return GetCollection<DoctorSpecTree>("Specs");
            }
        }

        /// <summary>
        /// Добавляет в базу классификаторы из файла XML
        /// </summary>
        /// <param name="updater">Пространство объектов ObjectSpace</param>
        /// <param name="xmlPath">Путь до файла классификатора</param>
        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);

            const string elementNameStartsWith = "zap";

            const string code_attr = "IDPR";
            const string name_attr = "PRNAME";

            const string dateBeg_attr = "DATEBEG";
            const string dateEnd_attr = "DATEEND";

            foreach (var element in doc.Root.Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                MedProfil obj = objSpace.FindObject<MedProfil>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<MedProfil>();
                    obj.Code = int.Parse(element.Attribute(code_attr).Value);
                    obj.Name = element.Attribute(name_attr).Value;

                    obj.DateBeg = element.Attribute(dateBeg_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateBeg_attr).Value);
                    obj.DateEnd = element.Attribute(dateEnd_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateEnd_attr).Value);
                }
            }
        }
    }
}
