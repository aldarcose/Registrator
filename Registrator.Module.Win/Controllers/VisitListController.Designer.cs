namespace Registrator.Module.Win.Controllers
{
    partial class VisitListController
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
            this.action_get_emergency = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // action_get_emergency
            // 
            this.action_get_emergency.Caption = "Отчет неотложка";
            this.action_get_emergency.ConfirmationMessage = null;
            this.action_get_emergency.Id = "0e74713d-c220-43fe-be40-38de76c7a9a7";
            this.action_get_emergency.ToolTip = null;
            this.action_get_emergency.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.action_get_emergency_Execute);
            // 
            // VisitListController
            // 
            this.Actions.Add(this.action_get_emergency);
            this.TargetObjectType = typeof(Registrator.Module.BusinessObjects.Abstract.CommonCase);
            this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.TypeOfView = typeof(DevExpress.ExpressApp.ListView);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction action_get_emergency;
    }
}
