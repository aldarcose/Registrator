using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Scheduler.Win;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.UI;
using Registrator.Module.Controllers;
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
                IAppointmentLabel label = labelStorage.CreateNewLabel(15, "fuck", "fuck1");
                label.SetColor(Color.Red);
                labelStorage.Add(label);
            }
        }
    }
}
