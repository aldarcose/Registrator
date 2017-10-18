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
    class Vaccinations
    {
        public HoldingVaccinations HoldingVaccinationsType { get; set; }
    }
    [DefaultClassOptions]
    public class HoldingVaccinations : BaseObject
    {
        public HoldingVaccinations(Session session) : base(session) { }

        public string Name { get; set; }
    }
}
