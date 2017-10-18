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
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (http://documentation.devexpress.com/#Xaf/CustomDocument2701).
    public class Otdel : DevExpress.Persistent.BaseImpl.BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (http://documentation.devexpress.com/#Xaf/CustomDocument3146).
        public Otdel(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (http://documentation.devexpress.com/#Xaf/CustomDocument2834).
        }
        
        [XafDisplayName("ID")]
        /// <summary>
        /// Код отдела
        /// </summary>
        public string ID { get; set; }
        [XafDisplayName("Код")]
        /// <summary>
        /// Код реестра
        /// </summary>
        public string CODE { get; set; }

        [XafDisplayName("Название")]
        /// <summary>
        /// Название отдела
        /// </summary>
        [Size(255)]
        public string NAME { get; set; }

        public override string ToString()
        {
            return CODE;
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
                Otdel obj = objSpace.FindObject<Otdel>(DevExpress.Data.Filtering.CriteriaOperator.Parse("ID=?", id));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<Otdel>();
                    obj.ID = el.Attribute("id").Value;
                    obj.CODE = el.Attribute("code").Value;
                    obj.NAME = el.Attribute("name").Value;
                }
            }
        }
    }
}
