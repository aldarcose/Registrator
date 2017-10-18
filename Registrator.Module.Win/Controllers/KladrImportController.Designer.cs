namespace Registrator.Module.Win.Controllers
{
    partial class KladrImportController
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
            this.LoadKladrAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // LoadKladrAction
            // 
            this.LoadKladrAction.Caption = "Загрузка КЛАДР";
            this.LoadKladrAction.ConfirmationMessage = null;
            this.LoadKladrAction.Id = "5f49c34e-ead8-4d94-bf23-bfc88d2a60c5";
            this.LoadKladrAction.ToolTip = null;
            this.LoadKladrAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.LoadKladrAction_Execute);
            // 
            // KladrImportController
            // 
            this.TargetObjectType = typeof(Registrator.Module.BusinessObjects.Kladr);
            this.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.TypeOfView = typeof(DevExpress.ExpressApp.ListView);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction LoadKladrAction;
    }
}
