using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using Registrator.Module.BusinessObjects;

namespace Registrator.Module.Win
{
    [PropertyEditor(typeof(Address), false)]
    public class AddressPropertyEditor : WinPropertyEditor
    {
        public AddressPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model)
        {
            var propertyType = model.ModelMember.Type;
            var validTypes = new List<Type>{
                typeof(Address) 
            };
            if (!validTypes.Contains(propertyType))
                throw new Exception("Can't use AddressPropertyEditor with property type " + propertyType.FullName);
            this.ControlBindingProperty = "TestAddress";
        }

        private AddressControl _control;
        private ButtonEdit _btnButton;

        protected override object CreateControlCore()
        {
            _control = new AddressControl();

            _btnButton = new ButtonEdit();
            //_btnButton.Text = this.ControlValue.ToString();
            _btnButton.ButtonClick += _button_ButtonClick;
            return _btnButton;
        }

        void _button_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            var form = new AddressSelectorForm(this.ControlValue as Address);
            if (form.ShowDialog() == DialogResult.OK)
            {
                
            }
        }

        protected override void Dispose(bool disposing)
        {
            _btnButton.ButtonClick -= _button_ButtonClick;
            base.Dispose(disposing);
        }
    }
}
