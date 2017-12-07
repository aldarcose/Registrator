using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Templates.Ribbon;
using DevExpress.ExpressApp.Xpo;
using System;

namespace Registrator.Win {
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/DevExpressExpressAppWinWinApplicationMembersTopicAll
    public partial class RegistratorWindowsFormsApplication : WinApplication {

        public static WinApplication Instance { get; set; }

        public RegistratorWindowsFormsApplication() {
            InitializeComponent();

            // используем статичное поле для хранения инстанса приложения
            // поле используем в кастомных формах
            Instance = this;
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection, true);
        }

        private void RegistratorWindowsFormsApplication_CustomizeLanguagesList(object sender, CustomizeLanguagesListEventArgs e) {
            string userLanguageName = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            if(userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1) {
                e.Languages.Add(userLanguageName);
            }
        }

        /// <inheritdoc/>
        protected override void OnLoggingOn(LogonEventArgs args)
        {
            base.OnLoggingOn(args);

            AuthenticationStandardLogonParameters logon = args.LogonParameters as AuthenticationStandardLogonParameters;
            if (logon == null)
                return;

            // Пароль по умолчанию в режиме отладки
#if DEBUG
            if (String.IsNullOrEmpty(logon.Password))
            {
                logon.Password = logon.UserName + "123";
            }
#endif
        }

        protected override void OnCustomizeTemplate(DevExpress.ExpressApp.Templates.IFrameTemplate frameTemplate, string templateContextName)
        {
            var mainform = frameTemplate as MainRibbonFormV2;
            if (mainform != null)
            {
                mainform.Ribbon.SelectedPage = mainform.Ribbon.Pages["Home"];
            }
            base.OnCustomizeTemplate(frameTemplate, templateContextName);
        }

        private void RegistratorWindowsFormsApplication_DatabaseVersionMismatch(object sender, DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
            e.Updater.Update();
            e.Handled = true;
#else
            if(System.Diagnostics.Debugger.IsAttached) {e.Updater.Update();
                e.Handled = true;
            }
            else {
                throw new InvalidOperationException(
                    "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application.\r\n" +
                    "This error occurred  because the automatic database update was disabled when the application was started without debugging.\r\n" +
                    "To avoid this error, you should either start the application under Visual Studio in debug mode, or modify the " +
                    "source code of the 'DatabaseVersionMismatch' event handler to enable automatic database update, " +
                    "or manually create a database using the 'DBUpdater' tool.\r\n" +
                    "Anyway, refer to the 'Update Application and Database Versions' help topic at http://www.devexpress.com/Help/?document=ExpressApp/CustomDocument2795.htm " +
                    "for more detailed information. If this doesn't help, please contact our Support Team at http://www.devexpress.com/Support/Center/");
            }
#endif
        }

        private void RegistratorWindowsFormsApplication_SettingUp(object sender, SetupEventArgs e)
        {
           DatabaseUpdateMode = DatabaseUpdateMode.Never;
        }
    }
}
