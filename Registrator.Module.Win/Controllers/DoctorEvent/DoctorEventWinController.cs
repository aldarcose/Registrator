using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using Registrator.Module.BusinessObjects;
using DevExpress.ExpressApp.Scheduler.Win;
using DevExpress.XtraScheduler;
using System.Drawing;
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Utils;

namespace Registrator.Module.Win.Controllers
{
    public class DoctorEventWinController : ObjectViewController<ObjectView, Registrator.Module.BusinessObjects.DoctorEvent>
    {
        private const string FILTERKEY = "FilterDoctors";
        private Dictionary<Guid, DoctorEvent> eventsDict = new Dictionary<Guid, DoctorEvent>();

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            ListView listView = View as ListView;
            DetailView detailView = View as DetailView;
            
            if (listView != null)
            {
                listView.CollectionSource.CriteriaApplied += (o, e) =>
                {
                    // Предварительная загрузка расписаний докторов
                    var collectionSB = (CollectionSourceBase)o;
                    if (collectionSB.Criteria != null)
                    {
                        var events = new List<DoctorEvent>(ObjectSpace.GetObjects<DoctorEvent>(collectionSB.Criteria[FILTERKEY]));
                        eventsDict = events.ToDictionary(de => de.Oid, de => de);
                    }
                };

                SchedulerListEditor listEditor = ((ListView)View).Editor as SchedulerListEditor;
                if (listEditor != null)
                {
                    SchedulerControl scheduler = listEditor.SchedulerControl;
                    if (scheduler != null)
                    {
                        scheduler.InitAppointmentDisplayText += (o, e) =>
                        {
                            Registrator.Module.BusinessObjects.DoctorEvent doctorEvent =
                                ObjectSpace.GetObjectByKey<Registrator.Module.BusinessObjects.DoctorEvent>(e.Appointment.Id);
                            e.Text = doctorEvent != null && doctorEvent.Pacient != null ? doctorEvent.Pacient.FullName : string.Empty;
                        };

                        // https://documentation.devexpress.com/WindowsForms/118551/Controls-and-Libraries/Scheduler/Visual-Elements/Scheduler-Control/Appointment-Flyout
                        scheduler.OptionsView.ToolTipVisibility = ToolTipVisibility.Always;
                        scheduler.OptionsCustomization.AllowDisplayAppointmentFlyout = false;
                        scheduler.ToolTipController = new ToolTipController();
                        scheduler.ToolTipController.ToolTipType = ToolTipType.SuperTip;
                        scheduler.ToolTipController.BeforeShow += (o, e) =>
                        {
                            var toolTipController = (ToolTipController)o;
                            AppointmentViewInfo aptViewInfo = null;
                            try
                            {
                                aptViewInfo = (AppointmentViewInfo)toolTipController.ActiveObject;
                            }
                            catch
                            {
                                return;
                            }

                            if (aptViewInfo == null) return;

                            Guid guid = (Guid)aptViewInfo.Appointment.Id;
                            if (!eventsDict.ContainsKey(guid))
                            {
                                // В словаре нет ключа т.к. расписание было только что создано
                                var events = new List<DoctorEvent>(ObjectSpace.GetObjects<DoctorEvent>(listView.CollectionSource.Criteria[FILTERKEY]));
                                eventsDict = events.ToDictionary(de => de.Oid, de => de);
                            }
                            DoctorEvent doctorEvent = eventsDict[guid];

                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(string.Format("Время: с {0:HH:mm} по {1:HH:mm}", doctorEvent.StartOn, doctorEvent.EndOn));
                            sb.AppendLine(string.Format("Пациент: {0}", doctorEvent.Pacient != null ? doctorEvent.Pacient.FullName : null));
                            sb.AppendLine(string.Format("Кем создано: {0}", doctorEvent.CreatedBy != null ? doctorEvent.CreatedBy.FullName : null));

                            SuperToolTip SuperTip = new SuperToolTip();
                            SuperToolTipSetupArgs args = new SuperToolTipSetupArgs();
                            //args.Title.Text = "Info";
                            //args.Title.Font = new Font("Times New Roman", 10);
                            args.Contents.Text = sb.ToString();
                            args.Contents.Font = new Font("Times New Roman", 11);
                            // args.Contents.Image = resImage;
                            // args.ShowFooterSeparator = true;
                            // args.Footer.Font = new Font("Comic Sans MS", 8);
                            // args.Footer.Text = "SuperTip";
                            SuperTip.Setup(args);
                            e.SuperTip = SuperTip;
                        };

                        var storage = scheduler.Storage;
                        IAppointmentLabelStorage labelStorage = storage.Appointments.Labels;
                        FillLabelStorage(labelStorage);
                    }
                }
            }
            else if (detailView != null)
            {
                foreach (SchedulerLabelPropertyEditor pe in ((DetailView)View).GetItems<SchedulerLabelPropertyEditor>())
                    if (pe.Control != null)
                    {
                        ISchedulerStorage storage = ((AppointmentLabelEdit)pe.Control).Storage;
                        IAppointmentLabelStorage labelStorage = storage.Appointments.Labels;
                        FillLabelStorage(labelStorage);
                    }
            }
        }

        private void FillLabelStorage(IAppointmentLabelStorage labelStorage)
        {
            labelStorage.Clear();
            int i = 1;
            using (IObjectSpace os = Application.CreateObjectSpace())
            {
                IAppointmentLabel label = labelStorage.CreateNewLabel(i, "Нет", "Нет");
                label.SetColor(Color.White);
                labelStorage.Add(label);
                i++;

                IList<DoctorEventLabel> labels = os.GetObjects<DoctorEventLabel>();
                foreach (var doctorEventLabel in labels)
                {
                    label = labelStorage.CreateNewLabel(i, doctorEventLabel.Name, doctorEventLabel.Name);
                    label.SetColor(doctorEventLabel.Color);
                    labelStorage.Add(label);
                    i++;
                }
            }
        }
    }
}
