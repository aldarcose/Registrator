namespace Registrator.Module.Controllers
{
    partial class DoctorEventController
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
            this.CreateDoctorEventAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // CreateDoctorEventAction
            // 
            this.CreateDoctorEventAction.AcceptButtonCaption = null;
            this.CreateDoctorEventAction.CancelButtonCaption = null;
            this.CreateDoctorEventAction.Caption = "Создать расписание";
            this.CreateDoctorEventAction.Category = "ObjectsCreation";
            this.CreateDoctorEventAction.ConfirmationMessage = null;
            this.CreateDoctorEventAction.Id = "Registrator.Module.Controllers.CreateDoctorEventAction";
            this.CreateDoctorEventAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.CreateDoctorEventAction.TargetObjectsCriteria = "Scheduling";
            this.CreateDoctorEventAction.TargetObjectType = typeof(Registrator.Module.BusinessObjects.Doctor);
            this.CreateDoctorEventAction.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.CreateDoctorEventAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.CreateDoctorEventAction.ToolTip = null;
            this.CreateDoctorEventAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.CreateDoctorEventAction.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.CreateDoctorEventAction_CustomizePopupWindowParams);
            this.CreateDoctorEventAction.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.CreateDoctorEventAction_Execute);
            // 
            // DoctorEventController
            // 
            this.Actions.Add(this.CreateDoctorEventAction);
            this.TargetObjectType = typeof(Registrator.Module.BusinessObjects.Doctor);
            this.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.TypeOfView = typeof(DevExpress.ExpressApp.ListView);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CreateDoctorEventAction;
    }
}
