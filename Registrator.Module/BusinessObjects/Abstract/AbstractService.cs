using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using Registrator.Module.BusinessObjects.Dictionaries;
using Registrator.Module.BusinessObjects.Dictionaries.BaseMedical;
using Registrator.Module.BusinessObjects.Interfaces;
using System;
using System.ComponentModel;
using System.Xml.Linq;

namespace Registrator.Module.BusinessObjects.Abstract
{
    [DefaultClassOptions]
    [XafDisplayName("Услуги")]
    public abstract class AbstractService : BaseObject
    {
        private Doctor _doctor;
        private TerritorialUsluga _usluga;

        protected AbstractService(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            
            EditableProtocol = new EditableProtocol(Session);
            CommonProtocol = new CommonProtocol(Session);

            DateIn = DateTime.Now;

            // определяем текущего пользователя
            var createdBy = SecuritySystem.CurrentUser as Doctor;
            if (createdBy != null)
            {
                // находим доктора с таким же Логином
                var doctor = Session.FindObject<Doctor>(CriteriaOperator.Parse("UserName=?", createdBy.UserName));
                if (doctor != null)
                    this.Doctor = doctor;
            }
        }

        #region Мин. поля для реестра ТФОМС
        /// <summary>
        /// Обязательное поле T(6)
        /// Код МО
        /// МО лечения, указывается в соответствии с реестром F003
        /// </summary>
        [XafDisplayName("Лечащее МО")]
        [Browsable(false)]
        public MedOrg LPU { get; set; }

        /// <summary>
        /// Условно-обязательное поле T(8)
        /// Подразделение МО лечения из регионального справочника.
        /// </summary>
        [XafDisplayName("Код подразделения МО")]
        [Browsable(false)]
        public string LPU_1 {get; set;}
        

        /// <summary>
        /// Обязательное поле
        /// Дата начала оказания услуги
        /// </summary>
        [XafDisplayName("Дата начала оказания услуги")]
        public DateTime DateIn { get; set; }

        /// <summary>
        /// Обязательное поле
        /// Дата окончания оказания услуги
        /// </summary>
        [XafDisplayName("Дата окончания оказания услуги")]
        public DateTime DateOut { get; set; }

        /// <summary>
        /// Условно-обязательное поле N(15.2)
        /// Тариф
        /// </summary>
        [XafDisplayName("Тариф")]
        [Browsable(false)]
        public Decimal Tarif { get; set; }

        /// <summary>
        /// Обязательное поле N(15.2)
        /// Стоимость медицинской услуги, принятая к оплате (руб.)
        /// </summary>
        [XafDisplayName("Стоимость услуги")]
        [Browsable(false)]
        public Decimal Sum { get; set; }

        /// <summary>
        /// Обязательное поле N(9)
        /// Специальность врача, выполнившего услугу
        /// </summary>
        [Browsable(false)]
        public DoctorSpecTree DoctorSpec 
        {
            get { return Doctor != null ? Doctor.SpecialityTree : null; }
        }

        /// <summary>
        /// Обязательное поле T(25)
        /// Код медицинского работника, оказавшего медицинскую услугу
        /// В соответствии с территориальным справочником
        /// </summary>
        [XafDisplayName("Врач")]
        public Doctor Doctor
        {
            get
            {
                return _doctor;
            }
            set
            {
                SetPropertyValue("Doctor", ref _doctor, value);
                OnChanged("DoctorSpec");
                OnChanged("Otdel");
                OnChanged("Usluga");
            }
        }

        /// <summary>
        /// Условно-обязательное поле T(250)
        /// Служебная информация
        /// </summary>
        [XafDisplayName("Служебное поле")]
        [Browsable(false)]
        public string Comment { get; set; }

        #endregion

        /// <summary>
        /// Услуга
        /// Для реестра ТФОМС нужно только для общих случаев
        /// В бурятский ТФОМС можно отправлять услуги в реестре диспансеризаций
        /// </summary>
        [XafDisplayName("Услуга")]
        [DataSourceCriteriaProperty("UslugaCriteria")]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [ImmediatePostData(true)]
        public TerritorialUsluga Usluga
        {
            get { return _usluga; }
            set
            {
                SetPropertyValue("Usluga", ref _usluga, value);
                
                if (IsLoading || IsDeleted || value == null)
                    return;

                var criteria = new ContainsOperator("ServicesFor", new BinaryOperator("Code", value.Code, BinaryOperatorType.Equal));
                IList<ProtocolRecordType> protocolRecordTypes = new XPCollection<ProtocolRecordType>(Session, criteria);
                
                var records = protocolRecordTypes.Select(protocolRecordType => 
                    ProtocolRecord.GetProtocolRecord(Session, protocolRecordType));

                if (records.Any())
                {
                    foreach (var protocolRecord in records)
                    {
                        EditableProtocol.Records.Add(protocolRecord);
                    }

                    OnChanged("EditableProtocol");
                    if (EditableProtocolChanged != null)
                    {
                        EditableProtocolChanged(this, null);
                    }
                }
            }
        }

