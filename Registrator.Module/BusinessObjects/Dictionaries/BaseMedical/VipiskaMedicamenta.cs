using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    [DefaultClassOptions]
    [XafDisplayName("Назначение препарата")]
    public class VipiskaMedicamenta : BaseObject
    {
        public VipiskaMedicamenta(Session session) : base(session) { }

        [XafDisplayName("Препарат")]
        public Medicament Medicament { get; set; }

        [XafDisplayName("Начало приема")]
        public DateTime Start { get; set; }

        [XafDisplayName("Конец приема")]
        public DateTime End { get; set; }

        [Size(450)]
        [XafDisplayName("Комментарий")]
        public string Comment { get; set; }

        [Browsable(false)][Association("Protocol-Medicaments")]
        public CommonProtocol Protocol { get; set; }
    }
}
