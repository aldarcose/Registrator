using System;
using System.Text;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Validation;
using Registrator.Module.BusinessObjects;
using Registrator.Module.BusinessRules;
using Registrator.Module.Reports;
using DevExpress.Utils;
using System.Drawing;
using DevExpress.ExpressApp.Notifications;
using DevExpress.Persistent.Base.General;

namespace Registrator.Module 
{
    public sealed partial class RegistratorModule : ModuleBase 
    {
        static RegistratorModule()
        {
            ModelNodesGeneratorSettings.SetIdPrefix(typeof(DoctorEvent), "Registrator_Event");
        }

        public RegistratorModule() 
        {
            InitializeComponent();
        }

        public override void Setup(XafApplication application) 
        {
            base.Setup(application);

            // Установка шрифтов
            AppearanceObject.DefaultFont = new Font(FontFamily.GenericSansSerif, 10);
            DevExpress.XtraScheduler.SchedulerCompatibility.Base64XmlObjectSerialization = false;
            application.SetupComplete += application_SetupComplete;
            application.LoggedOn += application_LoggedOn;
        }

        void application_SetupComplete(object sender, EventArgs e)
        {
            var module = Application.Modules.FindModule<DevExpress.ExpressApp.Notifications.NotificationsModule>();
            module.DefaultNotificationsProvider = new DefaultNotificationsProvider(base.Application);
            module.DefaultNotificationsProvider.NotificationTypesInfo.Add(XafTypesInfo.Instance.FindTypeInfo(typeof(DoctorEvent)));
        }

        void application_LoggedOn(object sender, LogonEventArgs e)
        {
            NotificationsModule notificationsModule = Application.Modules.FindModule<NotificationsModule>();
            DefaultNotificationsProvider notificationsProvider = notificationsModule.DefaultNotificationsProvider;
            notificationsProvider.CustomizeNotificationCollectionCriteria += notificationsProvider_CustomizeNotificationCollectionCriteria;
        }
        void notificationsProvider_CustomizeNotificationCollectionCriteria(
            object sender, CustomizeCollectionCriteriaEventArgs e)
        {
            //if (e.Type == typeof(Event))
            //    e.Criteria = CriteriaOperator.Parse("AssignedTo.Oid == CurrentUserId()");
        }

        public override void Setup(ApplicationModulesManager moduleManager)
        {
            base.Setup(moduleManager);

            ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(DispServiceListValueRule), typeof(IRuleBaseProperties));
            ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(PacientValueRule), typeof(IRuleBaseProperties));
            ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(PacientDocumentValueRule), typeof(IRuleBaseProperties));
            ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(PacientPolisValueRule), typeof(IRuleBaseProperties));
            ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(DSValueRule), typeof(IRuleBaseProperties));
            ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(EditableProtocolValueRule), typeof(IRuleBaseProperties));
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(
                IObjectSpace objectSpace, Version versionFromDB)
        {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            PredefinedReportsUpdater predefinedReportsUpdater =
                new PredefinedReportsUpdater(Application, objectSpace, versionFromDB);

            predefinedReportsUpdater.AddPredefinedReport<PacientReport>("Pacient Report", typeof(Pacient), typeof(PacientReportParametersObject), true);
            predefinedReportsUpdater.AddPredefinedReport<VisitReport>("Visits Report", typeof(VisitCase), typeof(VisitReportParametersObject), true);

            return new ModuleUpdater[] { updater, predefinedReportsUpdater };
        }
    }
}
