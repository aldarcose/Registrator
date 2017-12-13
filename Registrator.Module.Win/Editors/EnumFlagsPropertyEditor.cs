using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.ExpressApp.Win.Editors;

namespace Registrator.Module.Win.Editors
{
    /// <summary>
    /// Редактор свойства типа Enum с атрибутом Flags
    /// </summary>
    /// <remarks>Source: <![CDATA[https://www.devexpress.com/Support/Center/Example/Details/E689]]></remarks>
    /// <todo>Баги: при выходе из представления предлагает сохранить изменения (?); 
    /// при выборе нескольких значений (несколько битов enum) и повторном открытии представлении выдает ошибку (похоже на парсинг сохраненного фильтра)</todo>
    /// <todo>Фильтр колонки: все указанные значения, любое из указанных
    /// <![CDATA[https://www.devexpress.com/Support/Center/Question/Details/Q551557]]></todo>
    [PropertyEditor(typeof(System.Enum), false)]
    public class EnumFlagsPropertyEditor : EnumPropertyEditor
    {
        private object noneValue;
        private EnumDescriptor enumDescriptorCore = null;

        public EnumFlagsPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {
        }

        protected EnumDescriptor EnumDescriptor
        {
            get
            {
                if (enumDescriptorCore == null)
                {
                    enumDescriptorCore = new EnumDescriptor(GetUnderlyingType());
                }
                return enumDescriptorCore;
            }
        }

        private bool TypeHasFlagsAttribute()
        {
            return GetUnderlyingType().GetCustomAttributes(typeof(FlagsAttribute), true).Length > 0;
        }

        protected override object CreateControlCore()
        {
            CheckedComboBoxEdit checkedEdit = new CheckedComboBoxEdit();
            if (TypeHasFlagsAttribute())
            {
                return checkedEdit;
            }
            return base.CreateControlCore();
        }

        protected override RepositoryItem CreateRepositoryItem()
        {
            if (TypeHasFlagsAttribute())
            {
                return new RepositoryItemCheckedComboBoxEdit();
            }
            return base.CreateRepositoryItem();
        }

        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            base.SetupRepositoryItem(item);
            if (TypeHasFlagsAttribute())
            {
                RepositoryItemCheckedComboBoxEdit checkedItem = ((RepositoryItemCheckedComboBoxEdit)item);
                checkedItem.BeginUpdate();
                checkedItem.Items.Clear();
                noneValue = GetNoneValue();
                //checkedItem.SelectAllItemVisible = false;
                //Dennis: this is required to show localized items in the editor.
                foreach (object value in EnumDescriptor.Values)
                {
                    if (!IsNoneValue(value))
                    {
                        checkedItem.Items.Add(value, EnumDescriptor.GetCaption(value), CheckState.Unchecked, true);
                    }
                }
                //Dennis: use this method if you don't to show localized items in the editor.
                //checkedItem.SetFlags(GetUnderlyingType());
                checkedItem.EndUpdate();
                checkedItem.ParseEditValue += checkedEdit_ParseEditValue;
                checkedItem.CustomDisplayText += checkedItem_CustomDisplayText;
                checkedItem.Disposed += checkedItem_Disposed;
            }
        }

        void checkedItem_Disposed(object sender, EventArgs e)
        {
            RepositoryItemCheckedComboBoxEdit checkedItem = (RepositoryItemCheckedComboBoxEdit)sender;
            checkedItem.ParseEditValue -= checkedEdit_ParseEditValue;
            checkedItem.CustomDisplayText -= checkedItem_CustomDisplayText;
            checkedItem.Disposed -= checkedItem_Disposed;
        }

        private void checkedEdit_ParseEditValue(object sender, ConvertEditValueEventArgs e)
        {
            if (string.IsNullOrEmpty(Convert.ToString(e.Value)))
            {
                ((CheckedComboBoxEdit)sender).EditValue = noneValue;
                e.Handled = true;
            }
        }

        private void checkedItem_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            if (!IsNoneValue(e.Value))
            {
                return;
            }
            e.DisplayText = EnumDescriptor.GetCaption(e.Value);
        }

        private bool IsNoneValue(object value)
        {
            if (value is string)
            {
                return false;
            }
            int result = int.MinValue;
            try
            {
                result = Convert.ToInt32(value);
            }
            catch { }
            return 0.Equals(result);
        }

        private object GetNoneValue()
        {
            return Enum.ToObject(GetUnderlyingType(), 0);
        }
    }
}
