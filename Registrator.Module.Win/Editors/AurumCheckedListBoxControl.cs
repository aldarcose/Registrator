using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.Module.Win.Editors
{
    public class AurumCheckedListBoxControl : CheckedListBoxControl
    {
        /// <summary>
        /// Выражение, по которому вычисляется текст для отображения элемента
        /// </summary>
        public CriteriaOperator ItemTextCriteria { get; set; }

        private string GetItemText(XPBaseObject xpObject)
        {
            if (xpObject == null)
            {
                return null;
            }

            if (!ReferenceEquals(ItemTextCriteria, null))
            {
                var result = xpObject.Evaluate(ItemTextCriteria);
                return Convert.ToString(result);
            }
            return null;
        }

        /// <summary>
        /// Текстовое представление элемента. 
        /// Если установлено <see cref="ItemTextCriteria"/>, 
        /// то текстовое представление вычисляется по этому выражению,
        /// иначе возвращается значение из базового класса.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override string GetItemText(int index)
        {
            var obj = base.GetItem(index);
            var xpObject = obj as XPBaseObject;
            return GetItemText(xpObject) ?? base.GetItemText(index);
        }
    }
}
