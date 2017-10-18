using DevExpress.ExpressApp.Actions;

namespace Registrator.Module.Win.Controllers
{
    partial class ProtocolRecordViewController
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
            // 
            // ProtocolRecordViewController
            // 
            this.TargetObjectType = typeof(Registrator.Module.BusinessObjects.Dictionaries.ProtocolRecord);
            this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.TypeOfView = typeof(DevExpress.ExpressApp.ListView);

            this.components = new System.ComponentModel.Container();
            this.collapseAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.expandAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // свернуть
            // 
            this.collapseAction.Caption = "Свернуть";
            this.collapseAction.ConfirmationMessage = null;
            this.collapseAction.Id = "collapseAction";
            this.collapseAction.ToolTip = null;
            this.collapseAction.Category = "Edit";
            this.collapseAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.collapseAction_Execute);

            // 
            // Развернуть
            // 
            this.expandAction.Caption = "Развернуть";
            this.expandAction.ConfirmationMessage = null;
            this.expandAction.Id = "expandAction";
            this.expandAction.ToolTip = null;
            this.expandAction.Category = "Edit";
            this.expandAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.expandAction_Execute);
            // 
        }

        

        #endregion
    }
}
