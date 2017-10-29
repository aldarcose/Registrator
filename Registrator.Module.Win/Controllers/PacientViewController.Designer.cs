namespace Registrator.Module.Win.Controllers
{
    partial class PacientViewController
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
            this.pacientFilterAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.LoadNewSRZAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.GetPacientPoliciesInfo = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.CopyAddressToFactAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.CopyAddressFromFactAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // pacientFilterAction
            // 
            this.pacientFilterAction.AcceptButtonCaption = "Искать";
            this.pacientFilterAction.CancelButtonCaption = "Отмена";
            this.pacientFilterAction.Caption = "Поиск пациента";
            this.pacientFilterAction.Category = "FullTextSearch";
            this.pacientFilterAction.ConfirmationMessage = null;
            this.pacientFilterAction.Id = "db7789e8-afce-4f58-ab06-ccf73650a7cc";
            this.pacientFilterAction.Shortcut = "CtrlF";
            this.pacientFilterAction.TargetObjectType = typeof(Registrator.Module.BusinessObjects.Pacient);
            this.pacientFilterAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.pacientFilterAction.ToolTip = null;
            this.pacientFilterAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.pacientFilterAction.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.pacientFilterAction_CustomizePopupWindowParams);
            this.pacientFilterAction.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.pacientFilterAction_Execute);
            // 
            // LoadNewSRZAction
            // 
            this.LoadNewSRZAction.Caption = "Загрузить новые данные";
            this.LoadNewSRZAction.Category = "Edit";
            this.LoadNewSRZAction.ConfirmationMessage = null;
            this.LoadNewSRZAction.Id = "5f28335e-2fa0-4d1e-824a-bfc0023558b4";
            this.LoadNewSRZAction.TargetObjectType = typeof(Registrator.Module.BusinessObjects.Pacient);
            this.LoadNewSRZAction.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.LoadNewSRZAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.LoadNewSRZAction.ToolTip = null;
            this.LoadNewSRZAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.LoadNewSRZAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.LoadNewSRZAction_Execute);
            // 
            // GetPacientPoliciesInfo
            // 
            this.GetPacientPoliciesInfo.Caption = "Получить информацию по полисам от ТФОМС";
            this.GetPacientPoliciesInfo.Category = "Edit";
            this.GetPacientPoliciesInfo.ConfirmationMessage = null;
            this.GetPacientPoliciesInfo.Id = "0b536aaf-1412-4f38-a4c9-e4d63e1fadf3";
            this.GetPacientPoliciesInfo.TargetObjectType = typeof(Registrator.Module.BusinessObjects.Pacient);
            this.GetPacientPoliciesInfo.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.GetPacientPoliciesInfo.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.GetPacientPoliciesInfo.ToolTip = null;
            this.GetPacientPoliciesInfo.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            this.GetPacientPoliciesInfo.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.GetPacientPoliciesInfo_Execute);
            // 
            // CopyAddressToFactAction
            // 
            this.CopyAddressToFactAction.Caption = "Скопировать в адрес проживания";
            this.CopyAddressToFactAction.Category = "CopyAddressToFactCategory";
            this.CopyAddressToFactAction.ConfirmationMessage = "Вы собираетесь установить значения адреса прописки для адреса проживания.\r\nПродол" +
    "жить?";
            this.CopyAddressToFactAction.Id = "Registrator.Module.Win.Controllers.CopyAddressToFactAction";
            this.CopyAddressToFactAction.TargetObjectType = typeof(Registrator.Module.BusinessObjects.Pacient);
            this.CopyAddressToFactAction.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.CopyAddressToFactAction.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.CopyAddressToFactAction.ToolTip = null;
            this.CopyAddressToFactAction.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            this.CopyAddressToFactAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.CopyAddressToFactAction_Execute);
            // 
            // CopyAddressFromFactAction
            // 
            this.CopyAddressFromFactAction.Caption = "Скопировать в адрес прописки";
            this.CopyAddressFromFactAction.Category = "CopyAddressFromFactCategory";
            this.CopyAddressFromFactAction.ConfirmationMessage = "Вы собираетесь установить значения адреса проживания для адреса прописки.\r\nПродол" +
    "жить?";
            this.CopyAddressFromFactAction.Id = "Registrator.Module.Win.Controllers.CopyAddressFromFactAction";
            this.CopyAddressFromFactAction.TargetObjectType = typeof(Registrator.Module.BusinessObjects.Pacient);
            this.CopyAddressFromFactAction.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.CopyAddressFromFactAction.ToolTip = null;
            this.CopyAddressFromFactAction.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            this.CopyAddressFromFactAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.CopyAddressFromFactAction_Execute);
            // 
            // PacientViewController
            // 
            this.Actions.Add(this.pacientFilterAction);
            this.Actions.Add(this.LoadNewSRZAction);
            this.Actions.Add(this.GetPacientPoliciesInfo);
            this.Actions.Add(this.CopyAddressToFactAction);
            this.Actions.Add(this.CopyAddressFromFactAction);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction pacientFilterAction;
        private DevExpress.ExpressApp.Actions.SimpleAction LoadNewSRZAction;
        private DevExpress.ExpressApp.Actions.SimpleAction GetPacientPoliciesInfo;
        private DevExpress.ExpressApp.Actions.SimpleAction CopyAddressToFactAction;
        private DevExpress.ExpressApp.Actions.SimpleAction CopyAddressFromFactAction;
    }
}
