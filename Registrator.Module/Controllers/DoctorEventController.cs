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
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Persistent.AuditTrail;
using DevExpress.XtraScheduler;
using DevExpress.XtraEditors;
using Registrator.Module.BusinessObjects.Dictionaries;
using System.Drawing;
using DevExpress.ExpressApp.Editors;

namespace Registrator.Module.Controllers
{
    /// <summary>
    /// Контроллер создания расписания врача
    /// </summary>
    public partial class DoctorEventController : ViewController
    {
        private const string FILTERKEY = "FilterDoctors";
        private const string CREATEEVENTENABLE = "CreateEventEnabled";
        private const string CLONEDOCTOREVENTACTIVE = "CloneDoctorEventActive";
        private SchedulerViewBase activeView;
        static ListView rootListView = null;

        public DoctorEventController()
        {
            InitializeComponent();
        }

        private void DoctorEventController_ViewControlsCreated(object sender, EventArgs e)
        {
            PanelControl panelControl = View.Control as PanelControl;
            if (panelControl != null && panelControl.Controls.Count > 0)
            {
                SchedulerControl scheduler = panelControl.Controls[0] as SchedulerControl;
                activeView = scheduler.ActiveView;
                scheduler.ActiveViewChanged += mainControl_ActiveViewChanged;

                SetDefaultInterval();
            }
        }

        // Установка текущей недели
        private void SetDefaultInterval()
        {
            TimeIntervalCollection intervals = new TimeIntervalCollection();
            DateTime today = DateTime.Today;
            int dayOfWeek = (int)today.DayOfWeek;
            DateTime currentMonday = today.DayOfWeek == DayOfWeek.Sunday ? today.AddDays(-6) : today.AddDays(1 - dayOfWeek);
            intervals.SetContent(new TimeInterval(currentMonday, TimeSpan.FromDays(5)));
            activeView.SetVisibleIntervals(intervals);
        }

        private void mainControl_ActiveViewChanged(object sender, EventArgs e)
        {
            SchedulerControl mainControl = sender as SchedulerControl;
            if (mainControl != null)
                SetCloneDoctorEventActive(mainControl.ActiveViewType);
        }

        protected override void OnActivated()
        {
            ObjectSpace.CustomRefresh += ObjectSpace_CustomRefresh;

            rootListView = View as ListView;
            // Пустое представление при запуске
            ((ListView)View).CollectionSource.Criteria[FILTERKEY] = CriteriaOperator.Parse("1=0");
            
            SetCreateEventEnable();
            SetCloneDoctorEventActive(SchedulerViewType.Day);

            foreach (var doctorSpec in ObjectSpace.GetObjects<DoctorSpecTree>(DoctorSpecTree.Fields.Scheduling))
                FilterDoctorSpecEventAction.Items.Add(new ChoiceActionItem(doctorSpec.Oid.ToString(), doctorSpec.Name, doctorSpec));
            base.OnActivated();
        }

        // При обновлении представления возникало исключение StackOverflow, поэтому оно отключено
        private void ObjectSpace_CustomRefresh(object sender, HandledEventArgs e)
        {
            e.Handled = true;
        }

        #region Фильтрация расписания

        private void FilterDoctorSpecEventAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            DoctorSpecTree doctorSpec = (DoctorSpecTree)e.SelectedChoiceActionItem.Data;
            FilterDoctorEventAction.Items.Clear();
            foreach (var doctor in ObjectSpace.GetObjects<Doctor>(Doctor.Fields.Scheduling & Doctor.Fields.SpecialityTree == doctorSpec))
            {
                FilterDoctorEventAction.Items.Add(new ChoiceActionItem(doctor.Oid.ToString(), doctor.FullName, doctor));
            }
        }

