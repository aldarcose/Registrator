using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Model;
using DevExpress.Data.Filtering;
using Registrator.Module.BusinessObjects.Dictionaries;
using System.ComponentModel;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using System.Xml.Linq;
using Registrator.Module.BusinessObjects.Abstract;
using Registrator.Module.BusinessObjects.Dictionaries.BaseStacionar;

namespace Registrator.Module.BusinessObjects
{
    [DefaultClassOptions]
    [RuleCriteria("", DefaultContexts.Save, "KoikoDni > 0",
   "Кол-во койко-дней должно быть больше 0", SkipNullOrEmptyValues = false)]
    public class DnevnoyStacionar : XPObject
    {
        private MKB10 _diagnose;
        public DnevnoyStacionar(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            this.DataPriema = DateTime.Now;
            this.Napravlenie = BusinessObjects.Napravlenie.Planovaya;

            ResultatOplati = Oplata.NetResheniya;

            // определяем текущего пользователя
            var createdBy = SecuritySystem.CurrentUser as Doctor;
            if (createdBy != null)
            {
                // находим доктора с таким же Логином
                var doctor = Session.FindObject<Doctor>(CriteriaOperator.Parse("UserName=?", createdBy.UserName));

                if (doctor !=null)
                {
                    this.Doctor = doctor;
                }

            }
            
        }

        /// <summary>
        /// Дата приема
        /// </summary>
        [XafDisplayName("Дата приема")]
        [RuleRequiredField(DefaultContexts.Save)]
        public DateTime DataPriema { get; set; }

