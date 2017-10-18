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
using System.Xml.Linq;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// Вид медицинского вмешательства (V001)
    /// </summary>
    [DefaultClassOptions]
    [XafDisplayName("Вид мед. вмешательства")]
    public class VidMedVmeshatelstva : DevExpress.Persistent.BaseImpl.BaseObject
    {
        public VidMedVmeshatelstva(Session session)
            : base(session)
        {
        }

        /// <summary>
        /// Идентификатор вида вмешательства
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Код вида вмешательства
        /// </summary>
        [Size(50)]
        [XafDisplayName("Код")]
        public string Code { get; set; }

        [Size(255)]
        [XafDisplayName("Наименование")]
        public string Name { get; set; }

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

            const string elementNameStartsWith = "entry";

            const string id_attr = "id";
            const string code_attr = "s_code";
            const string name_attr = "s_name";

            foreach (var element in doc.Descendants("entry"))
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                VidMedVmeshatelstva obj = objSpace.FindObject<VidMedVmeshatelstva>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Id=?", element.Element(id_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<VidMedVmeshatelstva>();
                    obj.Id = int.Parse(element.Element(id_attr).Value);
                    obj.Code = element.Element(code_attr).Value;
                    obj.Name = element.Element(name_attr).Value;
                }
            }
        }
    }
}
