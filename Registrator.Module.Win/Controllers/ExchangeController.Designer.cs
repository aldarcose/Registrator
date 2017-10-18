namespace Registrator.Module.Win.Controllers
{
    partial class ExchangeController
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
            this.LoadLAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.SaveLAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.SaveHAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.LoadHAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // LoadLAction
            // 
            this.LoadLAction.Caption = "Импорт L-файла";
            this.LoadLAction.ConfirmationMessage = null;
            this.LoadLAction.Id = "LoadLAction";
            this.LoadLAction.TargetObjectType = typeof(Registrator.Module.BusinessObjects.DTO.PERS_LIST);
            this.LoadLAction.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.LoadLAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.LoadLAction.ToolTip = null;
            this.LoadLAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.LoadLAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.LoadXMLAction_Execute);
            // 
            // SaveLAction
            // 
            this.SaveLAction.Caption = "Экспорт L-файла";
            this.SaveLAction.ConfirmationMessage = null;
            this.SaveLAction.Id = "SaveLAction";
            this.SaveLAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.SaveLAction.TargetObjectType = typeof(Registrator.Module.BusinessObjects.DTO.PERS_LIST);
            this.SaveLAction.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.SaveLAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.SaveLAction.ToolTip = null;
            this.SaveLAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.SaveLAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.SaveLAction_Execute);
            // 
            // SaveHAction
            // 
            this.SaveHAction.Caption = "Экспорт H-файла";
            this.SaveHAction.ConfirmationMessage = null;
            this.SaveHAction.Id = "SaveHAction";
            this.SaveHAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.SaveHAction.TargetObjectType = typeof(Registrator.Module.BusinessObjects.DTO.ZL_LIST);
            this.SaveHAction.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.SaveHAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.SaveHAction.ToolTip = null;
            this.SaveHAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.SaveHAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.SaveHAction_Execute);
            // 
            // LoadHAction
            // 
            this.LoadHAction.Caption = "Импорт H-файла";
            this.LoadHAction.ConfirmationMessage = null;
            this.LoadHAction.Id = "LoadHAction";
            this.LoadHAction.TargetObjectType = typeof(Registrator.Module.BusinessObjects.DTO.ZL_LIST);
            this.LoadHAction.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.LoadHAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.LoadHAction.ToolTip = null;
            this.LoadHAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.LoadHAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.LoadHAction_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction LoadLAction;
        private DevExpress.ExpressApp.Actions.SimpleAction SaveLAction;
        private DevExpress.ExpressApp.Actions.SimpleAction SaveHAction;
        private DevExpress.ExpressApp.Actions.SimpleAction LoadHAction;
    }
}
