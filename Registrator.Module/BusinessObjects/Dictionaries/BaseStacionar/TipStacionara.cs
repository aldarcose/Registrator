using System;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System.Xml.Linq;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.DC;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    [DefaultClassOptions]
    [XafDisplayName("Тип стационара")]
    public class TipStacionara : BaseObject
    {
        public TipStacionara(Session session) : base(session) { }

        /// <summary>
        /// Код стационара
        /// </summary>
        [XafDisplayName("Код")]
        public int Code { get; set; }

        /// <summary>
        /// Наименование типа стационара
        /// </summary>
        [Size(255)]
        [XafDisplayName("Наименование")]
        public string Name { get; set; }

        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);
            const string elementNameStartsWith = "rec";

            const string code_attr = "CODE";
            const string name_attr = "NAME";


            foreach (var element in doc.Root.Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                TipStacionara obj = objSpace.FindObject<TipStacionara>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<TipStacionara>();
                    obj.Code = int.Parse(element.Attribute(code_attr).Value);
                    obj.Name = element.Attribute(name_attr).Value;
                }
            }
        }
    }
}
