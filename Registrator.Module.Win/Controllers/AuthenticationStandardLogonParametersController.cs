using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Registrator.Module.Win.Controllers
{
    /// <summary>
    /// Контроллер параметров авторизации входа в программу
    /// </summary>
    /// <remarks>Переопределяет фокус на ввод пользователя, если пользователь не определен</remarks>
    public class AuthenticationStandardLogonParametersController : ObjectViewController<DetailView, AuthenticationStandardLogonParameters>
    {
        /// <inheritdoc/>
        protected override void OnActivated()
        {
            base.OnActivated();

            Form form = Frame.Template as Form;
            if (form != null)
            {
                if (form.IsHandleCreated)
                    SetFocus();
                else
                    form.Shown += form_Shown;
            }
        }

        private void form_Shown(object sender, EventArgs e)
        {
            ((Form)sender).Shown -= form_Shown;
            SetFocus();
        }

        /// <summary>
        /// Устанавливает фокус на одном из элементов представления
        /// </summary>
        protected virtual void SetFocus()
        {
            // Установка фокуса на пользователе, если пользователь пустой, иначе на пароле
            AuthenticationStandardLogonParameters current = (AuthenticationStandardLogonParameters)View.CurrentObject;
            ViewItem userName = View.FindItem("UserName");
            ViewItem password = View.FindItem("Password");
            if (userName != null && password != null && current != null)
            {
                Control control = (string.IsNullOrEmpty(current.UserName) ? userName : password).Control as Control;
                if (control != null)
                    control.Focus();
            }
        }
    }
}
