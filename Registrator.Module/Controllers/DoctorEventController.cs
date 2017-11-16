using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using Registrator.Module.BusinessObjects;
using System;

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
            CreateDoctorEventParameters parameters = new CreateDoctorEventParameters();
            e.View = Application.CreateDetailView(objectSpace, parameters);
        }

        private void CreateDoctorEventAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            // Форма параметров
            CreateDoctorEventParameters parameters = (CreateDoctorEventParameters)e.PopupWindowViewCurrentObject;
            Doctor doctor = e.CurrentObject as Doctor;

            DateTime start = parameters.DateIn;
            TimeSpan norm = parameters.Norm;
            
            while (start < parameters.DateOut)
            {
                var doctorEvent = ObjectSpace.CreateObject<DoctorEvent>();
                doctorEvent.AssignedTo = doctor;
                doctorEvent.StartOn = start;
                doctorEvent.EndOn = start.Add(norm);
                start = start.Add(norm);
            }

            ObjectSpace.CommitChanges();

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
    public class CreateDoctorEventParameters
    {
        public CreateDoctorEventParameters()
        {
            this.DateIn = DateTime.Today;
            this.DateOut = DateTime.Today;
            this.Norm = TimeSpan.FromMinutes(15);
        }

        /// <summary>Дата с</summary>
        public DateTime DateIn { get; set; }

        /// <summary>Дата по</summary>
        public DateTime DateOut { get; set; }

        /// <summary>Норма</summary>
        /// <remarks>Длина приема</remarks>
        public TimeSpan Norm { get; set; }
    }
}
