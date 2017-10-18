namespace Registrator.Module.Controllers
{
    partial class CommonCaseViewController
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.NewCommonCaseAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // NewCommonCaseAction
            // 
            this.NewCommonCaseAction.Caption = "Создать";
            this.NewCommonCaseAction.Category = "Edit";
            this.NewCommonCaseAction.ConfirmationMessage = null;
            this.NewCommonCaseAction.Id = "e242cd3e-e308-4e95-9e59-2de349368ed6";
            this.NewCommonCaseAction.ImageName = "MenuBar_New";
            this.NewCommonCaseAction.TargetViewNesting = DevExpress.ExpressApp.Nesting.Nested;
            this.NewCommonCaseAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.NewCommonCaseAction.ToolTip = null;
            this.NewCommonCaseAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.NewCommonCaseAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.NewAction_Execute);
            // 
            // CommonCaseViewController
            // 
            this.Actions.Add(this.NewCommonCaseAction);
            this.TargetObjectType = typeof(Registrator.Module.BusinessObjects.Abstract.CommonCase);
            this.TypeOfView = typeof(DevExpress.ExpressApp.View);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction NewCommonCaseAction;
    }
}
