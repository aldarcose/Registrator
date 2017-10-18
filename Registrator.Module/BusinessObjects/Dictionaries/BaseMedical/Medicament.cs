using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    [DefaultClassOptions]
    [XafDisplayName("Медицинские препараты")]
    [XafDefaultProperty("InterName")]
    public class Medicament : BaseObject
    {
        public Medicament(Session session) : base(session) { }

        /// <summary>
        /// Код препарата
        /// </summary>
        [XafDisplayName("Код препарата")]
        public string Code { get; set; }

        /// <summary>
        /// Код препарата
        /// </summary>
        [XafDisplayName("Наименование")]
        [VisibleInListView(true)]
        [VisibleInLookupListView(true)]
        [Size(255)]
        public string Name { get; set; }

        /// <summary>
        /// Код препарата
        /// </summary>
        [XafDisplayName("Международное название")]
        [VisibleInListView(true)]
        [VisibleInLookupListView(true)]
        [Size(255)]
        public string InterName { get; set; }
        
        /// <summary>
        /// Код препарата
        /// </summary>
        [XafDisplayName("Лекарственная форма")]
        [Size(255)]
        public string MedForm { get; set; }

        /// <summary>
        /// Код препарата
        /// </summary>
        [XafDisplayName("Производитель")]
        [Size(150)]
        public string Producer { get; set; }

        /// <summary>
        /// Код препарата
        /// </summary>
        [XafDisplayName("Страна-Производитель")]
        public string Country { get; set; }

        public override string ToString()
        {
            return string.IsNullOrEmpty(InterName) ? Name : InterName;
        }

        /// <summary>
        /// Добавляет в базу классификаторы из файла XML
        /// </summary>
        /// <param name="updater">Пространство объектов ObjectSpace</param>
        /// <param name="xmlPath">Путь до файла классификатора</param>
        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);

            foreach (XElement el in doc.Root.Elements())
            {
                //<Prep code="П0039603" mnn="~" Name="игла Микро-файн плюс №100 30G" form="~" producer="Becton Dick" country="ИРЛАНДИЯ" />
                string code = el.Attribute("code").Value;
                Medicament obj = objSpace.FindObject<Medicament>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", code));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<Medicament>();
                    obj.Code = el.Attribute("code").Value;
                    obj.Name = el.Attribute("Name").Value;
                    obj.InterName = el.Attribute("mnn").Value;
                    obj.MedForm = el.Attribute("form").Value;
                    obj.Producer = el.Attribute("producer").Value;
                    obj.Country = el.Attribute("country").Value;
                }
            }
        }
    }
}
