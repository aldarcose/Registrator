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
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class CommonCaseViewController : ViewController
    {
        public CommonCaseViewController()
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
                Frame.GetController<DevExpress.ExpressApp.SystemModule.NewObjectViewController>().Active.SetItemValue("EnabledNewAction", false);
                Frame.GetController<DevExpress.ExpressApp.SystemModule.DeleteObjectsViewController>().Active.SetItemValue("EnabledDeleteAction", false);
                Frame.GetController<DevExpress.ExpressApp.SystemModule.LinkUnlinkController>().Active.SetItemValue("EnabledLinkAction", false);
            }
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.

            var detailView = View as DetailView;

            if (detailView != null)
            {
                var commonCase = View.CurrentObject as CommonCase;
                
                if (commonCase != null)
                {
                    // если нужно автооткрыть первую услугу, делаем это
                    if (commonCase.Services.Count == 1 && commonCase.Services[0].AutoOpen)
                    {
                        
                        var os = Application.CreateObjectSpace();
                        var service = os.GetObject(commonCase.Services[0]);

                        DetailView dv = Application.CreateDetailView(os, service);
                        ShowViewParameters svp = new ShowViewParameters();
                        svp.CreatedView = dv;
                        svp.TargetWindow = TargetWindow.NewModalWindow;
                            
                        //отображаем окно
                        Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                    }
                }
            }
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void NewAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var listView = View as ListView;

            if (listView != null)
            {
                string visitListViewId = "VisitCases_ListView";
                string hospitalListViewId = "HospitalCases_ListView";

                if (listView.Id.Contains(visitListViewId))
                {
                    ShowViewParameters svp = new ShowViewParameters();

                    var os = Application.CreateObjectSpace();
                    DetailView dv = Application.CreateDetailView(os, new VisitCaseFields());
                    svp.CreatedView = dv;
                    svp.TargetWindow = TargetWindow.NewModalWindow;
                    DialogController dc = new DialogController();
                    dc.Accepting += CelVisita_Accepting;
                    dc.CancelAction.Caption = "Отмена";
                    svp.Controllers.Add(dc);
                    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, (ActionBase)sender));

                }

                if (listView.Id.Contains(hospitalListViewId)){
                    CreateCommonCase(typeof(HospitalCase));
                }
            }
        }

        private void CreateCommonCase(Type commonCaseType, VisitCaseFields fields = null)
        {
            var listView = View as ListView;
            if (listView != null)
            {
                var pacient = ((PropertyCollectionSource)listView.CollectionSource).MasterObject as Pacient;
                if (pacient != null)
                {
                    // создаем ObjectSpace
                    var os = Application.CreateObjectSpace();
                    // создаем нужный объект в этом пространстве
                    var commonCase = os.CreateObject(commonCaseType);
                    // привязываем случай к пациенту
                    ((AbstractCase) commonCase).Pacient = os.GetObject(pacient);
                    if (fields != null)
                    {
                        ((VisitCase)commonCase).Cel = fields.CelPosesch;
                        ((VisitCase)commonCase).Mesto = fields.Mesto;
                    }

                    // Промежуточный коммит. GetObjectsNonReenterant workaround.
                    os.CommitChanges();

                    // создаем детальный вид
                    ShowViewParameters svp = new ShowViewParameters();
                    DetailView dv = Application.CreateDetailView(os, commonCase);
                    dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
                    svp.CreatedView = dv;

                    //svp.TargetWindow = TargetWindow.NewModalWindow;
                    Application.ShowViewStrategy.ShowView(svp,new ShowViewSource(Frame, (ActionBase) NewCommonCaseAction));
                }
            }
        }

        private void CelVisita_Accepting(object sender, DialogControllerAcceptingEventArgs e)
        {
            var field = e.AcceptActionArgs.CurrentObject as VisitCaseFields;
            if (field != null)
                CreateCommonCase(typeof(VisitCase), field);
        }
    }

    [DomainComponent]
    [XafDisplayName("Укажите данные посещения")]
    public class VisitCaseFields
    {
        public VisitCaseFields()
        {
            CelPosesch = CelPosescheniya.LechebnoDiagnosticheskaya;
        }

        [XafDisplayName("Цель посещения")]
        public CelPosescheniya CelPosesch { get; set; }

        [XafDisplayName("Место обслуживания")]
        public MestoObsluzhivaniya Mesto { get; set; }
    }
}
