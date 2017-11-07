using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using System;

namespace Registrator.Module.Controllers
{
    /// <summary>
    /// Prevent SimultaneousDataModificationException raised when modified object is 
    /// opened from nestedListView in new MDI Tab
    /// </summary>
    public partial class CustomController : ViewController
    {
        public CustomController()
        {
            InitializeComponent();
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem += 
                new EventHandler<CustomProcessListViewSelectedItemEventArgs>(object_CustomProcessSelectedItem);
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
