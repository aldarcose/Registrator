namespace Registrator.Module.Controllers
{
    partial class DispCaseViewController
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
            this.NewDispCaseAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.simpleAction1 = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // NewDispCaseAction
            // 
            this.NewDispCaseAction.Caption = "Создать";
            this.NewDispCaseAction.Category = "Edit";
            this.NewDispCaseAction.ConfirmationMessage = null;
            this.NewDispCaseAction.Id = "66166522-d4a0-42f7-a419-39bb06d40fbd";
            this.NewDispCaseAction.ImageName = "MenuBar_New";
            this.NewDispCaseAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.NewDispCaseAction.ToolTip = null;
            this.NewDispCaseAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.NewDispCaseAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.NewDispCaseAction_Execute);
            // 
            // simpleAction1
            // 
            this.simpleAction1.Caption = "Тестовая выгрузка";
            this.simpleAction1.ConfirmationMessage = null;
            this.simpleAction1.Id = "07c26a95-60dd-4955-bb2d-448a5c3d6acd";
            this.simpleAction1.TargetViewNesting = DevExpress.ExpressApp.Nesting.Nested;
            this.simpleAction1.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.simpleAction1.ToolTip = null;
            this.simpleAction1.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.simpleAction1.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.simpleAction1_Execute);
            // 
            // DispCaseViewController
            // 
            this.Actions.Add(this.NewDispCaseAction);
            this.Actions.Add(this.simpleAction1);
            this.TargetObjectType = typeof(Registrator.Module.BusinessObjects.Abstract.DispCase);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction NewDispCaseAction;
        private DevExpress.ExpressApp.Actions.SimpleAction simpleAction1;
    }
}
