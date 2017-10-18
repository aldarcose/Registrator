using DevExpress.Persistent.Base;

using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.Module.BusinessObjects.DTO
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    [DefaultClassOptions]
    public partial class ZL_LIST : DevExpress.Persistent.BaseImpl.BaseObject
    {
        public ZL_LIST(Session session)
            : base(session)
        {

        }

        public ZGLV ZGLV { get; set; }

        public SCHET SCHET { get; set; }

        [Aggregated, Association("ZAPLIST_ZAP")]
        public XPCollection<ZAP> ZAPS
        {
            get { return GetCollection<ZAP>("ZAPS"); }
        }
    }

}
