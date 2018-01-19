using System;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Design;
using DevExpress.XtraEditors;
using Registrator.Module.BusinessObjects;
using Registrator.Module.BusinessObjects.Abstract;
using Registrator.Module.BusinessObjects.Dictionaries;
using ListView = DevExpress.ExpressApp.ListView;

namespace Registrator.Module.Controllers
{
    public partial class CommonServiceViewController : ViewController
    {
        public CommonServiceViewController()
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
                //var newController = Frame.GetController<DevExpress.ExpressApp.SystemModule.NewObjectViewController>();
                //newController.ObjectCreated += NewControllerOnObjectCreated;

                Frame.GetController<DevExpress.ExpressApp.SystemModule.DeleteObjectsViewController>().Active.SetItemValue("EnabledDeleteAction", false);
                Frame.GetController<DevExpress.ExpressApp.SystemModule.LinkUnlinkController>().Active.SetItemValue("EnabledLinkAction", false);
            }
        }

        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            
            // в нем находим контроллер обработчика текущего объекта списка
            var controller = this.Frame.GetController<TextTemplateViewController>();
            // добавляем обработчик для выбранного элемента
            controller.TextTemplateItemProcess += ServiceViewController_TextTemplateItemProcess;
        }

        private void ServiceViewController_TextTemplateItemProcess(object sender, EventArgs eArgs)
        {
            var e = eArgs as CustomProcessListViewSelectedItemEventArgs;
            var template = e.InnerArgs.CurrentObject as TextTemplate;
            if (template != null)
            {
                if (this.Frame is NestedFrame)
                {
                    var detailView = ((NestedFrame)this.Frame).ViewItem.View as DetailView;
                    if (detailView != null)
                    {
                        var itemId = "CommonProtocol.Anamnez";
                        if (template is ComplainTemplate)
                            itemId = "CommonProtocol.Complain";
                        if (template is RecomendTemplate)
                            itemId = "CommonProtocol.Recommendation";
                        if (template is ObjStatusTerTemplate)
                            itemId = "CommonProtocol.ObjectiveStatus";

                        var pEditor = detailView.FindItem(itemId) as StringPropertyEditor;
                        if (pEditor != null)
                        {
                            pEditor.ReadValue();
                            var oldValue = pEditor.ControlValue == null ? "" : pEditor.ControlValue.ToString();
                            pEditor.Control.EditValue = oldValue + Environment.NewLine +  template.ToString();

                            pEditor.WriteValue();

                            ObjectSpace.SetModified(detailView.CurrentObject);
                        }
                    }
                }
                e.Handled = true;
            }
        }

        /*
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            var view = View as DetailView;
            if (view != null)
            {
                var commonService = view.CurrentObject as CommonService;
                if (commonService!=null)
                {
                    // Основная услуга может изменяться в некоторых случаях (например, медикаментозный аборт, операции)
                    var d1Diagnose = view.FindItem("CaseDiagnose") as PropertyEditor;
                    var d1Time = view.FindItem("CaseDiagnoseIsFirstTime") as PropertyEditor;
                    var d1Phase = view.FindItem("CaseDiagnoseStadia") as PropertyEditor;
                    var d1Character = view.FindItem("CaseDiagnoseCharacter") as PropertyEditor;

                    if (d1Diagnose != null)
                        d1Diagnose.AllowEdit.SetItemValue("DenyEditCaseDiagn", commonService.IsMainService);
                    if (d1Time != null)
                        d1Time.AllowEdit.SetItemValue("DenyEditPreDiagn", commonService.IsMainService);
                    if (d1Phase != null)
                        d1Phase.AllowEdit.SetItemValue("DenyEditPreDiagn", commonService.IsMainService);
                    if (d1Character != null)
                        d1Character.AllowEdit.SetItemValue("DenyEditPreDiagn", commonService.IsMainService);
                }
            }
        }
        */
    }
}
