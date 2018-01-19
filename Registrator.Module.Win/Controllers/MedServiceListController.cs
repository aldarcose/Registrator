using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Registrator.Module.BusinessObjects;
using Registrator.Module.BusinessObjects.Abstract;
using System;

namespace Registrator.Module.Win.Controllers
{
    /// <summary>
    /// Контроллер списка услуг посещенией 
    /// </summary>
    /// <remarks>
    /// 1. Меняет заголовок группы списочного представления услуги
    /// 2. Создает услугу посещения вместе с посещением, либо добавляет к выбранному посещению
    /// </remarks>
    public class MedServiceGrouppedListController : ObjectViewController<ListView, CommonService>
    {
        private GridView gridView;
        private VisitCase currentVisitCase;
        private MedService currentMedService;
        private NewObjectViewController newObjController;
        private Pacient currentPacient;
        private Doctor currentDoctor;

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            // Получаем текущего пациента
            DetailView ownerView = ObjectSpace.Owner as DetailView;
            currentPacient = ownerView.CurrentObject as Pacient;
            // Текущий доктор
            currentDoctor = ObjectSpace.GetObject((Doctor)SecuritySystem.CurrentUser);
            
            // Контроллер создания услуги
            newObjController = Frame.GetController<DevExpress.ExpressApp.SystemModule.NewObjectViewController>();
            newObjController.ObjectCreating += newObjController_ObjectCreating;

            SplitContainerControl container = View.Control as SplitContainerControl;
            if (container == null) return;
            // Pacient_VisitCaseServices_ListView
            var gridControl = container.Controls[0].Controls[0] as GridControl;
            if (gridControl == null)
                return;

            gridView = gridControl.MainView as GridView;
            gridView.CustomDrawGroupRow += gridView_CustomDrawGroupRow;

            gridView.SelectionChanged += (o, e) =>
            {
                bool isGroupRow = gridView.IsGroupRow(gridView.FocusedRowHandle);
                // Oid посещения если isGroupRow = True
                if (isGroupRow)
                {
                    object selectedGroup = gridView.GetGroupRowValue(gridView.FocusedRowHandle);
                    currentVisitCase = selectedGroup != null && selectedGroup is Guid ?
                        ObjectSpace.FindObject<VisitCase>(VisitCase.Fields.Oid == (Guid)selectedGroup) : null;
                    currentMedService = null;
                }
                else
                {
                    currentVisitCase = null;
                    currentMedService = View.CurrentObject as MedService;
                }
            };
        }

        private void gridView_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            object val = view.GetGroupRowValue(e.RowHandle);
            VisitCase visitCase = val != null && val is Guid ? 
                ObjectSpace.FindObject<VisitCase>(VisitCase.Fields.Oid == (Guid)val) : null;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;
            if (info.Column.FieldName == "Case.Oid")
                info.GroupText = string.Format("Посещение ({0})", visitCase != null &&
                    visitCase.MainDiagnose != null && visitCase.MainDiagnose.Diagnose != null ?
                    visitCase.MainDiagnose.Diagnose.MKB : null);
        }

        private void newObjController_ObjectCreating(object sender, ObjectCreatingEventArgs e)
        {
            IObjectSpace objectSpace = e.ObjectSpace;
            MedService newMedService = objectSpace.CreateObject<MedService>();
            if (currentVisitCase != null || currentMedService != null)
            {
                var lookAndFeel = new UserLookAndFeel(this);
                var result = XtraMessageBox.Show(lookAndFeel, "Создать новое посещение?", "Уточнение",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Question);

                if (result == System.Windows.Forms.DialogResult.No)
                {
                    result = XtraMessageBox.Show(lookAndFeel, "Услуга производится в ЛПУ?", "Уточнение",
                        System.Windows.Forms.MessageBoxButtons.YesNo, 
                        System.Windows.Forms.MessageBoxIcon.Question);
                    // устанавливаем услугу по умолчанию
                    SetService(newMedService, result == System.Windows.Forms.DialogResult.Yes);
                    newMedService.Case = currentVisitCase != null ? currentVisitCase : currentMedService.VisitCase;
                    e.NewObject = newMedService;
                    objectSpace.CommitChanges();
                    // Обновление представления пациента
                    ObjectSpace.CommitChanges();
                    ((DetailView)ObjectSpace.Owner).Refresh();
                    return;
                }
            }

            ShowViewParameters svp = new ShowViewParameters();
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, new VisitCaseParameters());
            svp.CreatedView = dv;
            svp.TargetWindow = TargetWindow.NewModalWindow;
            DialogController dc = new DialogController();
            dc.Accepting += (o, e_) =>
            {
                var visitCaseParameters = e_.AcceptActionArgs.CurrentObject as VisitCaseParameters;
                VisitCase newVisitCase = objectSpace.CreateObject<VisitCase>();
                newVisitCase.Pacient = currentPacient;
                newVisitCase.Cel = visitCaseParameters.CelPosesch;
                newVisitCase.Mesto = visitCaseParameters.Mesto;
                newMedService.Case = newVisitCase;
                
                // устанавливаем услугу по умолчанию
                SetService(newMedService, visitCaseParameters.Mesto == MestoObsluzhivaniya.LPU);
                objectSpace.CommitChanges();

                // Обновление представления пациента
                ObjectSpace.CommitChanges();
                ((DetailView)ObjectSpace.Owner).Refresh();
            };
            dc.CancelAction.Caption = "Отмена";
            svp.Controllers.Add(dc);
            Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
            
            e.NewObject = newMedService;
        }

        private void SetService(MedService newMedService, bool lpuService)
        {
            if (currentDoctor != null && currentDoctor.SpecialityTree != null)
            {
                if (currentPacient.IsInogorodniy.HasValue &&
                    currentPacient.IsInogorodniy.Value)
                    newMedService.Usluga = currentDoctor.SpecialityTree.UslugaMUR;
                else
                    newMedService.Usluga = lpuService
                        ? currentDoctor.SpecialityTree.UslugaLPU
                        : currentDoctor.SpecialityTree.UslugaNaDomy;
            }
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            if (gridView != null)
                gridView.CustomDrawGroupRow -= gridView_CustomDrawGroupRow;
            if (newObjController != null)
                newObjController.ObjectCreating -= newObjController_ObjectCreating;
        }
    }

    [DomainComponent]
    [XafDisplayName("Данные услуги и посещения")]
    public class VisitCaseParameters
    {
        public VisitCaseParameters()
        {
            CelPosesch = CelPosescheniya.LechebnoDiagnosticheskaya;
        }

        /// <summary>Цель посещения</summary>
        [XafDisplayName("Цель посещения")]
        public CelPosescheniya CelPosesch { get; set; }

        /// <summary>Место обслуживания</summary>
        [XafDisplayName("Место обслуживания")]
        public MestoObsluzhivaniya Mesto { get; set; }
    }
}
