using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.Win.SystemModule;

namespace WinSolution.Module.Win
{
    public partial class ResetSettingsViewController : ViewController
    {
        private const string EnabledKey = "DisabledForNewObjectInDetailView";
        EventHandler committedEventHandler = null;
        public SimpleAction ResetViewSettingsAction { get; private set; }
        public ResetSettingsViewController()
        {
            TargetViewNesting = Nesting.Root;
            ResetViewSettingsAction = new SimpleAction(this, "ResetViewSettings_", DevExpress.Persistent.Base.PredefinedCategory.View);
            ResetViewSettingsAction.ImageName = "Attention";
            ResetViewSettingsAction.Caption = "Сбросить настройки вида";
            ResetViewSettingsAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.ResetViewSettingsAction_Execute);
#if EASYTEST
            new SimpleAction(this, "Test", DevExpress.Persistent.Base.PredefinedCategory.View).Execute += (s, e) => {
                if(View is DetailView) {
                    DetailView dv = (DetailView)View;
                    ((DevExpress.ExpressApp.Editors.IAppearanceVisibility)dv.FindItem("Text")).Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide;
                }
                else if(View is ListView) {
                    ListView lv = (ListView)View;
                    lv.Model.Columns["Text"].Index = -1;
                    lv.LoadModel();//Dennis: This internal method is used for testing purposes only. Do not use it in real apps.
                }
            };
#endif
        }
        private void ResetViewSettingsAction_Execute(object sender, DevExpress.ExpressApp.Actions.SimpleActionExecuteEventArgs e)
        {
            try
            {
                IModelView oldModel = View.Model;
                ViewShortcut oldViewShortcut = Frame.View.CreateShortcut();
                Frame.SetView(null);
                ((ModelNode)oldModel).Undo();
                Frame.SetView(Application.ProcessShortcut(oldViewShortcut), true, Frame);
                
                ((IDockManagerHolder) Application.MainWindow.Template).DockManager.Panels[0].Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;

                // что-то еще что можно сбросить
            }
            catch
            {
                XtraMessageBox.Show("An error occurred when resetting the View's settings. Please contact your application administrator for a solution. We will try to close the current View after you press the OK button.",
                    ResetViewSettingsAction.Caption,
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error
                );
                View.Close();
            }
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            if (View.ObjectSpace.IsNewObject(View.CurrentObject) && (View is DetailView))
            {
                ResetViewSettingsAction.Enabled[EnabledKey] = false;
                committedEventHandler = (s, e) =>
                {
                    View.ObjectSpace.Committed -= committedEventHandler;
                    ResetViewSettingsAction.Enabled[EnabledKey] = true;
                };
                View.ObjectSpace.Committed += committedEventHandler;
            }
        }
        protected override void OnDeactivated()
        {
            View.ObjectSpace.Committed -= committedEventHandler;
            base.OnDeactivated();
        }
    }
}