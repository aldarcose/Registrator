using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    [DefaultClassOptions]
    public class Constants : BaseObject
    {
        public Constants(Session session) : base(session) { }

        [XafDisplayName("Имя константы")]
        [Size(100)]
        public string Name { get; set; }

        [XafDisplayName("Значение константы")]
        [Size(255)]
        public string Value { get; set; }
    }
}
