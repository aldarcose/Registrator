using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.Persistent.BaseImpl;

namespace Registrator.Module.BusinessObjects.Abstract
{
    public abstract class AbstractProtocol : BaseObject
    {
        public AbstractProtocol(Session session) : base(session) { }

        [Browsable(false)]
        public Doctor DoctorCreated { get; set; }

        /// <summary>
        /// Текущий доктор
        /// </summary>
        [Browsable(false)]
        [NonPersistent]
        public Doctor CurrentDoctor
        {
            get
            {
                return SecuritySystem.CurrentUser as Doctor;
            }
        }
    }
}