        /// <summary>
        /// Дата выписки
        /// </summary>
        [XafDisplayName("Дата выписки")]
        [VisibleInListView(true)]
        [ImmediatePostData(true)]
        [Appearance("DateVipiski_Invisible", Context="DetailView", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(DataVypiski)")]
        public DateTime DataVypiski { get; set; }

        /// <summary>
        /// Койко дни
        /// </summary>
        [XafDisplayName("Койко дни")]
        [RuleRequiredField(DefaultContexts.Save)]
        public int KoikoDni { get; set; }

        /// <summary>
        /// Диагноз
        /// </summary>
        [XafDisplayName("Диагноз")]
        [DataSourceCriteriaProperty("DiagnoseCriteria")]
        [ImmediatePostData(true)]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [RuleRequiredField(DefaultContexts.Save)]
        public MKB10 Diagnose
        {
            get { return _diagnose; }
            set
            {
                _diagnose = value;
                OnChanged("VidVmeCriteria");
            }
        }

        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [NonPersistent]
        [Browsable(false)]
        public CriteriaOperator DiagnoseCriteria
        {
            // здесь критерий который будет применен для выборки объекта диагноз
            get 
            {
                // получаем все инстансы объекта 
                // используем след. критерий для получения всех инстансев
                CriteriaOperator co = CriteriaOperator.Parse("1=1");
                var KSGs = Session.GetObjects(Session.GetClassInfo<ClinicStatGroups>(), co, null, 0, false, false).Cast<ClinicStatGroups>().ToList();
                return new InOperator("MKB", KSGs.Select(t => t.Diagnose.MKB));
            }
        }
        [XafDisplayName("Вид мед. вмешательства")]
        [DataSourceCriteriaProperty("VidVmeCriteria")]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [Appearance("VidVme_Invisible", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(VidVmeCriteria)")]
        public VidMedVmeshatelstva VidVme { get; set; }

        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [NonPersistent]
        [Browsable(false)]
        public CriteriaOperator VidVmeCriteria
        {
            // здесь критерий который будет применен для выборки объекта диагноз
            get
            {
                if (Diagnose != null)
                {
                    // получаем все инстансы объекта 
                    // используем след. критерий для получения всех инстансев
                    CriteriaOperator co = CriteriaOperator.Parse("Diagnose.MKB =?", Diagnose.MKB);
                    var KSG = Session.FindObject(typeof (ClinicStatGroups), co) as ClinicStatGroups;
                    if (KSG != null)
                    {
                        CriteriaOperator co2 = CriteriaOperator.Parse("1=1");
                        var csg_vidvme =
                            Session.GetObjects(Session.GetClassInfo<CsgVidVme>(), co2, null, 0, false, false)
                                .Cast<CsgVidVme>()
                                .ToList();
                        if (csg_vidvme.Count(t => t.CsgNumber == KSG.Number) > 0)
                        return new InOperator("Code",
                            csg_vidvme.Where(t => t.CsgNumber == KSG.Number).Select(t => t.VidVme.Code));
                        else
                        {
                            return null;
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Лечение на дому
        /// </summary>
        [XafDisplayName("На дому")]
        public bool NaDomy { get; set; }
        
        /// <summary>
        /// Тип госпитализации
        /// </summary>
        [XafDisplayName("Тип стационара")]
        [RuleRequiredField(DefaultContexts.Save)]
        /*
         * В зависимости от выбранного типа, в отчет проставляем след. значения:
         * Терапия, то <CODE_USL> - 098305 для взрослых, 
         * Педиатрия - 198305 для детей
         * Акушерский, то <CODE_USL> - 098304 для взрослых, 198304 для детей
         * Неврологический, то <CODE_USL> - 098308 для взрослых, 198308 для детей
        */
        public TipStacionara Type { get; set; }
        
        [XafDisplayName("Направление (Госпитализация)")]
        [Browsable(false)]
        public Napravlenie Napravlenie { get; set; }

/// <summary>
        /// Результат госпитализации, представляет исход заболевания для дневного стационара
        /// </summary>
        [XafDisplayName("Результат госпитализации")]
        [RuleRequiredField(DefaultContexts.Save)]
        [ImmediatePostData(true)]
        [Appearance("Resultat_Invisible", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(DataVypiski)")]
        /*
         * В зависимости от выбранного результата, в отчет проставляем след. значения
         * Выздоровление, то <RSLT> = 201, <ISHOD> = 201,
         * Улучшение, то <RSLT> = 201, <ISHOD> = 202,
         * Без перемен, то <RSLT> = 201, <ISHOD> = 203,
         * Ухудшение, то <RSLT> = 201, <ISHOD> = 204
         */
        public ResultatGospitalizacii Resultat { get; set; }

        /// <summary>
        /// Врач
        /// </summary>
        [XafDisplayName("Врач")]
        [RuleRequiredField(DefaultContexts.Save)]
        public Doctor Doctor { get; set; }

        [Association("Stacionar_BindHistory")]
        [Browsable(false)]
        public XPCollection<StacionarBindDoctorHistory> BindDoctorHistory
        {
            get
            {
                return GetCollection<StacionarBindDoctorHistory>("BindDoctorHistory");
            }
        }

        /// <summary>
        /// Направившее ЛПУ
        /// </summary>
        [XafDisplayName("Направившее ЛПУ")]
        [DataSourceCriteriaProperty("LPUCriteria")]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        public MedOrg FromLPU { get; set; }

        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [NonPersistent]
        [Browsable(false)]
        private CriteriaOperator LPUCriteria
        {
            get
            {
                var okato = Settings.RegionSettings.GetCurrentRegionOKATO(Session);
                return CriteriaOperator.Parse("TF_OKATO=?", okato);
            }
        }

        /// <summary>
        /// Пациент дневного стационара
        /// </summary>
        [Association("Pacient_DnevnoyStacionar")]
        public Pacient Pacient { get; set; }

        /// <summary>
        /// Результат решения ТФОМС по оплате. В зависимости от результат скрывать или выделять записи
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public Oplata ResultatOplati { get; set; }


        /// <summary>
        /// Получаем запись для реестра HM в формате XElement
        /// </summary>
        /// <returns>Элемент XML</returns>
        public XElement GetReestElement(int zapNumber)
        {
            const int isBaby = 0;
            string lpuCode = Settings.MOSettings.GetCurrentMOCode(Session);
            string lpuCode_1 = lpuCode;
            const string dateTimeFormat = "{0:yyyy-MM-dd}";
            const string decimalFormat = "0.00";

            var ksg = Session.FindObject<ClinicStatGroups>(CriteriaOperator.Parse("Diagnose.MKB=?", this.Diagnose.MKB));
            var age = this.Pacient.GetAge();

            var zap = new XElement("ZAP");
            // номер записи в счете
            zap.Add(new XElement("N_ZAP", zapNumber));
            // признак новой записи: 0, 1
            // в зависимости от результата оплаты (если 0, то запись новая)
            zap.Add(new XElement("PR_NOV", (ResultatOplati == Oplata.NetResheniya) ? 0 : 1));

            //данные пациента
            var polis = this.Pacient.Polises.First(t => (t.DateEnd == null) || (t.DateEnd != null && DateTime.Now <= t.DateEnd));
            zap.Add(new XElement("PACIENT",
                            new XElement("ID_PAC", this.Pacient.Oid), // GUID!
                            // вид полиса. Классификатор
                            new XElement("VPOLIS", polis.Type.Code),
                            // серия полиса
                            new XElement("SPOLIS", polis.Serial),
                            // номер полиса
                            new XElement("NPOLIS", polis.Number),
                            // код СМО
                            new XElement("SMO", polis.SMO.Code),
                            // признак новорожденного
                            new XElement("NOVOR", isBaby)));

            Decimal tarif = Settings.TarifSettings.GetDnevnoyStacionarTarif(Session);
            var paymentCode = 43;
            var childFlag = (Pacient.GetAge() < 18) ? 1 : 0;
            var codeUslugi = string.Empty;
            /* код услуги для дневного стационара
            * Терапия, то <CODE_USL> - 098305 для взрослых, 198305 для детей
            * Акушерский, то <CODE_USL> - 098304 для взрослых, 198304 для детей
            * Неврологический, то <CODE_USL> - 098308 для взрослых, 198308 для детей
            */
            switch (this.Type.Code)
            {
                // Код терапии
                case 1 :
                    codeUslugi = childFlag == 1 ? "198305" : "098305";
                    break;
                // Акушерский код
                case 2:
                    codeUslugi = childFlag == 1 ? "198304" : "098304";
                    break;
                // Неврологический код
                case 4:
                    codeUslugi = childFlag == 1 ? "198308" : "098308";
                    break;
                case 3:
                    codeUslugi = "198308";
                    break;
                default:
                    break;
            }

            //данные случая
            var sluch = new XElement("SLUCH", 
                            // порядковый номер случая для пациента. При слиянии с основным реестром следует изменить значение с учетом уже существующих случаев.
                            new XElement("IDCASE", this.Oid),
                            // код 2: Дневной стацинар. Классификатор условий оказания медицинской помощи (V006)
                            new XElement("USL_OK", 2),
                            // код 1: Первичная медико-санитарная помощь. Классификатор видов медицинской помощи (V008)
                            new XElement("VIDPOM", 1),
                            // код 3: Плановая. Классификатор форм оказания медицинской помощи (V014)
                            new XElement("FOR_POM", 3),
                            // Направление (госпитализация)	1 –плановая; 2 – экстренная
                            new XElement("EXTR", (int)this.Napravlenie),
                            // Код МО. МО лечения, указывается в соответствии с реестром F003.
                            new XElement("LPU", lpuCode),
                            // Подразделение МО
                            new XElement("LPU_1", lpuCode_1),
                            // Код отделения. Отделение МО лечения из регионального справочника.
                            new XElement("PODR", Doctor.Otdelenie.Code),
                            // Профиль. Классификатор (V002)
                            new XElement("PROFIL", ksg.Profil.Code),
                            // Признак детского профиля. 0-нет, 1-да.
                            // Заполняется в зависимости от профиля оказанной медицинской помощи. ??
                            new XElement("DET", childFlag),
                            // Номер истории болезни/талона амбулаторного пациента/карты вызова скорой медицинской помощи
                            new XElement("NHISTORY", this.Oid), // проверить отправив в ТФОМС
                            // дата приема
                            new XElement("DATE_1", string.Format(dateTimeFormat, this.DataPriema)),
                            // дата выписки
                            new XElement("DATE_2", string.Format(dateTimeFormat, this.DataVypiski)),
                            // основной диагноз
                            new XElement("DS1", Diagnose.MKB),
                            // Классификатор МЭС. Указывается при на-личии утверждённого стандарта.
                            new XElement("CODE_MES1", ksg.Number),
                            // код 201: Выписан. Классификатор результатов обращения за медицинской помощью (V009)
                            new XElement("RSLT", 201),
                            // Классификатор исходов заболевания (V012)
                            new XElement("ISHOD", (int)this.Resultat),
                            // Специальность лечащего врача/врача, закрывшего талон
                            new XElement("PRVS", this.Doctor.SpecialityCode),
                            // Код классификатора специальностей
                            new XElement("VERS_SPEC", "V015"),
                            // Код врача, закрывшего талон/историю болезни
                            new XElement("IDDOKT", this.Doctor.InnerCode),
                            new XElement("IDSP", paymentCode),
                            // Количество единиц оплаты медицинской помощи
                            new XElement("ED_COL", 1),
                            new XElement("TARIF", (tarif * Convert.ToDecimal(ksg.KoeffZatrat)).ToString(decimalFormat).Replace(",", ".")),
                            new XElement("SUMV", (tarif * Convert.ToDecimal(ksg.KoeffZatrat)).ToString(decimalFormat).Replace(",", ".")),
                            // тип оплаты. При отправке всегда 0. Решение принимает ТФОМС и возвращает как статус.
                            // можно фильтровать по оплате (если 1, вообще не трогаем)
                            new XElement("OPLATA", 0));
            
            //данные оказанной услуги
            var medService = new XElement("USL",
                    // порядковый номер в пределах случая. Для дневного стационара всегда 1, т.к. оказывается одна услуга
                    new XElement("IDSERV", 1),
                    new XElement("LPU", lpuCode),
                    new XElement("LPU_1", lpuCode_1),
                    new XElement("PODR", this.Doctor.Otdelenie.Code),
                    new XElement("PROFIL", ksg.Profil.Code),
                    // Признак детского профиля. 0-нет, 1-да.
                    new XElement("DET", childFlag),
                    // дата начала оказания услуги
                    new XElement("DATE_IN", string.Format(dateTimeFormat, this.DataPriema)),
                    // дата окончания оказания услуги
                    new XElement("DATE_OUT", string.Format(dateTimeFormat, this.DataVypiski)),
                    // диагноз
                    new XElement("DS", Diagnose.MKB),
                    // Код услуги	Территориальный классификатор услуг
                    new XElement("CODE_USL", codeUslugi),
                    // койкодни
                    new XElement("KOL_USL", this.KoikoDni),
                    new XElement("TARIF", (tarif * Convert.ToDecimal(ksg.KoeffZatrat)).ToString(decimalFormat).Replace(",", ".")),
                    new XElement("SUMV_USL", (tarif * Convert.ToDecimal(ksg.KoeffZatrat)).ToString(decimalFormat).Replace(",", ".")),
                    // специальность врача
                    new XElement("PRVS", this.Doctor.SpecialityCode),
                    // Код врача, закрывшего талон/историю болезни
                    new XElement("CODE_MD", this.Doctor.InnerCode)
                );

            var criteria = VidVmeCriteria;
            if (criteria != null)
            {
                if (VidVme != null)
                {
                    var profilElement = medService.Element("PROFIL");
                    if (profilElement != null)
                    {
                        profilElement.AddAfterSelf(new XElement("VID_VME", VidVme.Code));
                    }
                }
            }

            sluch.Add(medService);
            zap.Add(sluch);
            return zap;
        }
    }

    /// <summary>
    /// Направление (Госпитализация). Значение попадает в поле EXTR реестра
    /// </summary>
    public enum Napravlenie
    {
        [XafDisplayName("Плановая")]
        Planovaya = 1,
        [XafDisplayName("Экстренная")]
        Extrennaya = 2
    }

    /// <summary>
    /// Исход заболевания из справочника V012 для дневного стационара
    /// </summary>
    public enum ResultatGospitalizacii 
    {
        [XafDisplayName("Выздоровление")]
        Vizdorovlenie = 201,
        [XafDisplayName("Улучшение")]
        Uluchshenie = 202,
        [XafDisplayName("Без перемен")]
        BezPeremen = 203,
        [XafDisplayName("Ухудшение")]
        Uhudshenie = 204
    }
}
