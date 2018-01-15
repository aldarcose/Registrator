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

namespace Registrator.Module.Controllers
{
    public partial class MKBWithDispInfoBeforeViewController : ViewController
    {
        public MKBWithDispInfoBeforeViewController()
        {
            InitializeComponent();
        }
        
        private PropertyEditor notDoneHealingEditor;
        private PropertyEditor notDoneRehabEditor;
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            if (View is DetailView)
            {
                var dv = View as DetailView;

                notDoneHealingEditor = dv.FindItem("Healing.NotDone") as PropertyEditor;
                if (notDoneHealingEditor != null)
                {
                    notDoneHealingEditor.ControlValueChanged += notDoneHealingEditor_ControlValueChanged;
                    SetHealingPropertiesEnabled((bool)notDoneHealingEditor.ControlValue);
                }

                notDoneRehabEditor = dv.FindItem("Rehabilitation.NotDone") as PropertyEditor;
                if (notDoneRehabEditor != null)
                {
                    notDoneRehabEditor.ControlValueChanged += notDoneRehabEditor_ControlValueChanged;
                    SetRehabPropertiesEnabled((bool) notDoneRehabEditor.ControlValue);
                }
            }
        }

        void notDoneHealingEditor_ControlValueChanged(object sender, EventArgs e)
        {
            var newValue = (bool)notDoneHealingEditor.ControlValue;
            if (newValue!=null)
            {
                SetHealingPropertiesEnabled(newValue);
            }
        }

        void notDoneRehabEditor_ControlValueChanged(object sender, EventArgs e)
        {
            var newValue = (bool)notDoneRehabEditor.ControlValue;
            if (newValue!=null)
            {
                SetRehabPropertiesEnabled(newValue);
            }
        }

        private void SetHealingPropertiesEnabled(bool newValue)
        {
            var p1 = ((DetailView) View).FindItem("Healing.NotDoneReason") as PropertyEditor;
            var p2 = ((DetailView)View).FindItem("Healing.AnotherReason") as PropertyEditor;

            p1.AllowEdit.SetItemValue("HealNotDoneReason", newValue);
            p2.AllowEdit.SetItemValue("HealAnotherReason", newValue);
        }

        private void SetRehabPropertiesEnabled(bool newValue)
        {
            var p1 = ((DetailView)View).FindItem("Rehabilitation.NotDoneReason") as PropertyEditor;
            var p2 = ((DetailView)View).FindItem("Rehabilitation.AnotherReason") as PropertyEditor;

            p1.AllowEdit.SetItemValue("RehabNotDoneReason", newValue);
            p2.AllowEdit.SetItemValue("RehabAnotherReason", newValue);

        }

        protected override void OnDeactivated()
        {
            if (notDoneHealingEditor != null)
                notDoneHealingEditor.ControlValueChanged -= notDoneHealingEditor_ControlValueChanged;

            if (notDoneRehabEditor != null)
                notDoneRehabEditor.ControlValueChanged -= notDoneRehabEditor_ControlValueChanged;
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
