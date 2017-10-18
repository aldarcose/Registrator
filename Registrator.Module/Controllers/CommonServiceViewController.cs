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
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class CommonServiceViewController : ViewController
    {
        public CommonServiceViewController()
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
                var newController = Frame.GetController<DevExpress.ExpressApp.SystemModule.NewObjectViewController>();
                newController.ObjectCreated += NewControllerOnObjectCreated;

                Frame.GetController<DevExpress.ExpressApp.SystemModule.DeleteObjectsViewController>().Active.SetItemValue("EnabledDeleteAction", false);
                Frame.GetController<DevExpress.ExpressApp.SystemModule.LinkUnlinkController>().Active.SetItemValue("EnabledLinkAction", false);
            }

            var detailView = View as DetailView;
            if (detailView!=null)
            {
                var commonService = detailView.CurrentObject as CommonService;
                if (commonService != null)
                {
                    if (commonService.AutoOpen)
                        commonService.AutoOpen = false;

                    if (commonService.Usluga == null)
                    {
                        if (commonService.Case is VisitCase){
                            if (commonService.Case.Doctor != null && commonService.Case.Doctor.SpecialityTree != null)
                            {
                                // устанавливаем услугу по умолчанию
                                if (commonService.Case.Pacient.IsInogorodniy.HasValue &&
                                    commonService.Case.Pacient.IsInogorodniy.Value)
                                    commonService.Usluga = commonService.Case.Doctor.SpecialityTree.UslugaMUR;
                                else
                                    commonService.Usluga = ((VisitCase) commonService.Case).Mesto == MestoObsluzhivaniya.LPU
                                        ? commonService.Case.Doctor.SpecialityTree.UslugaLPU
                                        : commonService.Case.Doctor.SpecialityTree.UslugaNaDomy;
                            }
                        }
                    }
                }
            }
        }

        private void NewControllerOnObjectCreated(object sender, ObjectCreatedEventArgs objectCreatedEventArgs)
        {
            var lookAndFeel = new UserLookAndFeel(this);
            var result = XtraMessageBox.Show(lookAndFeel, "Услуга производится в ЛПУ?", "Уточнение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            var atLpu = result == DialogResult.Yes;

            var commonService = (objectCreatedEventArgs.CreatedObject as CommonService);
            if (commonService.Usluga == null)
            {
                if (commonService.Case is VisitCase)
                {
                    if (commonService.Case.Doctor != null && commonService.Case.Doctor.SpecialityTree != null)
                    {
                        // устанавливаем услугу по умолчанию
                        if (commonService.Case.Pacient.IsInogorodniy.HasValue &&
                            commonService.Case.Pacient.IsInogorodniy.Value)
                            commonService.Usluga = commonService.Case.Doctor.SpecialityTree.UslugaMUR;
                        else
                            commonService.Usluga = atLpu
                                ? commonService.Case.Doctor.SpecialityTree.UslugaLPU
                                : commonService.Case.Doctor.SpecialityTree.UslugaNaDomy;
                    }
                }
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
                    /*var serviceProperty = view.FindItem("Usluga") as PropertyEditor;
                    if (serviceProperty != null)
                        serviceProperty.AllowEdit.SetItemValue("DenyChangeDefault", !commonService.IsMainService);*/
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
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
