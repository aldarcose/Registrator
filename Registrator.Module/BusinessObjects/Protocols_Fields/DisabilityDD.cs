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
using DevExpress.Persistent.BaseImpl;
namespace Registrator.Module.BusinessObjects.Protocols_Fields
{
    class DisabilityDD
    {
        public DisabilityType DisabilityTypeValue { get; set; }
        public DateTime FirstTimeSetDate { get; set; }
        public DateTime LastMedUpdate { get; set; }
        public DateTime DateAppointment { get; set; }
        public DisabilityDeviationType DeviationType { get; set; }
        public IndivProgRehabChild IndivProgRehabChildType { get; set; }   
    }
    public enum DisabilityType
    {
        [XafDisplayName("Нет")]
        No = 0,
        [XafDisplayName("C рождения")]
        FromBirth = 1,
        [XafDisplayName("Приобретенная")]
        Obtained = 2
    }
    public enum IndivProgRehabChild
    {
        [XafDisplayName("Полностью")]
        Fully = 0,
        [XafDisplayName("Частично")]
        Parcel = 1,
        [XafDisplayName("Не выполнено")]
        NotFulfilled = 2,
        [XafDisplayName("Начато")]
        Initiation = 3
    }
    [DefaultClassOptions]
    public class DisabilityDeviationType : BaseObject
    {
        public DisabilityDeviationType(Session session) : base(session) { }

        public string Name { get; set; }
    }
}
