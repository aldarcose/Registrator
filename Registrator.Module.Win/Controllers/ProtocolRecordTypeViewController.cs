using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using Registrator.Module.BusinessObjects.Dictionaries;
using System;

namespace Registrator.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class ProtocolRecordTypeViewController : ViewController
    {
        public ProtocolRecordTypeViewController()
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
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            var dv = View as DetailView;
            if (dv != null)
            {
                // Access and customize the target View control.
                propertyEditor = dv.FindItem("TimeType") as EnumPropertyEditor;
                if (propertyEditor != null)
                {
                    propertyEditor.ControlValueChanged += propertyEditor_ControlValueChanged;
                }
            }
        }

        private void propertyEditor_ControlValueChanged(object sender, EventArgs e)
        {
            var enumEditor = sender as EnumPropertyEditor;
            var value = (TimeTypes)enumEditor.Control.SelectedIndex;
            if (value != null)
            {
                SetPropertyEditorsEditable(value);
            }
        }

        private void SetPropertyEditorsEditable(TimeTypes value)
        {
            var dv = View as DetailView;
            if (dv != null)
            {
                var fromYear = dv.FindItem("TimeFrom.Year") as PropertyEditor;
                var fromMonth = dv.FindItem("TimeFrom.Month") as PropertyEditor;
                var toYear = dv.FindItem("TimeTo.Year") as PropertyEditor;
                var toMonth = dv.FindItem("TimeTo.Month") as PropertyEditor;
                if (fromYear != null && fromMonth != null && toYear != null && toMonth != null)
                {
                    switch (value)
                    {
                        case TimeTypes.FromLimit:
                            fromYear.AllowEdit.SetItemValue("AutoEnableDisableFromYear", true);
                            fromMonth.AllowEdit.SetItemValue("AutoEnableDisableFromMonth", true);
                            toYear.AllowEdit.SetItemValue("AutoEnableDisableToYear", false);
                            toMonth.AllowEdit.SetItemValue("AutoEnableDisableToMonth", false);
                            break;
                        case TimeTypes.ToLimit:
                            fromYear.AllowEdit.SetItemValue("AutoEnableDisableFromYear", false);
                            fromMonth.AllowEdit.SetItemValue("AutoEnableDisableFromMonth", false);
                            toYear.AllowEdit.SetItemValue("AutoEnableDisableToYear", true);
                            toMonth.AllowEdit.SetItemValue("AutoEnableDisableToMonth", true);
                            break;
                        case TimeTypes.Range:
                            fromYear.AllowEdit.SetItemValue("AutoEnableDisableFromYear", true);
                            fromMonth.AllowEdit.SetItemValue("AutoEnableDisableFromMonth", true);
                            toYear.AllowEdit.SetItemValue("AutoEnableDisableToYear", true);
                            toMonth.AllowEdit.SetItemValue("AutoEnableDisableToMonth", true);
                            break;
                        case TimeTypes.All:
                            fromYear.AllowEdit.SetItemValue("AutoEnableDisableFromYear", false);
                            fromMonth.AllowEdit.SetItemValue("AutoEnableDisableFromMonth", false);
                            toYear.AllowEdit.SetItemValue("AutoEnableDisableToYear", false);
                            toMonth.AllowEdit.SetItemValue("AutoEnableDisableToMonth", false);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        protected override void OnDeactivated()
        {
            if (propertyEditor != null)
            {
                propertyEditor.ControlValueChanged -= propertyEditor_ControlValueChanged;
            }
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void simpleAction1_Execute(object sender, DevExpress.ExpressApp.Actions.SimpleActionExecuteEventArgs e)
        {
            var lv = View as ListView;
            if (lv != null)
            {
                foreach (ProtocolRecordType recordType in lv.SelectedObjects)
                {
                    if (recordType.Code == null)
                    {
                        recordType.Code = recordType.GetDefaultCode();
                        recordType.Save();
                        ObjectSpace.CommitChanges();
                    }
                }
            }
        }
    }
}