        public event EventHandler EditableProtocolChanged;

        [DevExpress.Xpo.Aggregated]
        public EditableProtocol EditableProtocol { get; set; }

        [DevExpress.Xpo.Aggregated]
        public CommonProtocol CommonProtocol { get; set; }

        /// <summary>
        /// Критерий допустимых услуг
        /// Фильтр по случаю, докторской специальности, профилю.
        /// </summary>
        [NonPersistent]
        [Browsable(false)]
        public virtual CriteriaOperator UslugaCriteria
        {
            get
            {
                // если не указана специальность выводим все услуги
                if (DoctorSpec == null)
                    return CriteriaOperator.Parse("true");
                return new ContainsOperator("DoctorSpec", new BinaryOperator("Code", DoctorSpec.Code));
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// Услуга
    /// </summary>
    public abstract class CommonService : AbstractService, IReestrTFoms
    {
        private CommonCase _case;
        // Первичный диагноз
        private MKB10 preDiagnose;
        // Первичный диагноз выявлен впервые
        private bool preDiagnoseIsFirstTime;
        // Характер первичного диагноза
        private KharakterDiagnoza preDiagnoseCharacter;
        // Стадия первичного диагноза
        private StadiaDiagnoza preDiagnoseStadia;
        // Основной диагноз
        private MKB10 mainDiagnose;
        // Основной диагноз впервые выявлен
        private bool mainDiagnoseIsFirstTime;
        // характрер основного диагноза
        private KharakterDiagnoza mainDiagnoseCharacter;
        // Стадия основного диагноза
        private StadiaDiagnoza mainDiagnoseStadia;

        public CommonService(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            
            var createdBy = SecuritySystem.CurrentUser as Doctor;
            if (createdBy != null)
            {
                // находим доктора с таким же Логином
                var doctor = Session.FindObject<Doctor>(CriteriaOperator.Parse("UserName=?", createdBy.UserName));
                if (doctor != null)
                    this.Doctor = doctor;
            }

            this.DateIn = DateTime.Now;
            string MOCode = Settings.MOSettings.GetCurrentMOCode(Session);
            this.LPU = Session.FindObject<MedOrg>(CriteriaOperator.Parse("Code=?", MOCode));
            this.LPU_1 = MOCode;

            this.KolUslug = 1;
            
            if (Case != null && Case.Pacient != null)
                this.DetProfil = Case.Pacient.GetAge() >= 18 ? PriznakDetProfila.No : PriznakDetProfila.Yes;

            if (this.Doctor != null)
                this.Otdelenie = this.Doctor.Otdelenie;
            if (DoctorSpec != null)
                this.Profil = DoctorSpec.MedProfil;

            // т.к. абстрактная услуга не содержит ссылку на случай
            // ссылку на случай (2-х видов) хранят только два типа услуг (общий и диспансеризации)
            // применяем фильтр к пациенту при добавлении записей при создании протокола в абстрактную услугу
            this.EditableProtocolChanged += CommonService_EditableProtocolChanged;
        }
        
        void CommonService_EditableProtocolChanged(object sender, EventArgs e)
        {
            if (Case != null)
            {
                var pacient = Case.Pacient;
                if (pacient != null)
                    EditableProtocol.ApplyPacientFilter(pacient);
            }
        }

        #region Мин. поля для реестра ТФОМС
        /// <summary>
        /// Условно-обязательное поле N(8)
        /// Код отделения
        /// Отделение МО лечения из регионального справочника
        /// </summary>
        [XafDisplayName("Отделение МО")]
        public Otdel Otdelenie { get; set; }

        /// <summary>
        /// Обязательное поле N(3)
        /// Код профиля
        /// Классификатор профилей оказанной медицинской помощи (V002)
        /// </summary>
        [XafDisplayName("Отделение МО")]
        [Browsable(false)]
        public MedProfil Profil { get; set; }

        /// <summary>
        /// Условно-обязательное поле Т(15)
        /// Вид медицинского вмешательства
        /// Указывается в соответствии с номенклатурой медицинских услуг (V001)
        /// </summary>
        [XafDisplayName("Вид мед. вмешательства")]
        [Browsable(false)]
        public VidMedVmeshatelstva VidVme { get; set; }

        /// <summary>
        /// Обязательное поле N(1)
        /// Признак детского профиля (0-нет, 1-да)
        /// Заполняется в зависимости от профиля оказанной медицинской помощи
        /// </summary>
        [XafDisplayName("Детский профиль")]
        [Browsable(false)]
        public PriznakDetProfila DetProfil { get; set; }

        /// <summary>
        /// Обязательное поле T(20)
        /// Код услуги из территориального классификатора услуг
        /// </summary>
        [XafDisplayName("Кол-во (койкодни/что-то еще)")]
        [Appearance("KolUslug_Invisible", Context = "DetailView", Visibility = ViewItemVisibility.Hide, Criteria = "!ShowKolUslug")]
        public double KolUslug { get; set; }
        #endregion

        #region Первичный диагноз
        /// <summary>
        /// Первичный диагноз
        /// </summary>
        [XafDisplayName("Первичный диагноз")]
        [ImmediatePostData]
        [Appearance("PrDiagnoseVisibility", Context = "DetailView", Visibility = ViewItemVisibility.Hide, Criteria = "!IsMainService")]
        public MKB10 CasePreDiagnose
        {
            get { return preDiagnose; }
            set { SetPropertyValue("CasePreDiagnose", ref preDiagnose, value); }
        }

        /// <summary>
        /// Первичный диагноз впервые выявлен
        /// </summary>
        [Appearance("PreTimeVisibility", Context = "DetailView", Visibility = ViewItemVisibility.Hide, Criteria = "!IsMainService Or IsNull(CasePreDiagnose)")]
        [XafDisplayName("Первичный диагноз впервые выявлен")]
        public bool CasePreDiagnoseIsFirstTime
        {
            get { return preDiagnoseIsFirstTime; }
            set { SetPropertyValue("CasePreDiagnoseIsFirstTime", ref preDiagnoseIsFirstTime, value); }
        }

        /// <summary>
        /// Характер первичного диагноза
        /// </summary>
        [Appearance("PreCharacterVisibility", Context = "DetailView", Visibility = ViewItemVisibility.Hide, Criteria = "!IsMainService Or IsNull(CasePreDiagnose)")]
        [XafDisplayName("Характер первичного диагноза")]
        public KharakterDiagnoza CasePreDiagnoseCharacter
        {
            get { return preDiagnoseCharacter; }
            set { SetPropertyValue("CasePreDiagnoseCharacter", ref preDiagnoseCharacter, value); }
        }

        /// <summary>
        ///  Стадия первичного диагноза
        /// </summary>
        [Appearance("PreStadiaVisibility", Context = "DetailView", Visibility = ViewItemVisibility.Hide, Criteria = "!IsMainService Or IsNull(CasePreDiagnose)")]
        [XafDisplayName("Стадия первичного диагноза")]
        public StadiaDiagnoza CasePreDiagnoseStadia
        {
            get { return preDiagnoseStadia; }
            set { SetPropertyValue("CasePreDiagnoseStadia", ref preDiagnoseStadia, value); }
        }
        #endregion

        #region Основной диагноз

        /// <summary>
        /// Основной диагноз
        /// </summary>
        [ImmediatePostData]
        [XafDisplayName("Основной диагноз")]
        public MKB10 CaseDiagnose
        {
            get { return mainDiagnose; }
            set { SetPropertyValue("CaseDiagnose", ref mainDiagnose, value); }
        }

        /// <summary>
        /// Основной диагноз впервые выявлен
        /// </summary>
        [Appearance("TimeVisibility", Context = "DetailView", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(CaseDiagnose)")]
        [XafDisplayName("Основной диагноз впервые выявлен")]
        public bool CaseDiagnoseIsFirstTime
        {
            get { return mainDiagnoseIsFirstTime; }
            set { SetPropertyValue("CaseDiagnoseIsFirstTime", ref mainDiagnoseIsFirstTime, value); }
        }

        /// <summary>
        /// Характер основного диагноза
        /// </summary>
        [Appearance("CharacterVisibility", Context = "DetailView", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(CaseDiagnose)")]
        [XafDisplayName("Характер основного диагноза")]
        public KharakterDiagnoza CaseDiagnoseCharacter
        {
            get { return mainDiagnoseCharacter; }
            set { SetPropertyValue("CaseDiagnoseCharacter", ref mainDiagnoseCharacter, value); }
        }

        /// <summary>
        /// Стадия основного диагноза
        /// </summary>
        [Appearance("StadiaVisibility", Context = "DetailView", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(CaseDiagnose)")]
        [XafDisplayName("Стадия основного диагноза")]
        public StadiaDiagnoza CaseDiagnoseStadia
        {
            get { return mainDiagnoseStadia; }
            set { SetPropertyValue("CaseDiagnoseStadia", ref mainDiagnoseStadia, value); }
        }

        #endregion

        /// <summary>
        /// Определяет является ли услуга первой услугой случая
        /// В нем имеется возможность добавить первичный и основной диагнозы
        /// </summary>
        [Browsable(false)]
        [XafDisplayName("Первая услуга случая")]
        public bool IsMainService { get; set; }

        /// <summary>
        /// Флаг автооткрытия услуги
        /// </summary>
        [Browsable(false)]
        [NonPersistent]
        public bool AutoOpen { get; set; }

        /// <summary>
        /// Возвращает Истина если случай, в которой создана услуга - дневной стационар 
        /// </summary>
        [Browsable(false)]
        public bool ShowKolUslug
        {
            get
            {
                if (Case != null && Case.GetType() == typeof(HospitalCase))
                    return true;
                return false;
            }
        }

        /// <summary>
        /// Случай, в рамках которого была оказана услуга
        /// </summary>
        [Association("CommonCase-CommonServices")]
        public CommonCase Case
        {
            get { return _case; }
            set { SetPropertyValue("Case", ref _case, value); }
        }

        /// <summary>
        /// Диагнозы случая
        /// </summary>
        [Association("CommonService-Diagnoses"), DevExpress.Xpo.Aggregated]
        public XPCollection<MKBWithType> Diagnoses
        {
            get { return GetCollection<MKBWithType>("Diagnoses"); }
        }

        public abstract bool IsValidForReestr();
        public abstract XElement GetReestrElement();
        public abstract XElement GetReestrElement(int zapNumber);
    }

    /// <summary>
    /// Услуга для диспансеризации
    /// </summary>
    public abstract class DispService : AbstractService, IReestrTFoms
    {
        private const string MEDSESTRACODE = "219";

        public DispService(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            // т.к. абстрактная услуга не содержит ссылку на случай
            // ссылку на случай (2-х видов) хранят только два типа услуг (общий и диспансеризации)
            // применяем фильтр к пациенту при добавлении записей при создании протокола в абстрактную услугу
            this.EditableProtocolChanged += DispService_EditableProtocolChanged;
        }

        private void DispService_EditableProtocolChanged(object sender, EventArgs e)
        {
            if (Case != null)
            {
                var pacient = Case.Pacient;
                if (pacient != null)
                    EditableProtocol.ApplyPacientFilter(pacient, DateIn);
            }
        }

        [Browsable(false)]
        public bool IsMainService { get; set; }

        [XafDisplayName("Мед. сестра")]
        [DataSourceCriteriaProperty("MedSestraCriteria")]
        public Doctor MedSestra { get; set; }

        /// <summary>
        /// Критерий специальности &quat;Сестринское дело&quat;
        /// </summary>
        [Browsable(false)]
        public CriteriaOperator MedSestraCriteria
        {
            get { return CriteriaOperator.Parse("SpecialityTree.Code=?", MEDSESTRACODE); }
        }

        [XafDisplayName("Выявленные диагнозы до обследования")]
        [Association("DispService-MKBWithDispInfoBefore")]
        public XPCollection<MKBWithDispInfoBefore> DiagnosesBefore
        {
            get { return GetCollection<MKBWithDispInfoBefore>("DiagnosesBefore"); }
        }

        [XafDisplayName("Выявленные диагнозы после обследования")]
        [Association("DispService-MKBWithDispInfoAfter")]
        public XPCollection<MKBWithDispInfoAfter> DiagnosesAfter
        {
            get { return GetCollection<MKBWithDispInfoAfter>("DiagnosesAfter"); }
        }

        /// <summary>
        /// Случай, в рамках которого была оказана услуга
        /// </summary>
        [Association("DispCase-DispServices")]
        public DispCase Case { get; set; }

        //интерфейс элемента реестра реализуется в конкретной услуге
        public abstract bool IsValidForReestr();
        public abstract XElement GetReestrElement();
        public abstract XElement GetReestrElement(int zapNumber);
    }
}
