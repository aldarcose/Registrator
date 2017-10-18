namespace Registrator.Module.Win.Controllers
{
    partial class StreetSelectorController
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
            this.openSelectorAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.openFactSelectorAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // openSelectorAction
            // 
            this.openSelectorAction.AcceptButtonCaption = "Сохранить";
            this.openSelectorAction.CancelButtonCaption = "Отменить";
            this.openSelectorAction.Caption = "Адрес регистрации";
            this.openSelectorAction.Category = "OpenStreetSelectorCategory";
            this.openSelectorAction.ConfirmationMessage = null;
            this.openSelectorAction.Id = "openSelectorAction";
            this.openSelectorAction.ToolTip = null;
            this.openSelectorAction.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.openSelectorAction_CustomizePopupWindowParams);
            this.openSelectorAction.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.openSelectorAction_Execute);
            this.openSelectorAction.Cancel += new System.EventHandler(this.openSelectorAction_Cancel);
            // 
            // openFactSelectorAction
            // 
            this.openFactSelectorAction.AcceptButtonCaption = "Сохранить";
            this.openFactSelectorAction.CancelButtonCaption = "Отменить";
            this.openFactSelectorAction.Caption = "Адрес проживания";
            this.openFactSelectorAction.Category = "OpenFactStreetSelectorCategory";
            this.openFactSelectorAction.ConfirmationMessage = null;
            this.openFactSelectorAction.Id = "openFactSelectorAction";
            this.openFactSelectorAction.ToolTip = null;
            this.openFactSelectorAction.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.openFactSelectorAction_CustomizePopupWindowParams);
            this.openFactSelectorAction.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.openFactSelectorAction_Execute);
            this.openFactSelectorAction.Cancel += new System.EventHandler(this.openFactSelectorAction_Cancel);
            // 
            // StreetSelectorController
            // 
            this.Actions.Add(this.openSelectorAction);
            this.Actions.Add(this.openFactSelectorAction);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction openSelectorAction;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction openFactSelectorAction;

    }
}
