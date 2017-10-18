using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;

namespace Registrator.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class PacientDynamicMaskChangeController : ViewController
    {
        public PacientDynamicMaskChangeController()
        {
            InitializeComponent();
            RegisterActions(components);
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

            PropertyEditor propertyEditor = ((DetailView)View).FindItem("Document.Type") as PropertyEditor;

            if (propertyEditor != null)
            {
                AttachEvent(propertyEditor);
            }
        }

        private void propertyEditor_ControlCreated(object sender, EventArgs e)
        {
            AttachEvent(sender as PropertyEditor);
        }

        private void AttachEvent(PropertyEditor propertyEditor)
        {
            if (propertyEditor == null) return;

            propertyEditor.ControlValueChanged += propertyEditor_ControlValueChanged;
        }

        void propertyEditor_ControlValueChanged(object sender, EventArgs e)
        {
            var propertyEditorType = sender as PropertyEditor;

            PropertyEditor propertyEditorSerial = ((DetailView)View).FindItem("Document.Serial") as PropertyEditor;
            PropertyEditor propertyEditorNumber = ((DetailView)View).FindItem("Document.Number") as PropertyEditor;

            var value = propertyEditorType.ControlValue as Registrator.Module.BusinessObjects.Dictionaries.VidDocumenta;

            if (value != null)
            {
                var controlSerial = ((StringEdit)propertyEditorSerial.Control);
                var controlNumber = ((StringEdit)propertyEditorNumber.Control);

                controlSerial.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
                controlNumber.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;

                int start, len;
                GetIndexes(value.MaskSerial, out start, out len);
                var maskSerial = value.MaskSerial.Substring(start, len); // удаляем символы ("^$");

                GetIndexes(value.MaskNumber, out start, out len);
                var maskNumber = value.MaskNumber.Substring(start, len); // удаляем символы ("^$");

                controlSerial.Properties.Mask.EditMask = maskSerial;
                controlNumber.Properties.Mask.EditMask = maskNumber;
            }
        }

        private void GetIndexes(string value, out int start, out int len)
        {
            start = 0;
            len = 0;

            if (string.IsNullOrEmpty(value)) return;

            start = value[0] == '^' ? 1 : 0;

            if (value.Length > 0)
            {
                len = (value[value.Length - 1] == '$') ? value.Length - 1 : value.Length;
                len = len - start;
            }

        }

        private void DocumentChangeMaskController_ViewControlsCreated(object sender, EventArgs e)
        {
            PropertyEditor propertyEditor = ((DetailView)View).FindItem("Type") as PropertyEditor;

            if (propertyEditor != null)
            {
                AttachEvent(propertyEditor);
            }
            else
            {
                propertyEditor.ControlCreated += propertyEditor_ControlCreated;
            }
        }
           

        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
