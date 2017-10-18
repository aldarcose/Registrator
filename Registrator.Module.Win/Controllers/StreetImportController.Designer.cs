namespace Registrator.Module.Win.Controllers
{
    partial class StreetImportController
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
            this.LoadStreetAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // LoadStreetAction
            // 
            this.LoadStreetAction.Caption = "Загрузка улиц КЛАДР";
            this.LoadStreetAction.ConfirmationMessage = null;
            this.LoadStreetAction.Id = "StreetLoaderAction";
            this.LoadStreetAction.TargetObjectType = typeof(Registrator.Module.BusinessObjects.Dictionaries.Street);
            this.LoadStreetAction.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.LoadStreetAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.LoadStreetAction.ToolTip = null;
            this.LoadStreetAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.LoadStreetAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.LoadStreetAction_Execute);
            // 
            // StreetImportController
            // 
            this.TargetObjectType = typeof(Registrator.Module.BusinessObjects.Dictionaries.Street);
            this.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.TypeOfView = typeof(DevExpress.ExpressApp.ListView);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction LoadStreetAction;
    }
}
