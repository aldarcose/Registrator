using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    [DefaultClassOptions]
    [XafDisplayName("История передачи стационара")]
    public class StacionarBindDoctorHistory : BaseObject
    {
        public StacionarBindDoctorHistory(Session session) : base(session) { }

        [Association("Stacionar_BindHistory")]
        [XafDisplayName("Дневной стационар")]
        public DnevnoyStacionar Stacionar { get; set; }

        [XafDisplayName("Дата передачи")]
        public DateTime BindDate { get; set; }

        [XafDisplayName("Передал")]
        public Doctor OldDoctor { get; set; }
        [XafDisplayName("Передано")]
        public Doctor NewDoctor { get; set; }

        [XafDisplayName("Основание")]
        [Size(500)]
        public string Reason { get; set; }
    }
}
