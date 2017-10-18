using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Registrator.Module.BusinessObjects.Dictionaries;
using System.ComponentModel;

namespace Registrator.Module.BusinessObjects
{
    [DefaultClassOptions]
    public abstract class ProtocolField : BaseObject
    {
        protected ProtocolField(Session session) : base(session) { }

        /*[Browsable(false)]
        [Association("Protocol-Fields")]
        public EditableProtocol Protocol { get; set; }
        */
        // public bool flazhok { get; set; } 

        //[Size(100)]
        // public string textbox { get; set; }

        // public int chislo { get; set; }
        
        // public byte chislo0_255 { get; set; }
    }
}
