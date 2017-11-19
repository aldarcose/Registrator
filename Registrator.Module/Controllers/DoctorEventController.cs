using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Registrator.Module.BusinessObjects;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace Registrator.Module.Controllers
{
    public partial class DoctorEventController : ViewController
    {
        public DoctorEventController()
        {
            InitializeComponent();
        }

        private void CreateDoctorEventAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            CreateDoctorEventParameters parameters = new CreateDoctorEventParameters(objectSpace, (Doctor)View.CurrentObject);
            e.View = Application.CreateDetailView(objectSpace, parameters, true);
        }

        private void CreateDoctorEventAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            Doctor doctor = e.CurrentObject as Doctor;
            // Форма параметров
            CreateDoctorEventParameters parameters = (CreateDoctorEventParameters)e.PopupWindowViewCurrentObject;
            using (IObjectSpace os = Application.CreateObjectSpace())
            {
                TimeSpan norm = parameters.Norm;
                Action<DateTime, DateTime> CreateEvent = (dateIn, dateOut) =>
                {
                    IList<DoctorEvent> oldEvents = new List<DoctorEvent>(parameters.Events.Where(de =>
                        de.StartOn.Date >= dateIn.Date && de.EndOn < dateOut.Date.AddDays(1)));
                    // Удаление старого расписания на дату
                    if (oldEvents.Count != 0) os.Delete(oldEvents);

                    DateTime start = dateIn;
                    while (start < dateOut)
                    {
                        var doctorEvent = os.CreateObject<DoctorEvent>();
                        doctorEvent.AssignedTo = os.GetObject(doctor);
                        doctorEvent.StartOn = start;
                        doctorEvent.EndOn = start.Add(norm);
                        start = start.Add(norm);
                    }
                };

                uint weeksInterval = parameters.WeeksInterval;

                DateTime mondayDateIn = parameters.MondayDateIn;
                DateTime mondayDateOut = parameters.MondayDateOut;
                DateTime tuesdayDateIn = parameters.TuesdayDateIn;
                DateTime tuesdayDateOut = parameters.TuesdayDateOut;
                DateTime wednesdayDateIn = parameters.WednesdayDateIn;
                DateTime wednesdayDateOut = parameters.ThursdayDateOut; 
                DateTime thursdayDateIn = parameters.ThursdayDateIn;
                DateTime thursdayDateOut = parameters.ThursdayDateOut;
                DateTime fridayDateIn = parameters.FridayDateIn;
                DateTime fridayDateOut = parameters.FridayDateOut;
                DateTime saturdayDateIn = parameters.SaturdayDateIn;
                DateTime saturdayDateOut = parameters.SaturdayDateOut; 
                DateTime sundayDateIn = parameters.SundayDateIn;
                DateTime sundayDateOut = parameters.SundayDateOut;

                // Создание расписания
                int i = 1;
                while (i <= parameters.WeeksCount)
                {
                    CreateEvent(mondayDateIn, mondayDateOut);
                    CreateEvent(tuesdayDateIn, tuesdayDateOut);
                    CreateEvent(wednesdayDateIn, wednesdayDateOut);
                    CreateEvent(thursdayDateIn, thursdayDateOut);
                    CreateEvent(fridayDateIn, fridayDateOut);
                    CreateEvent(saturdayDateIn, saturdayDateOut);
                    CreateEvent(sundayDateIn, sundayDateOut);

                    mondayDateIn = mondayDateIn.AddDays(7 * (weeksInterval + 1)); mondayDateOut = mondayDateOut.AddDays(7 * (weeksInterval + 1));
                    tuesdayDateIn = tuesdayDateIn.AddDays(7 * (weeksInterval + 1)); tuesdayDateOut = tuesdayDateOut.AddDays(7 * (weeksInterval + 1));
                    wednesdayDateIn = wednesdayDateIn.AddDays(7 * (weeksInterval + 1)); wednesdayDateOut = wednesdayDateOut.AddDays(7 * (weeksInterval + 1));
                    thursdayDateIn = thursdayDateIn.AddDays(7 * (weeksInterval + 1)); thursdayDateOut = thursdayDateOut.AddDays(7 * (weeksInterval + 1));
                    fridayDateIn = fridayDateIn.AddDays(7 * (weeksInterval + 1)); fridayDateOut = fridayDateOut.AddDays(7 * (weeksInterval + 1));
                    saturdayDateIn = saturdayDateIn.AddDays(7 * (weeksInterval + 1)); saturdayDateOut = saturdayDateOut.AddDays(7 * (weeksInterval + 1));
                    sundayDateIn = sundayDateIn.AddDays(7 * (weeksInterval + 1)); sundayDateOut = sundayDateOut.AddDays(7 * (weeksInterval + 1));
                    i++;
                }
                os.CommitChanges();
            }

            // Открытие списочного представления
            OpenDoctorEventListView(doctor, e.ShowViewParameters);
        }

        private void OpenDoctorEventListView(Doctor doctor, ShowViewParameters showViewParams)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            string doctorEventListViewId = Application.FindListViewId(typeof(DoctorEvent));
            var listView = Application.CreateListView(typeof(DoctorEvent), true);
            var collection = Application.CreateCollectionSource(os,
                typeof(DoctorEvent), doctorEventListViewId,
                CollectionSourceDataAccessMode.DataView, CollectionSourceMode.Normal);
            var view = Application.CreateListView(listView.Model, collection, true);
            view.Model.Filter = CriteriaOperator.Parse("Doctor = ?", doctor.Oid).LegacyToString();
            showViewParams.CreatedView = view;
            showViewParams.TargetWindow = TargetWindow.Default;
            showViewParams.CreateAllControllers = true;
        }
    }

    /// <summary>
    /// Параметры операции Создать расписание для врача
    /// </summary>
    [DomainComponent]
    public class CreateDoctorEventParameters : INotifyPropertyChanged
    {
        private DateTime mondayDateIn, tuesdayDateIn, wednesdayDateIn, thursdayDateIn, fridayDateIn, saturdayDateIn, sundayDateIn;
        private IObjectSpace objectSpace;
        private Doctor doctor;
        private List<DoctorEvent> events;

        public CreateDoctorEventParameters(IObjectSpace objectSpace, Doctor doctor)
        {
            this.objectSpace = objectSpace;
            this.doctor = doctor;
            this.Norm = TimeSpan.FromMinutes(15);
            // Загрузка расписаний доктора
            this.events = new List<DoctorEvent>(objectSpace.GetObjects<DoctorEvent>(
                DoctorEvent.Fields.AssignedTo == objectSpace.GetObject(doctor)));

            DateTime today = DateTime.Today;
            int dayOfWeek = (int)today.DayOfWeek;
            this.MondayDateIn = today.AddDays(1 - dayOfWeek);
            this.MondayDateOut = today.AddDays(1 - dayOfWeek);
            this.TuesdayDateIn = today.AddDays(2 - dayOfWeek);
            this.TuesdayDateOut = today.AddDays(2 - dayOfWeek);
            this.WednesdayDateIn = today.AddDays(3 - dayOfWeek);
            this.WednesdayDateOut = today.AddDays(3 - dayOfWeek);
            this.ThursdayDateIn = today.AddDays(4 - dayOfWeek);
            this.ThursdayDateOut = today.AddDays(4 - dayOfWeek);
            this.FridayDateIn = today.AddDays(5 - dayOfWeek);
            this.FridayDateOut = today.AddDays(5 - dayOfWeek);
            this.SaturdayDateIn = today.AddDays(6 - dayOfWeek);
            this.SaturdayDateOut = today.AddDays(6 - dayOfWeek);
            this.SundayDateIn = today.AddDays(7 - dayOfWeek);
            this.SundayDateOut = today.AddDays(7 - dayOfWeek);

            LoadDayEventPeriod(MondayDateIn);
            LoadDayEventPeriod(TuesdayDateIn);
            LoadDayEventPeriod(WednesdayDateIn);
            LoadDayEventPeriod(ThursdayDateIn);
            LoadDayEventPeriod(FridayDateIn);
            LoadDayEventPeriod(SaturdayDateIn);
            LoadDayEventPeriod(SundayDateIn);
        }

        #region Понедельник

        /// <summary>
        /// Дата начала приема в понедельник
        /// </summary>
        [ModelDefault("EditMask", "HH:mm")]
        [ModelDefault("DisplayFormat", "{0:HH:mm}")]
        [ImmediatePostData]
        public DateTime MondayDateIn 
        { 
            get { return mondayDateIn; }
            set 
            {
                mondayDateIn = value;
                OnChanged("Monday");
            }
        }

        /// <summary>
        /// Дата окончания приема в понедельник
        /// </summary>
        [ModelDefault("EditMask", "HH:mm")]
        [ModelDefault("DisplayFormat", "{0:HH:mm}")]
        public DateTime MondayDateOut { get; set; }

        /// <summary>
        /// Дата в понельник
        /// </summary>
        [ModelDefault("DisplayFormat", "{0:ddd MMM dd yyyy}")]
        [ModelDefault("AllowEdit", "False")]
        public DateTime Monday { get { return mondayDateIn; } }

        #endregion

        #region Вторник
        /// <summary>
        /// Дата начала приема во вторник
        /// </summary>
        [ModelDefault("EditMask", "HH:mm")]
        [ModelDefault("DisplayFormat", "{0:HH:mm}")]
        public DateTime TuesdayDateIn 
        { 
            get { return tuesdayDateIn; }
            set
            {
                tuesdayDateIn = value;
                OnChanged("Tuesday");
            }
        }

        /// <summary>
        /// Дата окончания приема во вторник
        /// </summary>
        [ModelDefault("EditMask", "HH:mm")]
        [ModelDefault("DisplayFormat", "{0:HH:mm}")]
        public DateTime TuesdayDateOut { get; set; }

        /// <summary>
        /// Дата во вторник
        /// </summary>
        [ModelDefault("DisplayFormat", "{0:ddd MMM dd yyyy}")]
        public DateTime Tuesday
        {
            get { return tuesdayDateIn; }
        }

        #endregion

        #region Среда

        /// <summary>
        /// Дата начала приема в среду
        /// </summary>
        [ModelDefault("EditMask", "HH:mm")]
        [ModelDefault("DisplayFormat", "{0:HH:mm}")]
        public DateTime WednesdayDateIn
        {
            get { return wednesdayDateIn; }
            set
            {
                wednesdayDateIn = value;
                OnChanged("Wednesday");
            }
        }

        /// <summary>
        /// Дата окончания приема в среду
        /// </summary>
        [ModelDefault("EditMask", "HH:mm")]
        [ModelDefault("DisplayFormat", "{0:HH:mm}")]
        public DateTime WednesdayDateOut { get; set; }

        /// <summary>
        /// Дата в среду
        /// </summary>
        [ModelDefault("DisplayFormat", "{0:ddd MMM dd yyyy}")]
        public DateTime Wednesday
        {
            get { return wednesdayDateIn; }
        }

        #endregion

        #region Четверг

        /// <summary>
        /// Дата начала приема в черверг
        /// </summary>
        [ModelDefault("EditMask", "HH:mm")]
        [ModelDefault("DisplayFormat", "{0:HH:mm}")]
        public DateTime ThursdayDateIn
        {
            get { return thursdayDateIn; }
            set
            {
                thursdayDateIn = value;
                OnChanged("ThursdayDateIn");
            }
        }

        /// <summary>
        /// Дата окончания приема в четверг
        /// </summary>
        [ModelDefault("EditMask", "HH:mm")]
        [ModelDefault("DisplayFormat", "{0:HH:mm}")]
        public DateTime ThursdayDateOut { get; set; }

        /// <summary>
        /// Дата в четверг
        /// </summary>
        [ModelDefault("DisplayFormat", "{0:ddd MMM dd yyyy}")]
        public DateTime Thursday
        {
            get { return thursdayDateIn; }
        }

        #endregion

        #region Пятница

        /// <summary>
        /// Дата начала приема в пятницу
        /// </summary>
        [ModelDefault("EditMask", "HH:mm")]
        [ModelDefault("DisplayFormat", "{0:HH:mm}")]
        public DateTime FridayDateIn
        {
            get { return fridayDateIn; }
            set
            {
                fridayDateIn = value;
                OnChanged("FridayDateIn");
            }
        }

        /// <summary>
        /// Дата окончания приема в пятницу
        /// </summary>
        [ModelDefault("EditMask", "HH:mm")]
        [ModelDefault("DisplayFormat", "{0:HH:mm}")]
        public DateTime FridayDateOut { get; set; }

        /// <summary>
        /// Дата в пятницу
        /// </summary>
        [ModelDefault("DisplayFormat", "{0:ddd MMM dd yyyy}")]
        public DateTime Friday
        {
            get { return fridayDateIn; }
        }

        #endregion

        #region Суббота

        /// <summary>
        /// Дата начала приема в субботу
        /// </summary>
        [ModelDefault("EditMask", "HH:mm")]
        [ModelDefault("DisplayFormat", "{0:HH:mm}")]
        public DateTime SaturdayDateIn
        {
            get { return saturdayDateIn; }
            set
            {
                saturdayDateIn = value;
                OnChanged("SaturdayDateIn");
            }
        }

        /// <summary>
        /// Дата окончания приема в субботу
        /// </summary>
        [ModelDefault("EditMask", "HH:mm")]
        [ModelDefault("DisplayFormat", "{0:HH:mm}")]
        public DateTime SaturdayDateOut { get; set; }

        /// <summary>
        /// Дата в субботу
        /// </summary>
        [ModelDefault("DisplayFormat", "{0:ddd MMM dd yyyy}")]
        [ModelDefault("AllowEdit", "False")]
        public DateTime Saturday
        {
            get { return saturdayDateIn; }
        }

        #endregion

        #region Воскресенье

        /// <summary>
        /// Дата начала приема в воскресенье
        /// </summary>
        [ModelDefault("EditMask", "HH:mm")]
        [ModelDefault("DisplayFormat", "{0:HH:mm}")]
        public DateTime SundayDateIn
        {
            get { return sundayDateIn; }
            set
            {
                sundayDateIn = value;
                OnChanged("SundayDateIn");
            }   
        }

        /// <summary>
        /// Дата окончания приема в воскресенье
        /// </summary>
        [ModelDefault("EditMask", "HH:mm")]
        [ModelDefault("DisplayFormat", "{0:HH:mm}")]
        public DateTime SundayDateOut { get; set; }

        /// <summary>
        /// Дата в воскресенье
        /// </summary>
        [ModelDefault("DisplayFormat", "{0:ddd MMM dd yyyy}")]
        public DateTime Sunday
        {
            get { return sundayDateIn; }
        }

        #endregion

        /// <summary>Норма</summary>
        /// <remarks>Длина приема</remarks>
        public TimeSpan Norm { get; set; }

        /// <summary>Количество недель</summary>
        public uint WeeksCount { get; set; }

        /// <summary>Промежуток между неделями</summary>
        public uint WeeksInterval { get; set; }

        /// <summary>Расписание приема доктора</summary>
        [Browsable(false)]
        public List<DoctorEvent> Events
        {
            get { return events; }
        }

        /// <summary>
        /// Перейти на предыдущую неделю
        /// </summary>
        [Action(PredefinedCategory.PopupActions, Caption = "<< Предыдущая неделя")]
        public void PreviousWeek()
        {
            MondayDateIn = MondayDateIn.AddDays(-7);
            MondayDateOut = MondayDateOut.AddDays(-7);
            TuesdayDateIn = TuesdayDateIn.AddDays(-7);
            TuesdayDateOut = TuesdayDateOut.AddDays(-7);
            WednesdayDateIn = WednesdayDateIn.AddDays(-7);
            WednesdayDateOut = WednesdayDateOut.AddDays(-7);
            ThursdayDateIn = ThursdayDateIn.AddDays(-7);
            ThursdayDateOut = ThursdayDateOut.AddDays(-7);
            FridayDateIn = FridayDateIn.AddDays(-7);
            FridayDateOut = FridayDateOut.AddDays(-7);
            SaturdayDateIn = SaturdayDateIn.AddDays(-7);
            SaturdayDateOut = SaturdayDateOut.AddDays(-7);
            SundayDateIn = SundayDateIn.AddDays(-7);
            SundayDateOut = SundayDateOut.AddDays(-7);

            LoadDayEventPeriod(MondayDateIn);
            LoadDayEventPeriod(TuesdayDateIn);
            LoadDayEventPeriod(WednesdayDateIn);
            LoadDayEventPeriod(ThursdayDateIn);
            LoadDayEventPeriod(FridayDateIn);
            LoadDayEventPeriod(SaturdayDateIn);
            LoadDayEventPeriod(SundayDateIn);
        }

        /// <summary>
        /// Перейти на следующую неделю
        /// </summary>
        [Action(PredefinedCategory.PopupActions, Caption = "Следующая неделя >>")]
        public void NextWeek()
        {
            MondayDateIn = MondayDateIn.AddDays(7);
            MondayDateOut = MondayDateOut.AddDays(7);
            TuesdayDateIn = TuesdayDateIn.AddDays(7);
            TuesdayDateOut = TuesdayDateOut.AddDays(7);
            WednesdayDateIn = WednesdayDateIn.AddDays(7);
            WednesdayDateOut = WednesdayDateOut.AddDays(7);
            ThursdayDateIn = ThursdayDateIn.AddDays(7);
            ThursdayDateOut = ThursdayDateOut.AddDays(7);
            FridayDateIn = FridayDateIn.AddDays(7);
            FridayDateOut = FridayDateOut.AddDays(7);
            SaturdayDateIn = SaturdayDateIn.AddDays(7);
            SaturdayDateOut = SaturdayDateOut.AddDays(7);
            SundayDateIn = SundayDateIn.AddDays(7);
            SundayDateOut = SundayDateOut.AddDays(7);

            LoadDayEventPeriod(MondayDateIn);
            LoadDayEventPeriod(TuesdayDateIn);
            LoadDayEventPeriod(WednesdayDateIn);
            LoadDayEventPeriod(ThursdayDateIn);
            LoadDayEventPeriod(FridayDateIn);
            LoadDayEventPeriod(SaturdayDateIn);
            LoadDayEventPeriod(SundayDateIn);
        }

        private void LoadDayEventPeriod(DateTime date)
        {
            IEnumerable<DoctorEvent> dateEvents = events.Where(e => e.StartOn >= date && e.EndOn < date.AddDays(1));
            bool eventsAny = dateEvents.Any();
            var dayOfWeek = date.DayOfWeek;
            switch(dayOfWeek)
            {
                case DayOfWeek.Monday :
                    this.MondayDateIn = eventsAny ? dateEvents.Min(e => e.StartOn) : MondayDateIn.Date;
                    this.MondayDateOut = eventsAny ? dateEvents.Max(e => e.EndOn) : MondayDateIn.Date;
                    break;
                case DayOfWeek.Tuesday:
                    this.TuesdayDateIn = eventsAny ? dateEvents.Min(e => e.StartOn) : TuesdayDateIn.Date;
                    this.TuesdayDateOut = eventsAny ? dateEvents.Max(e => e.EndOn) : TuesdayDateIn.Date;
                    break;
                case DayOfWeek.Wednesday:
                    this.WednesdayDateIn = eventsAny ? dateEvents.Min(e => e.StartOn) : WednesdayDateIn.Date;
                    this.WednesdayDateOut = eventsAny ? dateEvents.Max(e => e.EndOn) : WednesdayDateIn.Date;
                    break;
                case DayOfWeek.Thursday:
                    this.ThursdayDateIn = eventsAny ? dateEvents.Min(e => e.StartOn) : ThursdayDateIn.Date;
                    this.ThursdayDateOut = eventsAny ? dateEvents.Max(e => e.EndOn) : ThursdayDateIn.Date;
                    break;
                case DayOfWeek.Friday:
                    this.FridayDateIn = eventsAny ? dateEvents.Min(e => e.StartOn) : FridayDateIn.Date;
                    this.FridayDateOut = eventsAny ? dateEvents.Max(e => e.EndOn) : FridayDateIn.Date;
                    break;
                case DayOfWeek.Saturday:
                    this.SaturdayDateIn = eventsAny ? dateEvents.Min(e => e.StartOn) : SaturdayDateIn.Date;
                    this.SaturdayDateOut = eventsAny ? dateEvents.Max(e => e.EndOn) : SaturdayDateIn.Date;
                    break;
                case DayOfWeek.Sunday:
                    this.SundayDateIn = eventsAny ? dateEvents.Min(e => e.StartOn) : SundayDateIn.Date;
                    this.SundayDateOut = eventsAny ? dateEvents.Max(e => e.EndOn) : SundayDateOut.Date;
                    break;
            }

            OnChanged("MondayDateOut");
            OnChanged("TuesdayDateOut");
            OnChanged("WednesdayDateOut");
            OnChanged("ThursdayDateOut");
            OnChanged("FridayDateOut");
            OnChanged("SaturdayDateOut");
            OnChanged("SundayDateOut");
        }

        #region INotifyPropertyChanged
        /// <summary>
        /// Вызывает событие изменения указанного свойства
        /// </summary>
        /// <param name="propertyName">Название свойства, которое было изменено</param>
        protected void OnChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <contentfrom cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
