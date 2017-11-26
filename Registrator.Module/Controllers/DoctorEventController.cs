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

namespace Registrator.Module.Controllers
{
    /// <summary>
    /// Контроллер создания расписания врача
    /// </summary>
    public partial class DoctorEventController : ViewController
    {
        private const string FILTERKEY = "FilterDoctors";
        private const string CREATEEVENTENABLE = "CreateEventEnabled";
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
                var mainControl = panelControl.Controls[0] as SchedulerControl;
                activeView = mainControl.ActiveView;
            }
        }

        protected override void OnActivated()
        {
            rootListView = View as ListView;
            // Пустое представление при запуске
            ((ListView)View).CollectionSource.Criteria[FILTERKEY] = CriteriaOperator.Parse("1=0");
            SetCreateEventEnable();
            foreach (var doctorSpec in ObjectSpace.GetObjects<DoctorSpecTree>(DoctorSpecTree.Fields.Scheduling))
                FilterDoctorSpecEventAction.Items.Add(new ChoiceActionItem(doctorSpec.Oid.ToString(), doctorSpec.Name, doctorSpec));
            base.OnActivated();
        }

        private void FilterDoctorSpecEventAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            DoctorSpecTree doctorSpec = (DoctorSpecTree)e.SelectedChoiceActionItem.Data;
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
                ((ListView)View).CollectionSource.Criteria[FILTERKEY] = DoctorEvent.Fields.AssignedTo == doctor;
                SetCreateEventEnable();
            }
        }

        private void SetCreateEventEnable()
        {
            CreateDoctorEventAction.Enabled[CREATEEVENTENABLE] = FilterDoctorEventAction.SelectedItem != null;
        }

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
                            doctorEvent.Label = parameters.Label;
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
}
