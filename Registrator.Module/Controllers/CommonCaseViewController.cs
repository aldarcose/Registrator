using System;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.StateMachine;
using DevExpress.ExpressApp.SystemModule;
using Registrator.Module.BusinessObjects;
using Registrator.Module.BusinessObjects.Abstract;
using DevExpress.ExpressApp.DC;
using ListView = DevExpress.ExpressApp.ListView;

namespace Registrator.Module.Controllers
{
    public partial class CommonCaseViewController : ViewController
    {
        public CommonCaseViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            var listView = View as ListView;
            
            if (listView != null)
            {
                //Frame.GetController<DevExpress.ExpressApp.SystemModule.NewObjectViewController>().Active.SetItemValue("EnabledNewAction", false);
                //Frame.GetController<DevExpress.ExpressApp.SystemModule.DeleteObjectsViewController>().Active.SetItemValue("EnabledDeleteAction", false);
                //Frame.GetController<DevExpress.ExpressApp.SystemModule.LinkUnlinkController>().Active.SetItemValue("EnabledLinkAction", false);
            }
        }
    }
}
