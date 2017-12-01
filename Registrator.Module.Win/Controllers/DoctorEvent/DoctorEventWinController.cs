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
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            ListView listView = View as ListView;
            DetailView detailView = View as DetailView;
            
            if (listView != null)
            {
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

                        //ToolTipController toolTipController = new ToolTipController();
                        //toolTipController.ShowBeak = true;
                        //toolTipController.ToolTipType = ToolTipType.Standard;
                        //scheduler.OptionsView.ToolTipVisibility = ToolTipVisibility.Standard;
                        //scheduler.ToolTipController = toolTipController;

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
