using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
namespace Registrator.Module.BusinessObjects.Protocols_Fields
{
    public class HeightField : ProtocolField
    {
        public HeightField(Session session) : base(session) { }
        public int Value { get; set; }
    }
}
