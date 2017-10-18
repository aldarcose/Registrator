using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Editors;
using DevExpress.Xpo;
namespace Registrator.Module.BusinessObjects.Protocols_Fields
{

    public class PhysProgress : ProtocolField
    {
        public PhysProgress(Session session) : base(session) { }
        [ImmediatePostData(true)]
        public PhysValue Value { get; set; }

        [Appearance("TypeVisibility", Criteria = "[Value]=0", Visibility = ViewItemVisibility.Hide, Context = "DetailView")]
        public PhysType Type { get; set; }
    }

    public enum PhysValue
    {
        [XafDisplayName("Нормальное")]
        Normal = 0,
        [XafDisplayName("С нарушениями")]
        WithDeviation = 1
    }

    public enum PhysType
    {
        [XafDisplayName("Дефицит массы")]
        WeightDeficit,
        [XafDisplayName("Избыток массы")]
        WeightExcifit,
        [XafDisplayName("Высокий рост")]
        Tall,
        [XafDisplayName("Низкий рост")]
        Short
    }
}
