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
            this.FilterDoctorEventAction = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            this.CreateDoctorEventAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // FilterDoctorEventAction
            // 
            this.FilterDoctorEventAction.Caption = "Врач";
            this.FilterDoctorEventAction.Category = "Filters";
            this.FilterDoctorEventAction.ConfirmationMessage = null;
            this.FilterDoctorEventAction.Id = "FilterDoctorEventAction";
            this.FilterDoctorEventAction.ShowItemsOnClick = true;
            this.FilterDoctorEventAction.TargetObjectType = typeof(Registrator.Module.BusinessObjects.DoctorEvent);
            this.FilterDoctorEventAction.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.FilterDoctorEventAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.FilterDoctorEventAction.ToolTip = null;
            this.FilterDoctorEventAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.FilterDoctorEventAction.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.FilterDoctorEventAction_Execute);
            // 
            // CreateDoctorEventAction
            // 
            this.CreateDoctorEventAction.AcceptButtonCaption = null;
            this.CreateDoctorEventAction.CancelButtonCaption = null;
            this.CreateDoctorEventAction.Caption = "Создать расписание";
            this.CreateDoctorEventAction.Category = "ObjectsCreation";
            this.CreateDoctorEventAction.ConfirmationMessage = null;
            this.CreateDoctorEventAction.Id = "CreateDoctorEventAction";
            this.CreateDoctorEventAction.TargetObjectType = typeof(Registrator.Module.BusinessObjects.DoctorEvent);
            this.CreateDoctorEventAction.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.CreateDoctorEventAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.CreateDoctorEventAction.ToolTip = null;
            this.CreateDoctorEventAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.CreateDoctorEventAction.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.CreateDoctorEventAction_CustomizePopupWindowParams);
            this.CreateDoctorEventAction.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.CreateDoctorEventAction_Execute);
            // 
            // DoctorEventController
            // 
            this.Actions.Add(this.FilterDoctorEventAction);
            this.Actions.Add(this.CreateDoctorEventAction);
            this.TargetObjectType = typeof(Registrator.Module.BusinessObjects.DoctorEvent);
            this.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.TypeOfView = typeof(DevExpress.ExpressApp.ListView);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SingleChoiceAction FilterDoctorEventAction;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CreateDoctorEventAction;
    }
}
