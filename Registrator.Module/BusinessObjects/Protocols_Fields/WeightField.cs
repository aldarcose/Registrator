using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Registrator.Module.BusinessObjects
{
    public class WeightField : ProtocolField
    {
        public WeightField(Session session) : base(session) { }

        // public bool flazhok { get; set; } 

        //[Size(100)]
        // public string textbox { get; set; }

        public int Value { get; set; }
        
        // public byte chislo0_255 { get; set; }
    }
}
