using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;
using System.Windows.Forms;
using DevExpress.Xpo;
using DevExpress.XtraEditors.Controls;

namespace Registrator.Module.Win.Editors
{
    /// <summary>
    /// CheckedListBoxControl с дополнительным CheckBox "Выбрать всех"
    /// и индикацией количества выбранных и общего количества объектов.
    /// </summary>
    public partial class CustomCheckedListBoxEdit : UserControl
    {
        public CustomCheckedListBoxEdit()
        {
            InitializeComponent();

            Items.ListChanged += (sender, args) =>
            {
                var checkedCount = checkedListBox.CheckedItemsCount;
                var totalCount = Items.Count;

                UpdateCheckAll(checkedCount, totalCount);
                UpdateCounters(checkedCount, totalCount);
            };
        }

        private void decimalEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void checkAll_CheckedChanged(object sender, EventArgs e)
        {
            checkedListBox.ItemCheck -= checkedListBox_ItemCheck;
            if (checkAll.CheckState == CheckState.Checked)
            {
                checkedListBox.CheckAll();
            }
            else if (checkAll.CheckState == CheckState.Unchecked)
            {
                checkedListBox.UnCheckAll();
            }

            checkedListBox.ItemCheck += checkedListBox_ItemCheck;

            int checkedCount;
            int totalCount;
            GetCheckedCount(out checkedCount, out totalCount);
            UpdateCounters(checkedCount, totalCount);
        }

        /// <summary>
        /// Источник данных для отображения.
        /// </summary>
        public object DataSource
        {
            get { return checkedListBox.DataSource; }
            set { checkedListBox.DataSource = value; }
        }

        /// <summary>
        /// Коллекция объектов для отображения. <br/>
        /// Используется если не назначен <see cref="DataSource"/>
        /// </summary>
        public CheckedListBoxItemCollection Items
        {
            get { return checkedListBox.Items; }
        }

        /// <summary>
        /// Имя свойства для отображения.
        /// </summary>
        public string DisplayMember
        {
            get { return checkedListBox.DisplayMember; }
            set { checkedListBox.DisplayMember = value; }
        }

        /// <summary>
        /// Выражение для вычисления текстового представления.<br/>
        /// Если выражение не равно <c>null</c>, 
        /// то оно имеет приоритет перед <see cref="DisplayMember"/>
        /// </summary>
        public CriteriaOperator ItemTextCriteria
        {
            get { return checkedListBox.ItemTextCriteria; }
            set { checkedListBox.ItemTextCriteria = value; }
        }

        /// <summary>
        /// Задать значение отметки для указанного объекта.
        /// </summary>
        /// <param name="index">Индекс объекта.</param>
        /// <param name="value">true, если объект должен быть отмечен, иначе false.</param>
        public void SetItemChecked(int index, bool value)
        {
            checkedListBox.SetItemChecked(index, value);
        }

        /// <summary>
        /// Получить объект по индексу.
        /// </summary>
        /// <param name="index">Индекс объекта.</param>
        /// <returns>Объект с индексом index.</returns>
        public object GetItemValue(int index)
        {
            return checkedListBox.GetItemValue(index);
        }

        /// <summary>
        /// Событие срабатывает при изменении checked состояния какого-либо объекта из списка.
        /// </summary>
        public event DevExpress.XtraEditors.Controls.ItemCheckEventHandler ItemCheck
        {
            add { checkedListBox.ItemCheck += value; }
            remove { checkedListBox.ItemCheck -= value; }
        }

        private void checkedListBox_CheckMemberChanged(object sender, EventArgs e)
        {
        }

        private void checkedListBox_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
        {
            int checkedCount;
            int totalCount;
            GetCheckedCount(out checkedCount, out totalCount);

            UpdateCheckAll(checkedCount, totalCount);
            UpdateCounters(checkedCount, totalCount);
        }

        private void GetCheckedCount(out int checkedCount, out int totalCount)
        {
            var dataSource = checkedListBox.DataSource as XPCollection;

            checkedCount = checkedListBox.CheckedItemsCount;
            totalCount = dataSource != null && checkedListBox.CheckedItems != null
                ? dataSource.Count
                : Items.Count;
        }

        private void UpdateCheckAll(int checkedCount, int totalCount)
        {
            checkAll.CheckedChanged -= checkAll_CheckedChanged;
            checkAll.CheckState =
                checkedCount == 0 ? CheckState.Unchecked :
                totalCount == checkedCount ? CheckState.Checked
                                           : CheckState.Indeterminate;
            checkAll.CheckedChanged += checkAll_CheckedChanged;
        }

        /// <summary>
        /// Обновить индикатор отмеченных объектов и объектов всего.
        /// </summary>
        /// <param name="checkedCount">Количество отмеченных объектов.</param>
        /// <param name="totalCount">Количество объектов всего.</param>
        private void UpdateCounters(int checkedCount, int totalCount)
        {
            countersLabel.Text = string.Format("{0}/{1}", checkedCount, totalCount);
        }
    }
}
