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

namespace Registrator.Module.Controllers
{
    /// <summary>
    /// Контроллер создания расписания врача
    /// </summary>
    public partial class DoctorEventController : ViewController
    {
        private const string FILTERKEY = "FilterDoctors";
        private const string CREATEEVENTENABLE = "CreateEventEnabled";
        static ListView rootListView = null;

        public DoctorEventController()
        {
            InitializeComponent();
        }

        protected override void OnActivated()
        {
            ((ListView)View).CollectionSource.Criteria[FILTERKEY] = CriteriaOperator.Parse("1=0");
            SetCreateEventEnable();
            foreach (var doctor in ObjectSpace.GetObjects<Doctor>(Doctor.Fields.Scheduling))
                FilterDoctorEventAction.Items.Add(new ChoiceActionItem(doctor.Oid.ToString(), doctor.FullName, doctor));
            rootListView = View as ListView;
            base.OnActivated();
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
            CreateDoctorEventParameters parameters = new CreateDoctorEventParameters();
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
                // Удаляем старое расписание доктора за период
                List<DoctorEvent> oldEvents = new List<DoctorEvent>(os.GetObjects<DoctorEvent>(
                    DoctorEvent.Fields.AssignedTo == os.GetObject(doctor) &
                    new FunctionOperator(FunctionOperatorType.GetDate, DoctorEvent.Fields.StartOn) >= start.Date &
                    new FunctionOperator(FunctionOperatorType.GetDate, DoctorEvent.Fields.EndOn) < end.Date));
                os.Delete(oldEvents);

                // Цикл по дням
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
        public CreateDoctorEventParameters()
        {
            this.Norm = TimeSpan.FromMinutes(15);
            DateTime today = DateTime.Today;
            int dayOfWeek = (int)today.DayOfWeek;
            this.DateIn = today.AddDays(1 - dayOfWeek);
            this.TimeIn = today;
            this.TimeOut = today;
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
