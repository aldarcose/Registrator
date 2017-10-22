using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// Тип записи протокола
    /// </summary>
    /// <remarks>
    /// Представляет тип записи протокола: имя, список услуг, в которых оно используется, тип данных и т.п.
    /// Реализует Аттрибут шаблона EAV.
    /// </remarks>
    [DefaultClassOptions]
    [XafDisplayName("Справочник Поле протокола")]
    public class ProtocolRecordType : BaseObject, ITreeNode
    {
        public ProtocolRecordType(Session session) : base(session) { }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            Name = string.Empty;
            Gender = GenderFor.All;
            IsMultipleChoiceList = false;
            Criteria = string.Empty;
            TimeType = TimeTypes.All;
            TimeFrom = new TimeLimitValue(Session);
            TimeTo = new TimeLimitValue(Session);
            IsRequired = true;

            // проставляем дефолтный код
            Code = GetDefaultCode();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            if (TimeFrom == null)
            {
                TimeFrom = new TimeLimitValue(Session);
                TimeType = TimeTypes.All;
            }
            if (TimeTo == null)
                TimeTo = new TimeLimitValue(Session);
        }

        // уникальный код поля протокола.
        // используем для идентификации поля протокола для выгрузок значения.
        // т.к. поле протокола имеет древовидную структуру, то
        // код вложеных элементов содержит в себе код родителя разделенных точкой
        // для корня код будет формата "\d+"
        // для элементов первого уровня "\d+.\d+"
        // для элементов второго уровня "\d+.\d+.\d+"
        [RuleRequiredField]
        public string Code { get; set; }

        public string GetDefaultCode()
        {
            if (_parent == null)
            {
                var protocolTypes = Session.GetObjects(ClassInfo, CriteriaOperator.Parse("IsNull(Parent)"), null, 0, false, false).Cast<ProtocolRecordType>().ToList();
                if (protocolTypes.Count > 0)
                {
                    int max = 0;
                    foreach (var protocolType in protocolTypes)
                    {
                        if (protocolType.Code != null)
                        {
                            int code = int.Parse(protocolType.Code);
                            if (code > max)
                                max = code;
                        }
                    }

                    return (max+1).ToString();
                }
                return "1";
            }
            else
            {
                // для вложенных
                // получаем номер из вложенных, игнорируя другую часть кода
                var protocolTypes = Session.GetObjects(ClassInfo, CriteriaOperator.Parse("Parent=?", this.Parent), null, 0, false, false).Cast<ProtocolRecordType>().ToList();
                if (protocolTypes.Count > 0)
                {
                    int max = 0;
                    foreach (var protocolType in protocolTypes)
                    {
                        if (protocolType.Code != null)
                        {
                            var lastDelimIndex = protocolType.Code.LastIndexOf(".");
                            int code = int.Parse(protocolType.Code.Substring(lastDelimIndex + 1));
                            if (code > max)
                                max = code;
                        }
                    }

                    return string.Format("{0}.{1}", _parent.Code, max + 1);
                }
            }

            return "";
        }

        #region TimeCriteria
        [XafDisplayName("Тип фильтра")]
        [ImmediatePostData(true)]
        public TimeTypes TimeType { get; set; }
        [Appearance("TimeFromHide", Visibility = ViewItemVisibility.Hide, Criteria = "[TimeType]=1 or [TimeType]=3", Context = "DetailView")]
        [XafDisplayName("C")]
        [DevExpress.Xpo.Aggregated]
        public TimeLimitValue TimeFrom { get; set; }
        [Appearance("TimeToHide", Visibility = ViewItemVisibility.Hide, Criteria = "[TimeType]=0 or [TimeType]=3", Context = "DetailView")]
        [XafDisplayName("По")]
        [DevExpress.Xpo.Aggregated]
        public TimeLimitValue TimeTo { get; set; }

        #endregion
        
        [XafDisplayName("Пол")]
        public GenderFor Gender { get; set; }

        [XafDisplayName("Обязательное поле")]
        public bool IsRequired { get; set; }

        [Appearance("CriteriaVisible", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(Parent)", Context="DetailView")]
        [XafDisplayName("Критерий скрытия")]
        [VisibleInListView(false)]
        public string Criteria { get; set; }

        [XafDisplayName("Имя записи")]
        [RuleRequiredField]
        [Size(256)]
        public string Name { get; set; }
        #region ITreeNode
        [Association("ProtocolRecordTypeParent-ProtocolRecordTypeChildren")]
        [Browsable(false)]
        
        public ProtocolRecordType _parent { get; set; }

        [Association("ProtocolRecordTypeParent-ProtocolRecordTypeChildren")]
        [Appearance("TreeChildVisible", Context = "DetailView", Criteria = "[Type]!=7", Visibility = ViewItemVisibility.Hide)]
        [XafDisplayName("Тип значения")]
        public XPCollection<ProtocolRecordType> _children 
        {
            get { return GetCollection<ProtocolRecordType>("_children"); }
        }

        [Browsable(false)]
        public ITreeNode Parent { get { return _parent as ITreeNode; } }
        
        [Browsable(false)]
        public IBindingList Children { get { return _children as IBindingList; } }
        #endregion

        [XafDisplayName("Используется в услуге")]
        [Association("RecordType-Service")]
        [Appearance("ServicesVisible", Context = "DetailView", Criteria = "!IsNull(Parent)", Visibility = ViewItemVisibility.Hide)]
        public XPCollection<TerritorialUsluga> ServicesFor
        {
            get { return GetCollection<TerritorialUsluga>("ServicesFor"); }
        }
        
        [ImmediatePostData(true)]
        [XafDisplayName("Тип значения")]
        public TypeEnum Type { get; set; }

        [Appearance("ListVisible", Context = "DetailView", Criteria = "[Type]!=6", Visibility = ViewItemVisibility.Hide)] // список выбора
        [Association("RecordType-ListValues")]
        [XafDisplayName("Список значений для выбора")]
        public XPCollection<StringValue> ListValues 
        {
            get { return GetCollection<StringValue>("ListValues"); }
        }

        [Appearance("ListMultipleChoiceVisible", Context = "DetailView", Criteria = "[Type]!=6", Visibility = ViewItemVisibility.Hide)] // список выбора
        [XafDisplayName("Разрешить множественный выбор из списка")]
        [VisibleInListView(false)]
        public bool IsMultipleChoiceList { get; set; }

        public string GetDefaultValue()
        {
            string result = string.Empty;
            switch (this.Type)
            {
                case TypeEnum.Bool:
                    result = "False";
                    break;
                case TypeEnum.Integer:
                    result = "0";
                    break;
                case TypeEnum.Double:
                    result = "0.00";
                    break;
                case TypeEnum.Decimal:
                    result = "0.00";
                    break;
                case TypeEnum.Date:
                    result = DateTime.Now.ToString("dd.MM.yyyy");
                    break;
                case TypeEnum.String:
                    result = string.Empty;
                    break;
                case TypeEnum.List:
                    result = string.Empty;
                    break;
                case TypeEnum.Complex:
                    break;
                case TypeEnum.Address:
                    break;
                case TypeEnum.Time:
                    break;
                default:
                    return string.Empty;
            }
            return result;
        }
    }

    public enum GenderFor
    {
        [XafDisplayName("Все")]
        All = 0,
        [XafDisplayName("Ж.")]
        F = 1,
        [XafDisplayName("М.")]
        M = 2
    }

    public enum TypeEnum
    {
        [XafDisplayName("Булево")]
        Bool = 0,
        [XafDisplayName("Целочисленное")]
        Integer = 1,
        [XafDisplayName("Числовое двойной точности")]
        Double = 2,
        [XafDisplayName("Числовое для денежных расчетов")]
        Decimal = 3,
        [XafDisplayName("Дата")]
        Date = 4,
        [XafDisplayName("Время")]
        Time = 9,
        [XafDisplayName("Текстовое поле")]
        String = 5,
        [XafDisplayName("Список выбора строк")]
        List = 6,
        [XafDisplayName("Адрес")]
        Address = 8,
        [XafDisplayName("Составное")]
        Complex = 7
    }

    public enum TimeTypes
    {
        [XafDisplayName("C (включительно)")]
        FromLimit = 0,
        [XafDisplayName("По (включительно)")]
        ToLimit = 1,
        [XafDisplayName("Диапазон (включительно)")]
        Range = 2,
        [XafDisplayName("Для всех")]
        All = 3
    }

    public class TimeLimitValue : BaseObject
    {
        public TimeLimitValue(Session session) : base(session) { }

        [XafDisplayName("Год")]
        public int Year { get; set; }
        [XafDisplayName("Месяц")]
        public int Month { get; set; }

        public override string ToString()
        {
            // используем "л." - 5 л., 36 л.
            string yearSymbol = "л.";
            int mod = Year%10;
            // если последняя цифа года [1:4]
            if (mod > 0 && mod < 5)
            {
                // и это не 11, 12, 13, 14
                if (mod%10 != 1)
                {
                    // то используем "г." - 1 г., 21 г., но не 11 г.
                    yearSymbol = "г.";
                }
                    
            }
            return string.Format("{0} {2} {1} мес.", Year, Month, yearSymbol);
        }
    }

    public class StringValue : BaseObject
    {
        public StringValue(Session session) : base(session) { }
        
        [XafDisplayName("Значение выбора")]
        [Size(256)]
        public string Value { get; set; }

        [Association("RecordType-ListValues")]
        public ProtocolRecordType ProtocolRecordType { get; set; }
    }
}
