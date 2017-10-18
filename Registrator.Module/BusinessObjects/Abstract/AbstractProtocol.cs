using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.Persistent.BaseImpl;

namespace Registrator.Module.BusinessObjects.Abstract
{
    public abstract class AbstractProtocol : BaseObject{
        public AbstractProtocol(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (http://documentation.devexpress.com/#Xaf/CustomDocument2834).

            //this.DoctorCreated = CurrentDoctor; // установить при открытии в контроллере
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
        }

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
