using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Registrator.Module.BusinessObjects.Dictionaries.BaseStacionar
{
    [DefaultClassOptions]
    [XafDisplayName("Справочник соответствия КСГ и видов мед. вмешательства")]
    public class CsgVidVme : BaseObject
    {
        public CsgVidVme(Session session) : base(session) { }

        /// <summary>
        /// Номер КСГ
        /// </summary>
        [XafDisplayName("Номер КСГ")]
        public int CsgNumber { get; set; }

        [XafDisplayName("Вид мед. вмешательства")]
        public VidMedVmeshatelstva VidVme { get; set; }
    }
}
