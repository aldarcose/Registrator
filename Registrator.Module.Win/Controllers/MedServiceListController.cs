using DevExpress.ExpressApp;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Registrator.Module.BusinessObjects;
using Registrator.Module.BusinessObjects.Abstract;

namespace Registrator.Module.Win.Controllers
{
    public class MedServiceListController : ObjectViewController<ListView, CommonService>
    {
        private GridView gridView;

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            SplitContainerControl container = View.Control as SplitContainerControl;
            if (container == null) return;
            // Pacient_VisitCaseServices_ListView
            var gridControl = container.Controls[0].Controls[0] as GridControl;
            if (gridControl == null)
                return;
            gridView = gridControl.MainView as GridView;
            if (gridView == null)
                return;
            gridView.CustomDrawGroupRow += gridView_CustomDrawGroupRow;
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            if (gridView != null)
                gridView.CustomDrawGroupRow -= gridView_CustomDrawGroupRow;
        }

        private void gridView_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            object val = view.GetGroupRowValue(e.RowHandle);
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;
            if (info.Column.FieldName == "Oid")
                info.GroupText = "Посещение";
        }
    }
}
