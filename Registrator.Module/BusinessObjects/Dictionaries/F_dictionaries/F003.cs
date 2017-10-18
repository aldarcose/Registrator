using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using DevExpress.ExpressApp.Updating;
using System.Xml.Linq;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// Код МО, направившего на лечение (диагностику, консультацию)
    /// Код МО – юридического лица. Заполня-ется в соответствии со справочником F003 Приложения А. При отсутствии сведений может не заполняться.
    /// </summary>
    public class F003 : BaseObject
    {
        public F003() { }
        public F003(Session session) : base(session) { }

        /// <summary>
        /// Код
        /// </summary>
        [Size(1)]
        public string Code { get; set; }

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
    }
}
