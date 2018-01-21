using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Xpo;

namespace Registrator.Module.BusinessObjects.Abstract
{
    public abstract class AbstractProtocol : BaseObject
    {
        private Doctor currentDoctor;

        public AbstractProtocol(Session session) : base(session) { }

        [Browsable(false)]
        public Doctor DoctorCreated { get; set; }

        /// <summary>
        /// Текущий доктор
        /// </summary>
        [Browsable(false)]
        public Doctor CurrentDoctor
        {
            get 
            {
                if (currentDoctor == null)
                {
                    IObjectSpace os = XPObjectSpace.FindObjectSpaceByObject(this);
                    currentDoctor = os.GetObject(SecuritySystem.CurrentUser as Doctor);
                }
                return currentDoctor; 
            }
        }
    }
}
