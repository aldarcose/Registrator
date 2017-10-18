using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Model.NodeGenerators;
using Registrator.Module.BusinessObjects.Abstract;
using Registrator.Module.BusinessObjects.Dictionaries;
using ListView = DevExpress.ExpressApp.ListView;

namespace Registrator.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class DiagnoseListViewController : ViewController
    {
        public DiagnoseListViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.

            var listView = View as ListView;

            if (listView != null)
            {
                var commonService = ((PropertyCollectionSource)listView.CollectionSource).MasterObject as CommonService;
                if (commonService == null)
                {
                    Frame.GetController<DevExpress.ExpressApp.SystemModule.NewObjectViewController>()
                        .Active.SetItemValue("EnabledNewAction", false);
                    Frame.GetController<DevExpress.ExpressApp.SystemModule.DeleteObjectsViewController>()
                        .Active.SetItemValue("EnabledDeleteAction", false);
                    Frame.GetController<DevExpress.ExpressApp.SystemModule.LinkUnlinkController>()
                        .Active.SetItemValue("EnabledLinkAction", false);
                }
            }
        }

        
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
