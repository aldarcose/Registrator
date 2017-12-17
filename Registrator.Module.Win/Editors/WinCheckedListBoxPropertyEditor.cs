// See
// Also:
// http://www.devexpress.com/scid=S30847
// CheckedListBoxControl Class
// (http://documentation.devexpress.com/#WindowsForms/clsDevExpressXtraEditorsCheckedListBoxControltopic)
// Implement
// Custom Property Editors
// (http://documentation.devexpress.com/#Xaf/CustomDocument3097)
// http://www.devexpress.com/scid=E1806
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E1807

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.XtraEditors;
using DevExpress.Xpo;
using System.Windows.Forms;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Data.Filtering;
using System.Reflection;
using DevExpress.ExpressApp.DC;
//using Aurum.Interface.Model;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.PLinq.Helpers;
using DevExpress.Xpo.DB;
using DevExpress.XtraEditors.Controls;
using ItemCheckEventArgs = DevExpress.XtraEditors.Controls.ItemCheckEventArgs;

namespace Registrator.Module.Win.Editors
{
    /// <summary>
    /// Редактор для коллекций с чек-боксами.
    /// Если объект в редакторе помечен, то объект принадлежит к коллекции.<br/>
    /// Поддерживает XPO коллекции и наследников интерфейса IList.<br/>
    /// Отображаемое значение извлекается из объектов списка в соответствии со значениями модели 
    /// <see cref="IModelPropertyEditorDisplayItem.DisplayItemCriteriaProperty"/> и 
    /// <see cref="IModelPropertyEditorDisplayItem.DisplayItemCriteriaString"/>.<br/>
    /// Список сортируется в соответствии с отображаемым значением.<br/>
    /// Список значений, которые можно выбрать, задаётся в модели через поля 
    /// <see cref="IModelCommonMemberViewItem.DataSourceCriteria"/>, 
    /// <see cref="IModelMemberViewItem.DataSourceCriteriaProperty"/> или
    /// <see cref="IModelMemberViewItem.DataSourceProperty"/>.<br/>
    /// Если свойство, на которое навешан редактор, является XPO коллекцией, 
    /// то используются набор объектов для выбора извлекается из базы данных 
    /// по критерием DataSourceCriteria или DataSourceCriteriaProperty.<br/>
    /// Иначе, если свойство реализует IList, то набор объектов берётся из свойства, 
    /// указанного в DataSourceProperty.<br/>
    /// 
    /// <seealso cref="IModelPropertyEditorDisplayItem"/>
    /// </summary>
    [PropertyEditor(typeof(XPBaseCollection), false)]
    [PropertyEditor(typeof(IList), false)]
    public class WinCheckedListBoxPropertyEditor : WinPropertyEditor, IComplexViewItem
    {
        private XafApplication application;
        private IObjectSpace objectSpace;

        public WinCheckedListBoxPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) { }

        protected override object CreateControlCore()
        {
            return new CustomCheckedListBoxEdit() { };
        }

        protected override void ReadValueCore()
        {
            base.ReadValueCore();
            if (PropertyValue is XPBaseCollection && string.IsNullOrEmpty(Model.DataSourceProperty))
            {
                ReadXpBaseCollection();
            }
            else if (PropertyValue is IList)
            {
                ReadList();
            }
        }

        /// <summary>
        /// Получить выражение для вычисления текста из модели.
        /// </summary>
        /// <returns>Выражение для вычисления текста для отображения в списке.</returns>
        private CriteriaOperator GetDisplayItemCriteria()
        {
            CriteriaOperator result = null;
            if (!ReferenceEquals(result, null))
            {
                var classInfo = application.Model.BOModel.GetClass(MemberInfo.ListElementTypeInfo.Type);
                if (classInfo != null && String.IsNullOrWhiteSpace(classInfo.DefaultProperty))
                {
                    var expressionStr = String.Concat("[", classInfo.DefaultProperty, "]");
                    result = CriteriaOperator.Parse(expressionStr);
                }
            }

            return result;
        }

        private void RemoveAllItemCheckListeners()
        {
            Control.ItemCheck -= list_ItemCheck;
            Control.ItemCheck -= collection_ItemCheck;
        }

        private class CheckedListBoxItemExt : CheckedListBoxItem
        {
            private readonly object original;

            public CheckedListBoxItemExt(object caption, object original, bool isChecked) :
                base(caption, isChecked)
            {
                this.original = original;
            }

            public object Original { get { return original; } }
        }

        private IList CheckedList { get { return (IList)PropertyValue; } }

        private IEnumerable GetCheckedListItems()
        {
            if (!string.IsNullOrEmpty(Model.DataSourceProperty))
            {
                var propWithEnum = MemberInfo.Owner.FindMember(Model.DataSourceProperty);
                if (propWithEnum != null)
                {
                    return (propWithEnum.GetValue(CurrentObject) as IEnumerable);
                }
            }
            return Enumerable.Empty<object>();
        }

