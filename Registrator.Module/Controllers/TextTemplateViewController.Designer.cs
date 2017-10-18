namespace Registrator.Module.Controllers
{
    partial class TextTemplateViewController
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
            this.EditTemplateAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // EditTemplateAction
            // 
            this.EditTemplateAction.Caption = "Редактировать";
            this.EditTemplateAction.Category = "Edit";
            this.EditTemplateAction.ConfirmationMessage = null;
            this.EditTemplateAction.Id = "EditTextTemplateAction";
            this.EditTemplateAction.ToolTip = null;
            this.EditTemplateAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.EditTemplateAction_Execute);
            // 
            // TextTemplateViewController
            // 
            this.TargetObjectType = typeof(Registrator.Module.BusinessObjects.Dictionaries.TextTemplate);
            this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.TypeOfView = typeof(DevExpress.ExpressApp.ListView);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction EditTemplateAction;
    }
}
