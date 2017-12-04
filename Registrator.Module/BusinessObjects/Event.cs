using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.ComponentModel;
using System.Drawing;
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
        private Doctor createdBy;
        private Doctor editedBy;
        private DoctorEventSourceType sourceType;

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            appointmentImpl.AfterConstruction();
            Doctor curDoctor = SecuritySystem.CurrentUser as Doctor;
            this.CreatedBy = Session.GetObjectByKey<Doctor>(curDoctor.Oid);
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

        [Browsable(false)]
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

        [RuleRequiredField("Registrator.Module.BusinessObjects.DoctorEvent.AssignedToRequired", DefaultContexts.Save)]
        public Doctor AssignedTo
        {
            get { return doctor; }
            set { SetPropertyValue("AssignedTo", ref doctor, value); } 
        }

        /// <summary>
        /// Пациент
        /// </summary>
        public Pacient Pacient
        {
            get { return pacient; }
            set 
            {
                SetPropertyValue("Pacient", ref pacient, value);
                if (!IsLoading && !IsSaving && pacient != null)
                {
                    Doctor curDoctor = SecuritySystem.CurrentUser as Doctor;
                    this.EditedBy = Session.GetObjectByKey<Doctor>(curDoctor.Oid);
                }
            }
        }

        /// <summary>
        /// Кем создан
        /// </summary>
        [ModelDefault("AllowEdit", "False")]
        public Doctor CreatedBy
        {
            get { return createdBy; }
            set { SetPropertyValue("CreatedBy", ref createdBy, value); }
        }

        /// <summary>
        /// Кто записал пациента
        /// </summary>
        [ModelDefault("AllowEdit", "False")]
        public Doctor EditedBy
        {
            get { return editedBy; }
            set { SetPropertyValue("EditedBy", ref editedBy, value); }
        }

        /// <summary>
        /// Источник записи на прием
        /// </summary>
        public DoctorEventSourceType SourceType
        {
            get { return sourceType; }
            set { SetPropertyValue("SourceType", ref sourceType, value); }
        }

        /// <summary>Операнды свойств класса</summary>
        public static new readonly FieldsClass Fields = new FieldsClass();
        /// <summary>Операнды свойств класса</summary>
        public new class FieldsClass : BaseObject.FieldsClass
        {
            /// <summary>Конструктор</summary>
            public FieldsClass() { }
            /// <summary>Конструктор</summary>
            /// <param name="propertyName">Название вложенного свойства</param>
            public FieldsClass(string propertyName) : base(propertyName) { }

            /// <summary>Операнд свойства StartOn</summary>
            public OperandProperty StartOn { get { return new OperandProperty(GetNestedName("StartOn")); } }
            /// <summary>Операнд свойства EndOn</summary>
            public OperandProperty EndOn { get { return new OperandProperty(GetNestedName("EndOn")); } }
            /// <summary>Операнд свойства AssignedTo</summary>
            public Doctor.FieldsClass AssignedTo { get { return new Doctor.FieldsClass(GetNestedName("AssignedTo")); } }
        }
    }

    /// <summary>
    /// Вид талона
    /// </summary>
    /// <remarks>
    /// Используется для закрашивания цветами расписания врачей
    /// </remarks>
    public class DoctorEventLabel : BaseObject
    {
        private Color color;
        private string name;
        private int colorArgb;

        public DoctorEventLabel(Session session) : base(session) { }

        /// <summary>
        /// Цвет
        /// </summary>
        public Color Color
        {
            get { return System.Drawing.Color.FromArgb(ColorArgb); }
            set 
            {
                if (color != value)
                {
                    color = value;
                    ColorArgb = color.ToArgb();
                }
            }
        }

        /// <summary>
        /// Представление цвета в ARGB
        /// </summary>
        [Browsable(false)]
        public int ColorArgb
        {
            get { return colorArgb; }
            set { SetPropertyValue("ColorArgb", ref colorArgb, value); }
        }

        /// <summary>
        /// Название
        /// </summary>
        public string Name
        {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }
    }

    /// <summary>
    /// Источник записи на прием к доктору
    /// </summary>
    public enum DoctorEventSourceType
    {
        /// <summary>
        /// Нет
        /// </summary>
        None = 1,

        /// <summary>
        /// Регистратура
        /// </summary>
        Registry = 2,

        /// <summary>
        /// Интернет
        /// </summary>
        Internet = 3,

        /// <summary>
        /// Терминал
        /// </summary>
        Terminal = 4
    }
}
