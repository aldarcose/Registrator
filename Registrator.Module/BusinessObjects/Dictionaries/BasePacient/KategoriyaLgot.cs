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
    /// Категория льготников
    /// </summary>
    [DefaultClassOptions]
    public class KategoriyaLgot : BaseObject
    {
        // пример записи в XML:
        // <ROW kateg_lgot_id="1" kateg_lgot="Инвалиды"/>
        // <ROW kateg_lgot_id="2" kateg_lgot="Инвалиды&#160;детства"/>
        // <ROW kateg_lgot_id="3" kateg_lgot="Инвалиды&#160;ВОВ"/>
        // <ROW kateg_lgot_id="4" kateg_lgot="Инвалиды-участники&#160;ВОВ"/>

        public KategoriyaLgot() : base() { }
        public KategoriyaLgot(Session session) : base(session) { }

        /// <summary>
        /// Код категории
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Название категории
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

        /// <summary>
        /// Добавляет в базу классификаторы из файла XML
        /// </summary>
        /// <param name="updater">Пространство объектов ObjectSpace</param>
        /// <param name="xmlPath">Путь до файла классификатора</param>
        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);
            const string elementsContainer = "ROWDATA";
            const string elementNameStartsWith = "ROW";

            const string code_attr = "kateg_lgot_id";
            const string name_attr = "kateg_lgot";

            foreach (var element in doc.Root.Element(elementsContainer).Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                KategoriyaLgot obj = objSpace.FindObject<KategoriyaLgot>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<KategoriyaLgot>();
                    obj.Code = int.Parse(element.Attribute(code_attr).Value);
                    obj.Name = element.Attribute(name_attr).Value;
                }
            }
        }
    }
}
