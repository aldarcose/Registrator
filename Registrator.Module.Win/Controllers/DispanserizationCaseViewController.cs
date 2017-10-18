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
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace Registrator.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class DispanserizationCaseViewController : ViewController
    {
        public DispanserizationCaseViewController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }

        private PropertyEditor propertyEditor = null;
        private PropertyEditor isHealthyEditor = null;
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            var dv = View as DetailView;
            if (dv != null)
            {
                // Access and customize the target View control.
                propertyEditor = dv.FindItem("Rehab.IsNeed") as BooleanPropertyEditor;
                if (propertyEditor != null)
                {
                    propertyEditor.ControlValueChanged += propertyEditor_ControlValueChanged;
                }
                isHealthyEditor = dv.FindItem("IsPacientHealthy") as BooleanPropertyEditor;
                if (isHealthyEditor != null)
                {
                    isHealthyEditor.ControlValueChanged += isHealthyEditor_ControlValueChanged;
                }
            }
        }

        private void propertyEditor_ControlValueChanged(object sender, EventArgs e)
        {
            var boolEditor = sender as BooleanPropertyEditor;
            var value = (Boolean)boolEditor.ControlValue;
            SetPropertyEditorsEditable(value);
        }

        private void isHealthyEditor_ControlValueChanged(object sender, EventArgs e)
        {
            var boolEditor = sender as BooleanPropertyEditor;
            var value = (Boolean)boolEditor.ControlValue;
            SetInspectionPropertyEditorsEditable(value);
        }

        private void SetPropertyEditorsEditable(bool value)
        {
            var dv = View as DetailView;
            if (dv != null)
            {
                var progress = dv.FindItem("Rehab.Progress") as PropertyEditor;
                var date = dv.FindItem("Rehab.SetDate") as PropertyEditor;
                if (progress != null && date != null)
                {
                    progress.AllowEdit.SetItemValue("AutoEnableDisableRehabProgress", value);
                    date.AllowEdit.SetItemValue("AutoEnableDisableRehabDate", value);
                }
            }
        }

        private void SetInspectionPropertyEditorsEditable(bool value)
        {
            var dv = View as DetailView;
            if (dv != null)
            {
                var result = dv.FindItem("InspectionResult") as PropertyEditor;
                if (result != null)
                {
                    result.AllowEdit.SetItemValue("AutoEnableDisableInspection", value);
                }
            }
        }

        protected override void OnDeactivated()
        {
            if (propertyEditor != null)
            {
                propertyEditor.ControlValueChanged -= propertyEditor_ControlValueChanged;
            }
            if (isHealthyEditor != null)
            {
                isHealthyEditor.ControlValueChanged -= isHealthyEditor_ControlValueChanged;
            }
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
