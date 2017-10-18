namespace Registrator.Module.Controllers
{
    partial class CallCardUrgentViewController
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
            this.selectRegAdressAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // selectRegAdressAction
            // 
            this.selectRegAdressAction.AcceptButtonCaption = null;
            this.selectRegAdressAction.CancelButtonCaption = null;
            this.selectRegAdressAction.Caption = "select Reg Adress Action";
            this.selectRegAdressAction.Category = "OpenStreetSelectorCategory";
            this.selectRegAdressAction.ConfirmationMessage = null;
            this.selectRegAdressAction.Id = "selectRegAdressAction";
            this.selectRegAdressAction.ToolTip = null;
            this.selectRegAdressAction.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.selectRegAdressAction_CustomizePopupWindowParams);
            this.selectRegAdressAction.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.selectRegAdressAction_Execute);
            this.selectRegAdressAction.Cancel += new System.EventHandler(this.selectRegAdressAction_Cancel);
            // 
            // CallCardUrgentViewController
            // 
            this.Actions.Add(this.selectRegAdressAction);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction selectRegAdressAction;

    }
}
