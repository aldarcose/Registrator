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
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using Registrator.Module.BusinessObjects;

namespace Registrator.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class StreetSelectorController : ViewController
    {
        public StreetSelectorController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
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

        private void openSelectorAction_Cancel(object sender, EventArgs e)
        {

        }

        private void openSelectorAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var obj = e.PopupWindowViewCurrentObject as Address;
        }

        private void openSelectorAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            e.View = Application.CreateDetailView(objectSpace, new Address(((XPObjectSpace)objectSpace).Session));
        }

        private void openFactSelectorAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var obj = e.PopupWindowViewCurrentObject as Address;

            var sss = sender;
        }

        private void openFactSelectorAction_Cancel(object sender, EventArgs e)
        {

        }

        private void openFactSelectorAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            e.View = Application.CreateDetailView(objectSpace, new Address(((XPObjectSpace)objectSpace).Session));
        }
    }
}
