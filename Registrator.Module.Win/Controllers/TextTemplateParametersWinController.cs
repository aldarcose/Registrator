using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using Registrator.Module.Controllers;

namespace Registrator.Module.Win.Controllers
{
    public class TextTemplateParametersWinController : ObjectViewController<DetailView, TextTemplateEditParameters>
    {
        private LargeStringEdit stringEdit;
        private bool tabPressed = false;
        private int lastSelectionStart = 0;

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            ViewItem item = View.FindItem("Text");
            StringPropertyEditor editor = item != null ? item as StringPropertyEditor : null;
            if (editor != null && editor.Control != null)
            {
                stringEdit = editor.Control as LargeStringEdit;
                stringEdit.PreviewKeyDown += stringEdit_PreviewKeyDown;
                stringEdit.LostFocus += stringEdit_LostFocus;
            }
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            stringEdit.PreviewKeyDown -= stringEdit_PreviewKeyDown;
            stringEdit.LostFocus -= stringEdit_LostFocus;
        }

        private void stringEdit_LostFocus(object sender, System.EventArgs e)
        {
            if (tabPressed)
            {
                stringEdit.Focus();
                stringEdit.SelectionStart = lastSelectionStart;
            }
        }

        private void stringEdit_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            var edit = sender as LargeStringEdit;
            if (e.KeyCode == System.Windows.Forms.Keys.Tab)
            {
                lastSelectionStart = edit.SelectionStart;
                edit.Text = edit.Text.Insert(edit.SelectionStart, "\t");
                tabPressed = true;
            }
            else
            {
                tabPressed = false;
            }
        }
    }
}
