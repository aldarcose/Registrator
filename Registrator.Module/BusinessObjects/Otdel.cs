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
using DevExpress.Persistent.Validation;
using System.Xml.Linq;

namespace Registrator.Module.BusinessObjects
{

    [DefaultClassOptions]
    public class Otdel : DevExpress.Persistent.BaseImpl.BaseObject
    {
        private DoctorEvent doctorEvent;
        private BusinessObjects.DoctorEventLabel doctorEventLabel;
        public Otdel(Session session) : base(session) { }    
        
        /// <summary>
        /// Код отдела
        /// </summary>
        [XafDisplayName("Код отделения")]
        public string Id { get; set; }
        
        /// <summary>
        /// Код реестра
        /// </summary>
        [XafDisplayName("Реестровый код")]
        public string Code { get; set; }

        
        /// <summary>
        /// Название отдела
        /// </summary>
        [Size(255)]
        [XafDisplayName("Название")]
        public string Name { get; set; }

        /// <summary>
        /// Доктора отдела
        /// </summary>
        [Association("Otdel-Doctor")]
        public XPCollection<Doctor> Doctors
        {
            get { return GetCollection<Doctor>("Doctors"); }
        }

        /// <summary>
        /// Вид талона
        /// </summary>
        [Association]
        public DoctorEventLabel DoctorEventLabel
        {
            get { return doctorEventLabel; }
            set { SetPropertyValue("DoctorEventLabel", ref doctorEventLabel, value); }
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Добавляет в базу классификаторы из файла XML
        /// </summary>
        /// <param name="updater">Пространство объектов ObjectSpace</param>
        /// <param name="xmlPath">Путь до файла классификатора</param>
        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);

            foreach (XElement el in doc.Root.Element("ROWDATA").Elements("ROW"))
            {
                string id = el.Attribute("id").Value;
                Otdel obj = objSpace.FindObject<Otdel>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Id=?", id));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<Otdel>();
                    obj.Id = el.Attribute("id").Value;
                    obj.Code = el.Attribute("code").Value;
                    obj.Name = el.Attribute("name").Value;
                }
            }
        }
    }
}
