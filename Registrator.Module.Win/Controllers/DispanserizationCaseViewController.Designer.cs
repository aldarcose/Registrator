namespace Registrator.Module.Win.Controllers
{
    partial class DispanserizationCaseViewController
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
            this.simpleAction1 = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // simpleAction1
            // 
            this.simpleAction1.Caption = "Выгрузить отчет XML";
            this.simpleAction1.Category = "Export";
            this.simpleAction1.ConfirmationMessage = null;
            this.simpleAction1.Id = "183fd87a-5aa0-48aa-8d36-f3aecd0ea835";
            this.simpleAction1.TargetObjectType = typeof(Registrator.Module.BusinessObjects.DispanserizaionCase);
            this.simpleAction1.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.simpleAction1.ToolTip = null;
            this.simpleAction1.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            // 
            // DispanserizationCaseViewController
            // 
            this.Actions.Add(this.simpleAction1);
            this.TargetObjectType = typeof(Registrator.Module.BusinessObjects.Abstract.DispCase);
            this.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction simpleAction1;
    }
}
