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
    public class SexualProgress : ProtocolField
    {
        public SexualProgress(Session session) : base(session) { }
        public int PValue { get; set; }
        public int AxValue { get; set; }
        public int MaValue { get; set; }
        public int MeValue { get; set; }
        public int YearsValue { get; set; }
        public int MonthValue { get; set; }
        public MensesPeriod MensesPeriodValue { get; set; }
        public MensesQuantity MensesQuantityValue { get; set; }
        
    }
    public enum MensesPeriod
    {
        [XafDisplayName("Регулярные")]
        Regular = 0,
        [XafDisplayName("Нерегулярные")]
        NonRegular = 1
    }
    public enum MensesQuantity
    {
        [XafDisplayName("Обильные")]
        High = 0,
        [XafDisplayName("Скудные")]
        Low = 1,
        [XafDisplayName("Умеренные")]
        Medium = 2
    }
}
