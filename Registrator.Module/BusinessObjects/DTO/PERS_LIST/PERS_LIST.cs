using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
namespace Registrator.Module.BusinessObjects.DTO
{
    [DefaultClassOptions]
    public partial class PERS_LIST : BaseObject
    {
        public PERS_LIST()
        {
        }
        public PERS_LIST(Session session)
            : base(session)
        {
        }

        public ZGLV ZGLV { get; set; }

        [Aggregated]
        [Association("PERS_PERSLIST")]
        public XPCollection<PERS> People
        {
            get { return GetCollection<PERS>("People"); }
        }
    }
}
