using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Registrator.Module.BusinessObjects;
using Registrator.Module.BusinessObjects.Dictionaries;

namespace Registrator.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class MKBWithTypeViewController : ViewController
    {
        public MKBWithTypeViewController()
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

            var view = View as DetailView;
            /*
            if (view != null)
            {
                var mkb = (MKBWithType)view.CurrentObject;
                if (mkb == null || mkb.Service == null || mkb.Service.IsMainService)
                {
                    return;
                }

                var propertyEditor = view.FindItem("Type") as EnumPropertyEditor;

                if (propertyEditor != null)
                {
                    var combo = (ComboBoxEdit)propertyEditor.Control;
                    for (int i = combo.Properties.Items.Count - 1; i >= 0; i--)
                    {
                        var value = (TipDiagnoza)((ComboBoxItem)combo.Properties.Items[i]).Value;
                        if (value == TipDiagnoza.Pervichniy || value == TipDiagnoza.Osnovnoy)
                        {
                            combo.Properties.Items.RemoveAt(i);
                        }
                    }
                }
            }*/
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
