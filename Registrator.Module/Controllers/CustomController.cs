using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using Registrator.Module.BusinessObjects;
using System.Linq;
using System;

namespace Registrator.Module.Controllers
{
    /// <summary>
    /// Prevent SimultaneousDataModificationException raised when modified object is 
    /// opened from nestedListView in new MDI Tab.
    /// Отключение лишних контроллеров
    /// </summary>
    public partial class CustomController : ViewController
    {
        public CustomController()
        {
            InitializeComponent();
        }

        protected override void OnActivated()
        {
            Doctor curDoctor = (Doctor)SecuritySystem.CurrentUser;
            bool isAdmin = curDoctor.DoctorRoles.Any(t => t.IsAdministrative);

            // Отключение лишних контроллеров для пользователей (докторов) не имеющих админские права
            if (!isAdmin)
            {
                DisableControllers();
            }
            
            base.OnActivated();
            Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem += 
                new EventHandler<CustomProcessListViewSelectedItemEventArgs>(object_CustomProcessSelectedItem);
        }

        protected virtual void Disable<T>()
           where T : Controller
        {
            var c = Frame.GetController<T>();
            if (c != null)
            {
                c.Active["NonPersistent"] = false;
            }
        }

        protected virtual void DisableControllers()
        {
            Disable<ResetViewSettingsController>();
            Disable<FilterController>();
        }

        void object_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            if (View.ObjectSpace.ModifiedObjects.Contains(e.InnerArgs.CurrentObject))
            {
                View.ObjectSpace.CommitChanges();
            }
        }
    }
}
