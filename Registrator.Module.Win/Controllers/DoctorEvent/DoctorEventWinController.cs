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

namespace Registrator.Module.Win.Controllers
{
    public class DoctorEventWinController : ObjectViewController<ObjectView, DoctorEvent>
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
                        var storage = scheduler.Storage;
                        storage.Appointments.Labels.Clear();
                        storage.Appointments.Labels.Add(storage.Appointments.Labels.CreateNewLabel(15, "fuck", "fuck1", Color.Green));
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
                        labelStorage.Clear();
                        IAppointmentLabel label = labelStorage.CreateNewLabel(15, "fuck", "fuck1");
                        label.SetColor(Color.Red);
                        labelStorage.Add(label);
                    }
            }
        }
    }
}
