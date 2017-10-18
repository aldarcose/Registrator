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
    /// Классификатор видов диспансеризации
    /// </summary>
    [DefaultClassOptions]
    public class VidDispanserizacii : BaseObject
    {
        // пример записи в XML:
        // <rec IDDT="ДВ1" DTNAME="Первый этап диспансеризации определенных групп взрослого населения" DTRULE="1, 2, 3, 6" DATEBEG="26.12.2013" DATEEND="05.03.2014" />
        // <rec IDDT="ОПВ" DTNAME="Профилактические медицинские осмотры взрослого населения" DTRULE="1, 2, 3" DATEBEG="26.12.2013" DATEEND="" />

        public VidDispanserizacii() : base() { }
        public VidDispanserizacii(Session session) : base(session) { }

        /// <summary>
        /// Код диспансеризации
        /// todo: код, аббревиатура, категория?
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Название диспансеризации
        /// </summary>
        [Size(255)]
        public string Name { get; set; }

        /// <summary>
        /// Список кодов правил
        /// todo: что за правила?
        /// </summary>
        public string Rules { get; set; }
        
        /// <summary>
        /// Дата начала действия
        /// </summary>
        public DateTime? DateBeg { get; set; }

        /// <summary>
        /// Дата окончания действия
        /// </summary>
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

            const string elementNameStartsWith = "rec";

            const string code_attr = "IDDT";
            const string name_attr = "DTNAME";

            const string rules_attr = "DTRULE";

            const string dateBeg_attr = "DATEBEG";
            const string dateEnd_attr = "DATEEND";

            foreach (var element in doc.Root.Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                VidDispanserizacii obj = objSpace.FindObject<VidDispanserizacii>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<VidDispanserizacii>();
                    obj.Code = element.Attribute(code_attr).Value;
                    obj.Name = element.Attribute(name_attr).Value;
                    obj.Rules = element.Attribute(rules_attr).Value;

                    obj.DateBeg = element.Attribute(dateBeg_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateBeg_attr).Value);
                    obj.DateEnd = element.Attribute(dateEnd_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateEnd_attr).Value);
                }
            }
        }
    }
}
