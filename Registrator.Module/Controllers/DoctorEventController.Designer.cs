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
            this.FilterDoctorSpecEventAction = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            this.FilterDoctorEventAction = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            this.CloneDoctorEventAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // FilterDoctorSpecEventAction
            // 
            this.FilterDoctorSpecEventAction.Caption = "Специальность";
            this.FilterDoctorSpecEventAction.Category = "Filters";
            this.FilterDoctorSpecEventAction.ConfirmationMessage = null;
            this.FilterDoctorSpecEventAction.Id = "FilterDoctorSpecEvetAction";
            this.FilterDoctorSpecEventAction.ShowItemsOnClick = true;
            this.FilterDoctorSpecEventAction.TargetObjectType = typeof(Registrator.Module.BusinessObjects.DoctorEvent);
            this.FilterDoctorSpecEventAction.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.FilterDoctorSpecEventAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.FilterDoctorSpecEventAction.ToolTip = null;
            this.FilterDoctorSpecEventAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.FilterDoctorSpecEventAction.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.FilterDoctorSpecEventAction_Execute);
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
            // CloneDoctorEventAction
            // 
            this.CloneDoctorEventAction.AcceptButtonCaption = null;
            this.CloneDoctorEventAction.CancelButtonCaption = null;
            this.CloneDoctorEventAction.Caption = "Скопировать расписание";
            this.CloneDoctorEventAction.Category = "ObjectsCreation";
            this.CloneDoctorEventAction.ConfirmationMessage = null;
            this.CloneDoctorEventAction.Id = "CloneDoctorEventAction";
            this.CloneDoctorEventAction.TargetObjectType = typeof(Registrator.Module.BusinessObjects.DoctorEvent);
            this.CloneDoctorEventAction.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.CloneDoctorEventAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.CloneDoctorEventAction.ToolTip = null;
            this.CloneDoctorEventAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.CloneDoctorEventAction.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.CloneDoctorEventAction_CustomizePopupWindowParams);
            this.CloneDoctorEventAction.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.CloneDoctorEventAction_Execute);
            // 
            // DoctorEventController
            // 
            this.Actions.Add(this.FilterDoctorSpecEventAction);
            this.Actions.Add(this.FilterDoctorEventAction);
            this.Actions.Add(this.CreateDoctorEventAction);
            this.Actions.Add(this.CloneDoctorEventAction);
            this.TargetObjectType = typeof(Registrator.Module.BusinessObjects.DoctorEvent);
            this.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.ViewControlsCreated += new System.EventHandler(this.DoctorEventController_ViewControlsCreated);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SingleChoiceAction FilterDoctorSpecEventAction;
        private DevExpress.ExpressApp.Actions.SingleChoiceAction FilterDoctorEventAction;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CreateDoctorEventAction;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CloneDoctorEventAction;
    }
}
