using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using Registrator.Module.BusinessObjects;
using Registrator.Module.BusinessObjects.Dictionaries;
using System;
using Registrator.Module.BusinessObjects.Settings;
using ListView = DevExpress.ExpressApp.ListView;
using RepositoryItemTimeSpanEdit = DevExpress.ExpressApp.Win.Editors.RepositoryItemTimeSpanEdit;

namespace Registrator.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class ProtocolRecordViewController : ViewController
    {
        private SimpleAction collapseAction;
        private SimpleAction expandAction;

        public ProtocolRecordViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.

            var listView = View as ListView;
            if (listView != null)
            {
                // скрываем дефолтные экшены
                Frame.GetController<NewObjectViewController>().Active.SetItemValue("EnabledNewAction", false);
                Frame.GetController<DeleteObjectsViewController>().Active.SetItemValue("EnabledDeleteAction", false);
                Frame.GetController<LinkUnlinkController>().Active.SetItemValue("EnabledLinkAction", false);

                // обработчик двойного нажатия на элемент списка
                var listViewProcessCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();

                if (listViewProcessCurrentObjectController != null)
                {
                    string EnabledKeyShowDetailView = "ShowDetailViewFromListViewController";
                    listViewProcessCurrentObjectController.ProcessCurrentObjectAction.Enabled[EnabledKeyShowDetailView] = false;
                }

                //скрыть кнопку "показать объект"
                var openObjectController = Frame.GetController<OpenObjectController>();
                if (openObjectController != null)
                {
                    openObjectController.Active.SetItemValue("EnititesListViewController", false);  // всегда скрывать
                }
            }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.

            TreeListCustomBehavior();

            ApplyPacientFilter();
        }

        private void ApplyPacientFilter()
        {
             var lv = View as ListView;

            if (lv != null)
            {
                if (treeList != null)
                {
                    var service = lv.CurrentObject;

                    foreach (TreeListNode treeListNode in treeList.Nodes)
                    {
                        var record = treeListNode.Tag as ProtocolRecord;
                        if (record != null)
                        {
                            
                        }
                    }
                }
            }
        }

        private ObjectTreeList treeList = null;

        private void TreeListCustomBehavior()
        {
            var lv = View as ListView;

            if (lv != null)
            {
                var treeListEditor = lv.Editor as TreeListEditor;

                if (treeListEditor != null)
                {
                    treeListEditor.AllowEdit = true;

                    treeList = (ObjectTreeList)treeListEditor.TreeList;

                    foreach (RepositoryItem ri in treeList.RepositoryItems)
                    {
                        ri.ReadOnly = false;
                    }

                    foreach (var columnWrapper in treeListEditor.Columns)
                    {
                        if (columnWrapper is TreeListColumnWrapper)
                        {
                            var modelColumn = lv.Model.Columns[columnWrapper.PropertyName];
                            if (modelColumn != null)
                                ((TreeListColumnWrapper)columnWrapper).Column.OptionsColumn.AllowEdit = modelColumn.AllowEdit;
                        }
                    }

                    treeList.CustomNodeCellEdit += TreeListOnCustomNodeCellEdit;
                    treeList.ShownEditor += TreeListOnShownEditor;
                    treeList.CellValueChanged += treeList_CellValueChanged;
                    //treeList.CellValueChanging += treeList_CellValueChanging;
                    treeList.FilterNode += treeList_FilterNode;
                    treeList.OptionsBehavior.Editable = true;
                    treeList.OptionsBehavior.ImmediateEditor = true;
                    treeList.OptionsBehavior.EnableFiltering = true;

                    // отображаем бордер
                    treeList.CustomDrawNodeCell += treeList_CustomDrawNodeCell;

                }
            }
        }

        void treeList_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {
            var record = e.Node.Tag as ProtocolRecord;
            if (e.EditPainter!=null)
            {
                e.Appearance.FillRectangle(e.Cache, e.Bounds);
                DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs c = new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(e.EditViewInfo, e.Cache, e.EditViewInfo.Bounds);
                e.EditPainter.Draw(c);

                var commonPen = Pens.LightBlue;
                var requiredPen = Pens.Red;
                var donePen = Pens.Green;
                if (!e.Column.Name.Equals("Type") && record.Type.Type != TypeEnum.Complex)
                {
                    var temp = record.Type;
                    while (temp._parent!=null)
                    {
                        temp = record.Type._parent;
                    }
                    var required = temp.IsRequired;

                    var hasValue = !string.IsNullOrEmpty(e.CellText);

                    e.Graphics.DrawRectangle(required ? (hasValue ? donePen : requiredPen) : commonPen, e.EditViewInfo.BorderRect);
                }
                else
                {
                    e.Graphics.DrawRectangle(commonPen, e.EditViewInfo.BorderRect);
                }
                
                e.Handled = true;
            }
        }

        void treeList_FilterNode(object sender, FilterNodeEventArgs e)
        {
            if (e.Node.ParentNode != null)
            {
                var entity = e.Node.Tag as ProtocolRecord;
                var parentEntity = entity._parent;
                if (string.IsNullOrEmpty(entity.Type.Criteria) == false)
                {
                    string[] hideCriterias = entity.Type.Criteria.Split(';');

                    var hide = true;
                    foreach (var criteria in hideCriterias)
                    {
                        // проверяем каждый критерий
                        if (checkCriteria(criteria, parentEntity) == false)
                        {
                            hide = false;
                            break;
                        }
                    }
                    e.Node.Visible = !hide;
                    e.Handled = true;
                }
            }
        }

        private bool checkCriteria(string criteria, ProtocolRecord parentEntity)
        {
            string name = getFieldFromCriteria(criteria);
            string value = getValueFromCriteria(criteria);
            bool checkIsEqual = !criteria.Contains("!");
            return parentEntity._children.Any(
                t => t.Type.Name == name && (checkIsEqual ? t.Value == value : t.Value != value));

        }

        public string getFieldFromCriteria(string criteria)
        {
            int index = criteria.IndexOf("=");
            if (index == -1)
            {
                throw new UserFriendlyException("Неверный критерий скрытия для поля протокола. Обратитесь к администратору с указанием услуги. которую вы пытались оказать.");
            }

            if (criteria[index - 1] == '!')
                index --;

            return criteria.Substring(0, index).Trim();
        }

        public string getValueFromCriteria(string criteria)
        {
            int index = criteria.IndexOf("=");
            if (index == -1)
            {
                throw new UserFriendlyException("Неверный критерий скрытия для поля протокола. Обратитесь к администратору с указанием услуги. которую вы пытались оказать.");
            }

            return criteria.Substring(index + 1).Trim();
        }

        private void TreeListOnShownEditor(object sender, EventArgs eventArgs)
        {
            IGridInplaceEdit activeEditor = treeList.ActiveEditor as IGridInplaceEdit;
            if (activeEditor != null && treeList.FocusedObject is IXPSimpleObject)
            {
                activeEditor.GridEditingObject = treeList.FocusedObject;
            }

            var currentObject = treeList.FocusedObject as ProtocolRecord;
            if (currentObject != null)
            {
                var type = currentObject.Type;
                if (type.Type == TypeEnum.Complex)
                {
                    treeList.HideEditor(); //скрыть редактор для комплексных типов
                }
                if (type.Type == TypeEnum.Address)
                {
                    (Application as WinApplication).StartSplash();

                    // показываем окно выбора адреса
                    ShowViewParameters svp = new ShowViewParameters();
                    var os = Application.CreateObjectSpace();
                    var asta = new AddressSelector(os, currentObject.Value);
                    DetailView dv = Application.CreateDetailView(os, asta);
                    svp.CreatedView = dv;
                    svp.TargetWindow = TargetWindow.NewModalWindow;

                    var dc = Application.CreateController<DialogController>();
                    dc.Tag = currentObject;
                    dc.CancelAction.Caption = "Отмена";
                    dc.Accepting += dc_Accepting;
                    svp.Controllers.Add(dc);

                    dv.Closing += dv_Closing;
                    
                    dv.ControlsCreated += (o, e) =>
                    {
                        (Application as WinApplication).StopSplash();
                    };
                    
                    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                }
                if (type.Type == TypeEnum.List)
                {
                    var checkedComboEditor = treeList.ActiveEditor as CheckedComboBoxEdit;
                    
                    if (checkedComboEditor!=null && checkedComboEditor.IsEditorActive)
                    {
                        if (checkedComboEditor.IsPopupOpen == false)
                            checkedComboEditor.ShowPopup();
                    }

                    var comboEditor = treeList.ActiveEditor as ComboBoxEdit;
                    if (comboEditor != null && comboEditor.IsEditorActive)
                    {
                        if (comboEditor.IsPopupOpen ==false)
                            comboEditor.ShowPopup();
                    }
                }
            }
        }

        private void dv_Closing(object sender, EventArgs e)
        {
            
        }

        private void dc_Accepting(object sender, DialogControllerAcceptingEventArgs e)
        {
            var address = e.AcceptActionArgs.CurrentObject as AddressSelector;
            var protocol = (sender as DialogController).Tag as ProtocolRecord;

            protocol.Value = address.ToString();

            ObjectSpace.SetModified(protocol);
        }

        void treeList_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            var protocolRecord = (sender as ObjectTreeList).FocusedObject as ProtocolRecord;
            if (protocolRecord != null)
            {
                object newValue = e.Value;
                if (e.Value is IXPSimpleObject)
                    newValue = ObjectSpace.GetObject(e.Value);
                
                if (protocolRecord.Type.Type == TypeEnum.Date)
                {
                    if (e.Value is DateTime)
                        newValue = ((DateTime) e.Value).ToString("dd.MM.yyyy");
                }

                if (protocolRecord.Type.Type == TypeEnum.Time)
                {
                    if (e.Value is DateTime)
                        newValue = ((DateTime)e.Value).TimeOfDay.ToString(@"hh\:mm\:ss");
                }

                object focusedObject = treeList.FocusedObject;
                if (focusedObject != null)
                {
                    IMemberInfo focusedColumnMemberInfo =
                        ObjectSpace.TypesInfo.FindTypeInfo(focusedObject.GetType()).FindMember(e.Column.FieldName);
                    if (focusedColumnMemberInfo != null)
                    {
                        focusedColumnMemberInfo.SetValue(focusedObject,
                            Convert.ChangeType(newValue, focusedColumnMemberInfo.MemberType));
                    }
                    ObjectSpace.SetModified(focusedObject);
                }

                treeList.FilterNodes();
            }
        }

        private void TreeListOnCustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs eArgs)
        {
            if (eArgs.Column.FieldName!="Value") return;
            if (eArgs.Node.Selected && eArgs.Node.Focused)
            {
                var entity = eArgs.Node.Tag as ProtocolRecord;
                if (entity != null)
                {
                    var recordType = entity.Type;
                    switch (recordType.Type)
                    {
                        case TypeEnum.Bool:
                            eArgs.RepositoryItem = new RepositoryItemCheckEdit();
                            break;
                        case TypeEnum.Integer:
                            eArgs.RepositoryItem = new RepositoryItemSpinEdit()
                            {
                                IsFloatValue = false
                            };
                            break;
                        case TypeEnum.Double:
                            eArgs.RepositoryItem = new RepositoryItemDoubleEdit();
                            break;
                        case TypeEnum.Decimal:
                            eArgs.RepositoryItem = new RepositoryItemDecimalEdit();
                            break;
                        case TypeEnum.Date:
                            eArgs.RepositoryItem = new RepositoryItemDateEdit()
                            {
                                CalendarTimeEditing = DefaultBoolean.False
                            };
                            break;
                        case TypeEnum.Time:
                            eArgs.RepositoryItem = new RepositoryItemTimeEdit()
                            {
                            };
                            break;
                        case TypeEnum.String:
                            eArgs.RepositoryItem = new RepositoryItemTextEdit();
                            break;
                        case TypeEnum.List:
                            RepositoryItem ri = null;
                            if (recordType.ListValues != null || recordType.ListValues.Count > 0)
                            {
                                if (recordType.IsMultipleChoiceList)
                                {
                                    ri = new RepositoryItemCheckedComboBoxEdit();
                                    
                                    foreach (var stringValue in recordType.ListValues)
                                    {
                                        ((RepositoryItemCheckedComboBoxEdit)ri).Items.Add(stringValue.Value);
                                    }
                                }
                                else
                                {
                                    ri = new RepositoryItemComboBox();
                                    
                                    foreach (var stringValue in recordType.ListValues)
                                    {
                                        ((RepositoryItemComboBox)ri).Items.Add(stringValue.Value);
                                    }
                                }
                            }

                            eArgs.RepositoryItem = ri;
                            break;
                        case TypeEnum.Address:
                            break;
                        case TypeEnum.Complex:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                
            }
        }

        protected override void OnDeactivated()
        {

            if (treeList != null)
            {
                treeList.CustomNodeCellEdit -= TreeListOnCustomNodeCellEdit;
                treeList.ShownEditor -= TreeListOnShownEditor;
                treeList.CellValueChanged -= treeList_CellValueChanged;
                treeList.CustomDrawNodeCell -= treeList_CustomDrawNodeCell;
            }

            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void collapseAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            treeList.CollapseAll();
        }
        private void expandAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            treeList.ExpandAll();
        }
    }

    [DomainComponent]
    [XafDisplayName("Выбор адреса")]
    public class AddressSelector : Address
    {
        public AddressSelector(Session session) : base(session) { }
        public AddressSelector(IObjectSpace os, string address) : base(((XPObjectSpace)os).Session)
        {
            var code = RegionSettings.RegionCode(Session);
            this.Level1 = Session.FindObject<Kladr>(CriteriaOperator.Parse("CodeSignificantChars=?", code));

            var cityCode = "81401000000"; // улан-удэ
            this.Level3 = Session.FindObject<Kladr>(CriteriaOperator.Parse("CodeOkato=?", cityCode));

            GetValues(address);
        }

        private void GetValues(string address)
        {
            const string housePattern = @"д. (\w+)";
            const string buildPattern = @"стр. (\w+)";
            const string flatPattern = @"кв. (\w+)";
            int endStreetIndex = -1;
            var houseMatch = Regex.Match(address, housePattern);
            if (houseMatch.Success)
            { 
                this.House = houseMatch.Groups[1].Value;

                endStreetIndex = houseMatch.Index;
            }

            var buildMatch = Regex.Match(address, buildPattern);
            if (buildMatch.Success)
            {
                this.Build = buildMatch.Groups[1].Value;
                if (endStreetIndex == -1)
                    endStreetIndex = buildMatch.Index;
            }

            var flatMatch = Regex.Match(address, flatPattern);
            if (flatMatch.Success)
            {
                this.Flat = flatMatch.Groups[1].Value;
                if (endStreetIndex == -1)
                    endStreetIndex = flatMatch.Index;
            }

            int startStreetIndex = 0;
            if (address.Contains(", " /*<- nbsp*/))
            {
                int index = address.IndexOf(", ", System.StringComparison.Ordinal);
                var preCity = address.Substring(0, index);
                this.Level4 =
                    Session.FindObject<Kladr>(CriteriaOperator.And(CriteriaOperator.Parse("Name=?", preCity),
                        CriteriaOperator.Parse("Parent=?", this.Level3)));

                startStreetIndex = index + 2;
            }

            if (endStreetIndex != -1 && endStreetIndex > 0)
            {
                var index = address.IndexOf(',', startStreetIndex);
                var streetAllValue = address.Substring(startStreetIndex, index - startStreetIndex).Trim();

                var streetValue = streetAllValue.Split(new []{". "}, StringSplitOptions.RemoveEmptyEntries);
                this.Street = Session.FindObject<Street>(streetValue.Count() > 1 ? CriteriaOperator.Parse("Type.ShortName=? AND Name=?", streetValue[0], streetValue[1]) : CriteriaOperator.Parse("Name=?", streetAllValue));
            }
        }

        public override string ToString()
        {
            var str = string.Empty;
            /*if (this.Level1 != null)
                str = this.Level1.ToString();
            if (this.Level2 != null)
                str = this.Level2.ToString();*/
            /*if (this.Level3 != null)
                str = this.Level3.ToString();*/
            if (this.Level4 != null)
                str += Level4.Name;

            if (this.Street != null)
            {
                if (str.Length > 0)
                {
                    str += ", ";/*<- nbsp*/
                }
                str += this.Street.ToString();
            }

            if (string.IsNullOrEmpty(this.House) == false)
                str += (string.IsNullOrEmpty(str) ? string.Empty : ", ") + string.Format("д. {0}", this.House);

            if (string.IsNullOrEmpty(this.Build) == false)
                str += string.Format(", стр. {0}", this.Build);

            if (string.IsNullOrEmpty(this.Flat) == false)
                str += string.Format(", кв. {0}", this.Flat);

            return str.Trim();
        }
    }
}
