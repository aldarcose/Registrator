using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Scheduler.Win;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.UI;
using Registrator.Module.BusinessObjects;
using Registrator.Module.Controllers;
using System.Collections.Generic;
using System.Drawing;

namespace Registrator.Module.Win.Controllers
{
    public class CreateDoctorEventWinController : ObjectViewController<DetailView, CreateDoctorEventParameters>
    {
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            ViewItem item = View.FindItem("Label");
            SchedulerLabelPropertyEditor editor = item != null ? item as SchedulerLabelPropertyEditor : null;
            if (editor != null && editor.Control != null)
            {
                ISchedulerStorage storage = ((AppointmentLabelEdit)editor.Control).Storage;
                IAppointmentLabelStorage labelStorage = storage.Appointments.Labels;
                labelStorage.Clear();
                int i = 0;
                IAppointmentLabel label = labelStorage.CreateNewLabel(i, "Нет", "Нет");
                label.SetColor(Color.White);
                labelStorage.Add(label);
                i++;
                using (IObjectSpace os = Application.CreateObjectSpace())
                {
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
}
