using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Registrator.Module.BusinessObjects.Dictionaries;
using DevExpress.ExpressApp.DC;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Validation;
namespace Registrator.Module.BusinessObjects
{
    /// <summary>
    /// Полис
    /// </summary>
    [DefaultClassOptions]
    public class Polis : BaseObject
    {
private  Kladr filterby;
        public Polis() { }
        public Polis(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            this.Serial = string.Empty;
            this.Number = string.Empty;

            var defaultCode = "3"; // единого образца
            this.Type = Session.FindObject<VidPolisa>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", defaultCode));
        }

        [Association("Pacient_Polis")]
        public Pacient Pacient { get; set; }
        [XafDisplayName("Тип полиса")]
        public VidPolisa Type { get; set; }

        /// <summary>
        /// Серия полиса
        /// </summary>
        [Size(10)]
        [XafDisplayName("Серия полиса")]
        public string Serial { get; set; }

        /// <summary>
        /// Номер полиса
        /// </summary>
        [Size(20)]
        [XafDisplayName("Номер полиса")]
        public string Number { get; set; }

        /// <summary>
        /// Страховая мед. организация, выдавшая полис
        /// </summary>
        [XafDisplayName("Страховая организация")]
        [DataSourceCriteriaProperty("ShowSMOCriteria")]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [RuleRequiredField(DefaultContexts.Save)]
        public StrahMedOrg SMO { get; set; }

        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [NonPersistent]
        [Browsable(false)]
        private CriteriaOperator ShowSMOCriteria
        {
            get
            {
                return CriteriaOperator.And(SMODateEndCriteria, SMORegionCriteria);
            }
        }

        /// <summary>
        /// Исключенные из реестра СМО
        /// </summary>
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [NonPersistent]
        [Browsable(false)]
        private CriteriaOperator SMODateEndCriteria
        {
            get
            {
                return CriteriaOperator.Parse("D_End is null");
            }
        }

        private bool _isFromAnotherRegion;
        [Browsable(false)]
        public bool IsFromAnotherRegion
        {
            get
            {
                _isFromAnotherRegion = SMO.TF_OKATO != Settings.RegionSettings.GetCurrentRegionOKATO(Session);
                return _isFromAnotherRegion;
            }
            set { SetPropertyValue("IsFromAnotherRegion", ref _isFromAnotherRegion, value); }
        }

        #region Фильтрация по региону
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [NonPersistent]
        [Browsable(false)]
        private CriteriaOperator SMORegionCriteria
        {
            get
            {
                // если не выбран фильтр, то показываем все СМО
                if (FilterBy == null)
                {
                    return CriteriaOperator.Parse("1=1");
                }

                var okato = FilterBy.CodeOkato;
                // код ТФ ОКАТО - это 5 первых цифр ОКАТО
                var tfOkatoLen = 5;
                if (okato.Length > tfOkatoLen - 1)
                    okato = okato.Substring(0, tfOkatoLen);

                return CriteriaOperator.Parse("TF_OKATO=?", okato);
            }
        }

        [XafDisplayName("Фильтр по региону")]
        [DataSourceCriteriaProperty("KladrLevelCriteria")]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [NonPersistent]
        [ImmediatePostData]
        public Kladr FilterBy
        {
            get { return filterby; }
            set
            {
                SetPropertyValue("FilterBy", ref filterby, value);
                OnChanged("FilterBy");
                OnChanged("ShowSMOCriteria");
                OnChanged("SMO");
            }
        }

        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [NonPersistent]
        [Browsable(false)]
        private CriteriaOperator KladrLevelCriteria
        {
            get
            {
                // фильтрация по региону
                var kladrLevelFilter = 1;
                return CriteriaOperator.Parse("Level=?", kladrLevelFilter);
            }
        }
        #endregion

        /// <summary>
        /// Дата начала действия полиса
        /// </summary>
        [XafDisplayName("Дата начала действия")]
        [RuleRequiredField(DefaultContexts.Save)]
        public DateTime? DateBegin { get; set; }

        /// <summary>
        /// Дата окончания действия полиса
        /// </summary>
        [XafDisplayName("Годен до")]
        public DateTime? DateEnd { get; set; }

        [Browsable(false)]
        [PersistentAlias("Iif(IsNull(DateEnd), True, DateEnd >= Today())")]
        public bool IsActive
        {
            get { return (bool) EvaluateAlias("IsActive"); }
        }

        /// <summary>
        /// Пункт регистрации застрахованного
        /// </summary>
        [Size(50)]
        public string PRZ { get; set; }

        public override string ToString()
        {
            //если нет серии
            if (string.IsNullOrEmpty(this.Serial))
                return string.Format("№ {0} от {1:dd/MM/yyyy}", this.Number, this.DateBegin);

            return string.Format("{2} № {0} от {1:dd/MM/yyyy}", this.Number, this.DateBegin, this.Serial);
        }
    }
}