        /// <summary>
        /// Заполнить редактор в интерфейсе объектами из указанного свойства.
        /// </summary>
        private void ReadList()
        {
            var displayItemCriteria = GetDisplayItemCriteria();

            Control.Items.Clear();

            var datasource = GetCheckedListItems().Cast<object>().ToList();
            var items = datasource.Select(item =>
            {
                var evaluator = !ReferenceEquals(displayItemCriteria, null)
                    ? objectSpace.GetExpressionEvaluator(item.GetType(), displayItemCriteria)
                    : null;
                var text = evaluator != null ? evaluator.Evaluate(item) : null;
                var isChecked = CheckedList.Contains(item);
                return new CheckedListBoxItemExt(text ?? item, item, isChecked);
            }).OfType<CheckedListBoxItem>().ToArray();

            Array.Sort(items, (x, y) =>
            {
                var a = x.Value != null ? x.Value.ToString() : string.Empty;
                var b = y.Value != null ? y.Value.ToString() : string.Empty;
                return string.Compare(a, b, StringComparison.Ordinal);
            });
            Control.Items.AddRange(items);

            var removables = CheckedList
                .Cast<object>()
                .Where(item => !datasource.Contains(item))
                .ToList();

            foreach (var item in removables)
            {
                CheckedList.Remove(item);
            }

            RemoveAllItemCheckListeners();
            Control.ItemCheck += list_ItemCheck;
        }

        /// <summary>
        /// В пользовательском интерфейсе изменилось checked состояние одного из объектов.<br/>
        /// Добавляем в целевую коллекцию объект, если объект был отмечен, 
        /// иначе удаляем из целевой коллекции.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void list_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var obj = Control.Items[e.Index] as CheckedListBoxItemExt;
            if (obj == null)
            {
                return;
            }

            switch (e.State)
            {
                case CheckState.Checked:
                    CheckedList.Add(obj.Original);
                    break;
                case CheckState.Unchecked:
                    CheckedList.Remove(obj.Original);
                    break;
            }
            OnControlValueChanged();
            objectSpace.SetModified(CurrentObject);
        }

        private XPBaseCollection CheckedCollection { get { return (XPBaseCollection)PropertyValue; } }

        /// <summary>
        /// Получить критерий выборки объектов из базы данных.
        /// </summary>
        /// <returns></returns>
        private CriteriaOperator GetModelCriteria()
        {
            // коллекция данных для списка
            // дополнительные условия
            if (!string.IsNullOrEmpty(Model.DataSourceCriteria))
            {
                return CriteriaOperator.Parse(Model.DataSourceCriteria);
            }
            if (!string.IsNullOrEmpty(Model.DataSourceCriteriaProperty))
            {
                var propWithCriteria = MemberInfo.Owner.FindMember(Model.DataSourceCriteriaProperty);
                return (CriteriaOperator)propWithCriteria.GetValue(CurrentObject);
            }
            return null;
        }

        /// <summary>
        /// Наполнить объектами, извлечёнными из базы данных, список в интерфейсе.
        /// </summary>
        private void ReadXpBaseCollection()
        {
            // отписка от старого обработчика
            RemoveAllItemCheckListeners();

            var criteria = GetModelCriteria();
            criteria = !ReferenceEquals(criteria, null)
                       ? CriteriaOperator.And(CheckedCollection.Criteria, criteria)
                       : CheckedCollection.Criteria;

            var dataSource = new XPCollection(CheckedCollection.Session, MemberInfo.ListElementType,
                criteria, CheckedCollection.Sorting.ToArray());

            var classInfo = application.Model.BOModel.GetClass(MemberInfo.ListElementTypeInfo.Type);
            if (CheckedCollection.Sorting.Count > 0)
            {
                dataSource.Sorting = CheckedCollection.Sorting;
            }
            else if (CheckedCollection.Sorting.Count == 0
                     && !String.IsNullOrEmpty(classInfo.DefaultProperty))
            {
                dataSource.Sorting.Add(new SortProperty(classInfo.DefaultProperty,
                    SortingDirection.Ascending));
            }
            else
            {
                dataSource.Sorting.Add(new SortProperty(
                    CriteriaOperator.Parse("ToStr([This])"), SortingDirection.Ascending));
            }
            Control.DataSource = dataSource;
            Control.DisplayMember = classInfo.DefaultProperty;

            //Выполняем условие для отображения текста в контроле.
            Control.ItemTextCriteria = GetDisplayItemCriteria();

            var removables = CheckedCollection
                .Cast<object>()
                .Where(item => dataSource.IndexOf(item) == -1)
                .ToList();

            foreach (var item in removables)
            {
                CheckedCollection.BaseRemove(item);
            }

            foreach (var obj in CheckedCollection)
            {
                Control.SetItemChecked(dataSource.IndexOf(obj), true);
            }
            Control.ItemCheck += collection_ItemCheck;
        }

        /// <summary>
        /// В пользовательском интерфейсе изменилось checked состояние одного из объектов.<br/>
        /// Добавляем в целевую коллекцию объект, если объект был отмечен, 
        /// иначе удаляем из целевой коллекции.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void collection_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var obj = Control.GetItemValue(e.Index);
            switch (e.State)
            {
                case CheckState.Checked:
                    CheckedCollection.BaseAdd(obj);
                    break;
                case CheckState.Unchecked:
                    CheckedCollection.BaseRemove(obj);
                    break;
            }
            OnControlValueChanged();
            objectSpace.SetModified(CurrentObject);
        }

        public new CustomCheckedListBoxEdit Control
        {
            get { return (CustomCheckedListBoxEdit)base.Control; }
        }

        #region IComplexPropertyEditor Members

        public void Setup(IObjectSpace objectSpace, XafApplication application)
        {
            this.application = application;
            this.objectSpace = objectSpace;
        }

        #endregion
    }
}
