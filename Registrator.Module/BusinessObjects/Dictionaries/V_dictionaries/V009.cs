using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    public class V009 : BaseObject
    {
        public V009(Session session)
            : base(session)
        {
        }

        /// <summary>
        /// Код
        /// </summary>
        [Size(50)]
        public string Code { get; set; }

        public DateTime? DateBeg { get; set; }
        public DateTime? DateEnd { get; set; }

        [Size(50)]
        public string DlUslov { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        [Size(255)]
        public string Name { get; set; }

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

        //пример записи из XML
        // <rec1 IDRMP="101" RMPNAME="Выписан" DL_USLOV="1" DATEBEG="01.01.2011" DATEEND="" />
        // <rec2 IDRMP="102" RMPNAME="Переведён в др. ЛПУ" DL_USLOV="1" DATEBEG="01.01.2011" DATEEND="" />
        // <rec3 IDRMP="103" RMPNAME="Переведён в дневной стационар" DL_USLOV="1" DATEBEG="01.01.2011" DATEEND="" />
        // <rec4 IDRMP="104" RMPNAME="Переведён на другой профиль коек" DL_USLOV="1" DATEBEG="01.01.2011" DATEEND="" />

        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);

            foreach (XElement el in doc.Root.Elements())
            {
                V009 obj = objSpace.FindObject<V009>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", el.Attribute("IDRMP").Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<V009>();
                    obj.Code = el.Attribute("IDRMP").Value;
                    obj.DateBeg = el.Attribute("DATEBEG").Value == "" ? null : (DateTime?)Convert.ToDateTime(el.Attribute("DATEBEG").Value);
                    obj.DateEnd = el.Attribute("DATEEND").Value == "" ? null : (DateTime?)Convert.ToDateTime(el.Attribute("DATEEND").Value);
                    obj.Name = el.Attribute("RMPNAME").Value;
                    obj.DlUslov = el.Attribute("DL_USLOV").Value;
                }
            }
        }
    }
}
