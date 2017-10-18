namespace Registrator.Module.Win.Controllers
{
    partial class DnevnoyStacionarController
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
            this.ExportDataReestrAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.BindDoctorAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.CloseStacionarAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.FilterForDoctorAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.SetOplata = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.DupsFilter = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.ClearCriteria = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // ExportDataReestrAction
            // 
            this.ExportDataReestrAction.AcceptButtonCaption = "Экспорт";
            this.ExportDataReestrAction.CancelButtonCaption = "Отмена";
            this.ExportDataReestrAction.Caption = "Экспорт реестра";
            this.ExportDataReestrAction.Category = "Export";
            this.ExportDataReestrAction.ConfirmationMessage = null;
            this.ExportDataReestrAction.Id = "0a185e00-6f7c-4fc3-935c-faeb088d8cf0";
            this.ExportDataReestrAction.TargetObjectType = typeof(Registrator.Module.BusinessObjects.DnevnoyStacionar);
            this.ExportDataReestrAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.ExportDataReestrAction.ToolTip = null;
            this.ExportDataReestrAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.ExportDataReestrAction.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.exportDataReestrAction_CustomizePopupWindowParams);
            this.ExportDataReestrAction.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.exportDataReestrAction_Execute);
            // 
            // BindDoctorAction
            // 
            this.BindDoctorAction.AcceptButtonCaption = "Передать";
            this.BindDoctorAction.CancelButtonCaption = "Отмена";
            this.BindDoctorAction.Caption = "Передать пациента";
            this.BindDoctorAction.Category = "Edit";
            this.BindDoctorAction.ConfirmationMessage = "Вы действительно собираетесь передать пациента другому врачу?";
            this.BindDoctorAction.Id = "fba18e4b-a55f-402d-8a87-4091f8418f35";
            this.BindDoctorAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.BindDoctorAction.TargetObjectsCriteria = "";
            this.BindDoctorAction.TargetObjectType = typeof(Registrator.Module.BusinessObjects.DnevnoyStacionar);
            this.BindDoctorAction.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.BindDoctorAction.ToolTip = "Передача пациента другому врачу";
            this.BindDoctorAction.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            this.BindDoctorAction.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.BindDoctorAction_CustomizePopupWindowParams);
            this.BindDoctorAction.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.BindDoctorAction_Execute);
            // 
            // CloseStacionarAction
            // 
            this.CloseStacionarAction.AcceptButtonCaption = "Выписать";
            this.CloseStacionarAction.CancelButtonCaption = "Отмена";
            this.CloseStacionarAction.Caption = "Выписка";
            this.CloseStacionarAction.Category = "Edit";
            this.CloseStacionarAction.ConfirmationMessage = null;
            this.CloseStacionarAction.Id = "21a69eb2-b491-4f5b-802b-8c008c977ad4";
            this.CloseStacionarAction.TargetObjectType = typeof(Registrator.Module.BusinessObjects.DnevnoyStacionar);
            this.CloseStacionarAction.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.CloseStacionarAction.ToolTip = "Выписка пациента";
            this.CloseStacionarAction.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            this.CloseStacionarAction.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.CloseStacionarAction_CustomizePopupWindowParams);
            this.CloseStacionarAction.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.CloseStacionarAction_Execute);
            // 
            // FilterForDoctorAction
            // 
            this.FilterForDoctorAction.AcceptButtonCaption = "Фильтр";
            this.FilterForDoctorAction.CancelButtonCaption = "Отмена";
            this.FilterForDoctorAction.Caption = "Фильтрация";
            this.FilterForDoctorAction.Category = "Search";
            this.FilterForDoctorAction.ConfirmationMessage = null;
            this.FilterForDoctorAction.Id = "1359e438-12d7-400e-a2a7-288a1865d3de";
            this.FilterForDoctorAction.Shortcut = "CtrlF";
            this.FilterForDoctorAction.TargetObjectType = typeof(Registrator.Module.BusinessObjects.DnevnoyStacionar);
            this.FilterForDoctorAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.FilterForDoctorAction.ToolTip = null;
            this.FilterForDoctorAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.FilterForDoctorAction.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.FilterForDoctorAction_CustomizePopupWindowParams);
            this.FilterForDoctorAction.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.FilterForDoctorAction_Execute);
            // 
            // SetOplata
            // 
            this.SetOplata.Caption = "Проставить оплату";
            this.SetOplata.Category = "Edit";
            this.SetOplata.ConfirmationMessage = null;
            this.SetOplata.Id = "16300ba7-c07e-45b5-b03b-e492779f55a5";
            this.SetOplata.TargetObjectType = typeof(Registrator.Module.BusinessObjects.DnevnoyStacionar);
            this.SetOplata.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.SetOplata.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.SetOplata.ToolTip = null;
            this.SetOplata.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.SetOplata.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.SetOplata_Execute);
            // 
            // DupsFilter
            // 
            this.DupsFilter.Caption = "Поиск дублей (врем)";
            this.DupsFilter.Category = "Search";
            this.DupsFilter.ConfirmationMessage = null;
            this.DupsFilter.Id = "777153fc-a81a-48da-a1f6-74b264262836";
            this.DupsFilter.TargetObjectType = typeof(Registrator.Module.BusinessObjects.DnevnoyStacionar);
            this.DupsFilter.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.DupsFilter.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.DupsFilter.ToolTip = null;
            this.DupsFilter.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.DupsFilter.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.DupsFilter_Execute);
            // 
            // ClearCriteria
            // 
            this.ClearCriteria.Caption = "Сбросить фильтры";
            this.ClearCriteria.Category = "Search";
            this.ClearCriteria.ConfirmationMessage = null;
            this.ClearCriteria.Id = "4b53ac50-7a53-48df-8875-3619c447a9cb";
            this.ClearCriteria.TargetObjectType = typeof(Registrator.Module.BusinessObjects.DnevnoyStacionar);
            this.ClearCriteria.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.ClearCriteria.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.ClearCriteria.ToolTip = null;
            this.ClearCriteria.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.ClearCriteria.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.ClearCriteria_Execute);
            // 
            // DnevnoyStacionarController
            // 
            this.Actions.Add(this.ExportDataReestrAction);
            this.Actions.Add(this.BindDoctorAction);
            this.Actions.Add(this.CloseStacionarAction);
            this.Actions.Add(this.FilterForDoctorAction);
            this.Actions.Add(this.SetOplata);
            this.Actions.Add(this.DupsFilter);
            this.Actions.Add(this.ClearCriteria);
            this.TargetObjectType = typeof(Registrator.Module.BusinessObjects.DnevnoyStacionar);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction BindDoctorAction;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CloseStacionarAction;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction ExportDataReestrAction;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction FilterForDoctorAction;
        private DevExpress.ExpressApp.Actions.SimpleAction SetOplata;
        private DevExpress.ExpressApp.Actions.SimpleAction DupsFilter;
        private DevExpress.ExpressApp.Actions.SimpleAction ClearCriteria;
    }
}
