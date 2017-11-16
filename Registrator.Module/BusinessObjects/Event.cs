using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Registrator.Module.BusinessObjects
{
    /// <summary>
    /// Событие. Запись на прием к врачу
    /// </summary>
    /// <remarks>
    /// Класс реализует интерфейс <see cref="IRecurrentEvent"/> и <see cref="IEvent"/>
    /// для того, чтобы использовать интерфейс календаря XtraScheduler.
    /// </remarks>
    [DefaultProperty("Subject")]
    [NavigationItem("Default")]
    [DefaultListViewOptions(true, NewItemRowPosition.None)]
    public class DoctorEvent : BaseObject, IRecurrentEvent, IEvent
    {
        public DoctorEvent(Session session) : base(session) { }

        private EventImpl appointmentImpl = new EventImpl();
        [Persistent("RecurrencePattern")]
        private DoctorEvent recurrencePattern;
        private string recurrenceInfoXml;
        private Doctor doctor;
        private Pacient pacient;

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            appointmentImpl.AfterConstruction();
        }

        public bool AllDay
        {
            get { return appointmentImpl.AllDay; }
            set
            {
                bool oldValue = appointmentImpl.AllDay;
                appointmentImpl.AllDay = value;
                OnChanged("AllDay", oldValue, appointmentImpl.AllDay);
            }
        }

        [NonPersistent, Browsable(false)]
        public object AppointmentId
        {
            get { return Oid; }
        }

        [Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
        public string Description
        {
            get { return appointmentImpl.Description; }
            set
            {
                string oldValue = appointmentImpl.Description;
                appointmentImpl.Description = value;
                OnChanged("Description", oldValue, appointmentImpl.Description);
            }
        }

        [Indexed]
        public DateTime StartOn
        {
            get { return appointmentImpl.StartOn; }
            set
            {
                DateTime oldValue = appointmentImpl.StartOn;
                appointmentImpl.StartOn = value;
                OnChanged("StartOn", oldValue, appointmentImpl.StartOn);
            }
        }

        [Indexed]
        public DateTime EndOn
        {
            get { return appointmentImpl.EndOn; }
            set
            {
                DateTime oldValue = appointmentImpl.EndOn;
                appointmentImpl.EndOn = value;
                OnChanged("EndOn", oldValue, appointmentImpl.EndOn);
            }
        }

        public int Label
        {
            get { return appointmentImpl.Label; }
            set
            {
                int oldValue = appointmentImpl.Label;
                appointmentImpl.Label = value;
                OnChanged("Label", oldValue, appointmentImpl.Label);
            }
        }

        public string Location
        {
            get { return appointmentImpl.Location; }
            set
            {
                string oldValue = appointmentImpl.Location;
                appointmentImpl.Location = value;
                OnChanged("Location", oldValue, appointmentImpl.Location);
            }
        }

        [NonPersistent, Browsable(false)]
        public string ResourceId
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public int Status
        {
            get { return appointmentImpl.Status; }
            set
            {
                int oldValue = appointmentImpl.Status;
                appointmentImpl.Status = value;
                OnChanged("Status", oldValue, appointmentImpl.Status);
            }
        }

        [Size(250)]
        public string Subject
        {
            get { return appointmentImpl.Subject; }
            set
            {
                string oldValue = appointmentImpl.Subject;
                appointmentImpl.Subject = value;
                OnChanged("Subject", oldValue, appointmentImpl.Subject);
            }
        }

        [Browsable(false)]
        public int Type
        {
            get { return appointmentImpl.Type; }
            set
            {
                int oldValue = appointmentImpl.Type;
                appointmentImpl.Type = value;
                OnChanged("Type", oldValue, appointmentImpl.Type);
            }
        }

        [NonCloneable]
		[DevExpress.Xpo.DisplayName("Recurrence"), Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
        public string RecurrenceInfoXml
        {
            get { return recurrenceInfoXml; }
            set { SetPropertyValue("RecurrenceInfoXml", ref recurrenceInfoXml, value); }
        }

        [PersistentAlias("recurrencePattern")]
        public IRecurrentEvent RecurrencePattern
        {
            get { return recurrencePattern; }
            set { SetPropertyValue("RecurrencePattern", ref recurrencePattern, (DoctorEvent)value); }
        }

        public Doctor AssignedTo
        {
            get { return doctor; }
            set { SetPropertyValue("AssignedTo", ref doctor, value); } 
        }

        public Pacient Pacient
        {
            get { return pacient; }
            set { SetPropertyValue("Pacient", ref pacient, value); }
        }
    }
}
