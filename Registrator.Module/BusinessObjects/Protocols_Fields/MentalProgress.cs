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
    public class MentalProgress: ProtocolField
    { 
        public  MentalProgress(Session session) : base(session) { }
        [ImmediatePostData(true)]
        public Psych PsychValue { get; set; }
        public Intellekt IntellektValue { get; set; }
        public EmotVeg EmotVegValue { get; set; }
    }
    public enum Psych
    {
        [XafDisplayName("Нормальное")]
        Normal = 0,
        [XafDisplayName("С нарушениями")]
        WithDeviation = 1
    }
    public enum Intellekt
    {
        [XafDisplayName("Нормальное")]
        Normal = 0,
        [XafDisplayName("С нарушениями")]
        WithDeviation = 1
    }
    public enum EmotVeg
    {
        [XafDisplayName("Нормальное")]
        Normal = 0,
        [XafDisplayName("С нарушениями")]
        WithDeviation = 1
    }
}