        private void FilterDoctorEventAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            Doctor doctor = (Doctor)e.SelectedChoiceActionItem.Data;
            if (doctor != null)
            {
                CriteriaOperator criteria = DoctorEvent.Fields.AssignedTo.Oid == doctor.Oid;
                ((ListView)View).CollectionSource.BeginUpdateCriteria();
                ((ListView)View).CollectionSource.Criteria.Clear();
                ((ListView)View).CollectionSource.Criteria[FILTERKEY] = criteria;
                ((ListView)View).CollectionSource.EndUpdateCriteria();
                SetCreateEventEnable();
                SetCloneDoctorEventActive(activeView.Type);
            }
        }

        #endregion

        private void SetCreateEventEnable()
        {
            CreateDoctorEventAction.Enabled[CREATEEVENTENABLE] = FilterDoctorEventAction.SelectedItem != null;
        }

        private void SetCloneDoctorEventActive(SchedulerViewType type)
        {
            CloneDoctorEventAction.Active[CLONEDOCTOREVENTACTIVE] = FilterDoctorEventAction.SelectedItem != null &&
                type == SchedulerViewType.WorkWeek;
        }

        #region Создание расписания

        private void CreateDoctorEventAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            TimeIntervalCollection intervalCol = activeView.GetVisibleIntervals();
            CreateDoctorEventParameters parameters = new CreateDoctorEventParameters(intervalCol.Start);
            e.View = Application.CreateDetailView(objectSpace, parameters, true);
        }

        private void CreateDoctorEventAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            Doctor doctor = FilterDoctorEventAction.SelectedItem.Data as Doctor;
            // Форма параметров
            CreateDoctorEventParameters parameters = (CreateDoctorEventParameters)e.PopupWindowViewCurrentObject;
            using (IObjectSpace os = Application.CreateObjectSpace())
            {
                DateTime start = parameters.DateIn;
                DateTime end = parameters.DateIn.AddDays(7 * parameters.WeeksCount);
                
                while (start < end)
                {
                    if ((start.DayOfWeek == DayOfWeek.Monday && parameters.Monday) ||
                        (start.DayOfWeek == DayOfWeek.Tuesday && parameters.Tuesday) ||
                        (start.DayOfWeek == DayOfWeek.Wednesday && parameters.Wednesday) ||
                        (start.DayOfWeek == DayOfWeek.Thursday && parameters.Thursday) ||
                        (start.DayOfWeek == DayOfWeek.Friday && parameters.Friday) ||
                        (start.DayOfWeek == DayOfWeek.Saturday && parameters.Saturday) ||
                        (start.DayOfWeek == DayOfWeek.Sunday && parameters.Sunday))
                    {
                        DateTime timeStart = start.AddHours(parameters.TimeIn.Hour).AddMinutes(parameters.TimeIn.Minute);
                        DateTime timeEnd = start.AddHours(parameters.TimeOut.Hour).AddMinutes(parameters.TimeOut.Minute);

                        List<DoctorEvent> oldEvents = new List<DoctorEvent>();
                        GroupOperator oldEventsCriteria = new GroupOperator();
                        // Удаляем пересекающееся старое расписание
                        if (timeStart == timeStart.Date && timeEnd == timeEnd.Date)
                        {
                            // Если указано пустое расписание, то удаляем старое расписание полностью за день
                            oldEventsCriteria.Operands.Add(DoctorEvent.Fields.AssignedTo == os.GetObject(doctor) &
                                new FunctionOperator(FunctionOperatorType.GetDate, DoctorEvent.Fields.StartOn) >= timeStart.Date &
                                new FunctionOperator(FunctionOperatorType.GetDate, DoctorEvent.Fields.EndOn) < timeStart.Date.AddDays(1));
                        }
                        else
                        {
                            // Ищем пересекающиеся с указанным временем расписания
                            oldEventsCriteria.Operands.Add(DoctorEvent.Fields.AssignedTo == os.GetObject(doctor) & (
                                DoctorEvent.Fields.StartOn < timeStart & DoctorEvent.Fields.EndOn > timeStart |
                                DoctorEvent.Fields.StartOn >= timeStart & DoctorEvent.Fields.EndOn <= timeEnd |
                                DoctorEvent.Fields.StartOn < timeEnd & DoctorEvent.Fields.EndOn > timeEnd));
                        }
                        
                        oldEvents.AddRange(os.GetObjects<DoctorEvent>(oldEventsCriteria));
                        os.Delete(oldEvents);

                        // Цикл по времени
                        while (timeStart < timeEnd)
                        {
                            var doctorEvent = os.CreateObject<DoctorEvent>();
                            doctorEvent.AssignedTo = os.GetObject(doctor);
                            doctorEvent.StartOn = timeStart;
                            doctorEvent.EndOn = timeStart.Add(parameters.Norm);
                            doctorEvent.Label = parameters.Label + 1;
                            timeStart = timeStart.Add(parameters.Norm);
                        }
                    }
                    start = start.AddDays(1);
                }

                // Отключение аудита
                Session session = ((XPObjectSpace)os).Session;
                AuditTrailService.Instance.EndSessionAudit(session);
                os.CommitChanges();
            }

            // Обновление списочного представления
            if (rootListView != null && rootListView.ObjectSpace != null && rootListView.CollectionSource != null)
            {
                rootListView.CollectionSource.Reload();
            }
        }

        #endregion

        #region Клонирование расписания

        private void CloneDoctorEventAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            CloneDoctorEventParameters parameters = new CloneDoctorEventParameters();
            e.View = Application.CreateDetailView(ObjectSpaceInMemory.CreateNew(), parameters, true);
        }

        private void CloneDoctorEventAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            Doctor doctor = FilterDoctorEventAction.SelectedItem.Data as Doctor;
            // Форма параметров
            CloneDoctorEventParameters parameters = (CloneDoctorEventParameters)e.PopupWindowViewCurrentObject;
            using (IObjectSpace os = Application.CreateObjectSpace())
            {
                TimeIntervalCollection intervalCol = activeView.GetVisibleIntervals();
                DateTime clonedWeekDateIn = intervalCol.Start;
                Dictionary<int, List<DoctorEvent>> clonedWeek = new Dictionary<int, List<DoctorEvent>>();
                DateTime start = clonedWeekDateIn; int i = 1;
                // Копируем в буфер неделю, которую мы хотим скопировать
                while (start < clonedWeekDateIn.AddDays(7))
                {
                    clonedWeek[i] = new List<DoctorEvent>(os.GetObjects<DoctorEvent>(
                        DoctorEvent.Fields.AssignedTo == os.GetObject(doctor) &
                        DoctorEvent.Fields.StartOn >= start.Date & 
                        DoctorEvent.Fields.EndOn < start.Date.AddDays(1)));
                    start = start.AddDays(1);
                    i++;
                }

                int weekIndex = 0; start = clonedWeekDateIn.Date.AddDays(7);
                while (weekIndex < parameters.NextWeeksCount)
                {
                    int dayIndex = 1;
                    while (dayIndex <= 7)
                    {
                        foreach (DoctorEvent clonedEvent in clonedWeek[dayIndex])
                        {
                            var newEvent = os.CreateObject<DoctorEvent>();
                            newEvent.AssignedTo = clonedEvent.AssignedTo;
                            newEvent.Label = clonedEvent.Label;
                            newEvent.StartOn = start.AddHours(clonedEvent.StartOn.Hour).AddMinutes(clonedEvent.StartOn.Minute);
                            newEvent.EndOn = start.AddHours(clonedEvent.EndOn.Hour).AddMinutes(clonedEvent.EndOn.Minute);
                        }
                        dayIndex++;
                        start = start.AddDays(1);
                    }
                    weekIndex++;
                }

                // Отключение аудита
                Session session = ((XPObjectSpace)os).Session;
                AuditTrailService.Instance.EndSessionAudit(session);
                os.CommitChanges();
            }

            // Обновление списочного представления
            if (rootListView != null && rootListView.ObjectSpace != null && rootListView.CollectionSource != null)
            {
                rootListView.CollectionSource.Reload();
            }
        }

        #endregion
    }

    [DomainComponent]
    public class CreateDoctorEventParameters
    {
        public CreateDoctorEventParameters(DateTime dateIn)
        {
            this.Norm = TimeSpan.FromMinutes(15);
            this.DateIn = dateIn;
            DateTime today = DateTime.Today;
            this.TimeIn = today;
            this.TimeOut = today;
            this.WeeksCount = 1;
        }

        [ModelDefault("EditMask", "HH:mm")]
        [ModelDefault("DisplayFormat", "{0:HH:mm}")]
        public DateTime TimeIn { get; set; }

        [ModelDefault("EditMask", "HH:mm")]
        [ModelDefault("DisplayFormat", "{0:HH:mm}")]
        public DateTime TimeOut { get; set; }

        public TimeSpan Norm { get; set; }

        /// <summary>Начало расписания</summary>
        public DateTime DateIn { get; set; }

        /// <summary>Количество недель</summary>
        public uint WeeksCount { get; set; }

        [ModelDefault("PropertyEditorType", "DevExpress.ExpressApp.Scheduler.Win.SchedulerLabelPropertyEditor")]
        public int Label { get; set; }

        #region Дни недели

        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }

        #endregion
    }

    /// <summary>
    /// Параметры операции &qout;Клонирование расписания рабочей недели&qout;
    /// </summary>
    [DomainComponent]
    public class CloneDoctorEventParameters
    {
        /// <summary>
        /// Количество следующих недель на которые будут скопировано расписание
        /// </summary>
        public uint NextWeeksCount { get; set; }
    }
}
