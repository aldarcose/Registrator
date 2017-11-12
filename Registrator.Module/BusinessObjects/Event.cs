using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.ComponentModel;
using System.Xml;

namespace Registrator.Module.BusinessObjects
{
    /// <summary>
    /// Событие
    /// </summary>
    [DefaultProperty("Subject")]
    [NavigationItem("Default")]
    [DefaultListViewOptions(true, NewItemRowPosition.None)]
    public class Event : BaseObject, IRecurrentEvent, IEvent
    {
        public Event(Session session) : base(session) 
        {
            session.ObjectSaving += new ObjectManipulationEventHandler(session_ObjectSaving);
            Doctors.ListChanged += new ListChangedEventHandler(Resources_ListChanged);
        }

        [Persistent("ResourceIds"), Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
        private string resourceIds;
        private EventImpl appointmentImpl = new EventImpl();
        [Persistent("RecurrencePattern")]
        private Event recurrencePattern;
        private string recurrenceInfoXml;

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            appointmentImpl.AfterConstruction();
        }

        public void UpdateResourceIds()
        {
            resourceIds = "<ResourceIds>\r\n";
            foreach (Doctor resource in Doctors)
            {
                resourceIds += string.Format("<ResourceId Type=\"{0}\" Value=\"{1}\" />\r\n", resource.Id.GetType().FullName, resource.Id);
            }
            resourceIds += "</ResourceIds>";
        }

        private void UpdateResources()
        {
            Doctors.SuspendChangedEvents();
            try
            {
                while (Doctors.Count > 0)
                {
                    Doctors.Remove(Doctors[0]);
                }
                if (!String.IsNullOrEmpty(resourceIds))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(resourceIds);
                    foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
                    {
                        Doctor resource = Session.GetObjectByKey<Doctor>(new Guid(xmlNode.Attributes["Value"].Value));
                        if (resource != null)
                        {
                            Doctors.Add(resource);
                        }
                    }
                }
            }
            finally
            {
                Doctors.ResumeChangedEvents();
            }
        }

        private void Resources_ListChanged(object sender, ListChangedEventArgs e)
        {
            if ((e.ListChangedType == ListChangedType.ItemAdded) ||
                (e.ListChangedType == ListChangedType.ItemDeleted))
            {
                UpdateResourceIds();
                OnChanged("ResourceId");
            }
        }

        private void session_ObjectSaving(object sender, ObjectManipulationEventArgs e)
        {
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            if (Doctors.IsLoaded && !Session.IsNewObject(this))
            {
                Doctors.Reload();
            }
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
                if (resourceIds == null)
                {
                    UpdateResourceIds();
                }
                return resourceIds;
            }
            set
            {
                if (resourceIds != value)
                {
                    resourceIds = value;
                    UpdateResources();
                }
            }
        }

        [Association("Event-Doctor", typeof(Doctor))]
        public XPCollection Doctors
        {
            get { return GetCollection("Doctors"); }
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
            set { SetPropertyValue("RecurrencePattern", ref recurrencePattern, (Event)value); }
        }
    }
}
