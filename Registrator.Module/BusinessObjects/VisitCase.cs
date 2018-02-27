using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Data.Utils;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.XtraPrinting.Native;
using Registrator.Module.BusinessObjects.Abstract;
using Registrator.Module.BusinessObjects.Dictionaries;
using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using DevExpress.Persistent.Validation;

namespace Registrator.Module.BusinessObjects
{
    /// <summary>
    /// Посещение
    /// </summary>
    [DefaultClassOptions]
    [XafDisplayName("Посещение")]
    public class VisitCase : CommonCase
    {
        private MestoObsluzhivaniya mesto;
        private MKBWithType mainDiagnose;
        private const int minCodeForResultat = 301;
        private const int maxCodeForResultat = 315;
        public VisitCase(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            /*
             * Code = 29 - "За посещение в поликлинике"
             * Code = 30 - "За обращение (законченный случай) в поликлинике"
             */
            var sposobCode = (this.Cel == CelPosescheniya.ProfOsmotr) ? 29 : 30;
            this.SposobOplMedPom = Session.FindObject<Dictionaries.SposobOplatiMedPom>(CriteriaOperator.Parse("Code=?", sposobCode));

            /*
             * Code = 1 - "Стационаро"
             * Code = 2 - "В дневном стационаре"
             * Code = 3 - "Амбулаторно" 
             * Code = 4 - "Вне медицинской организации"
             */
            this.UsloviyaPomoshi = Session.FindObject<Dictionaries.VidUsloviyOkazMedPomoshi>(CriteriaOperator.Parse("Code=?", 3));

            /*
             * Code = "1" - "Экстренная"
             * Code = "2" - "Неотложная"
             * Code = "3" - "Плановая"
             */
            this.FormaPomoshi = Session.FindObject<Dictionaries.FormaMedPomoshi>(CriteriaOperator.Parse("Code=?", 3));

            if (Pacient != null)
            {
                this.DetProfil = Pacient.GetAge() >= 18 ? PriznakDetProfila.No : PriznakDetProfila.Yes;

                // если в качестве пациента указан мать, то указывается вес. в посещении не используется (уточнить)
                //if (false)
                //    this.VesPriRozhdenii = 0;
            }

            if (this.Doctor != null)
            {
                this.DoctorSpec = Doctor.SpecialityTree;
                this.Otdelenie = this.Doctor.Otdelenie;

                if (DoctorSpec != null)
                {
                    this.Profil = DoctorSpec.MedProfil;
                    /*
                     * Первичная мед.-санитарная помощь (код 1) проставляется для терапевтов (код 27), педиаторов (код 22), врачей сем. практики (код 16)
                     */
                    var vidPomoshiCode = 1;
                    if (DoctorSpec.Code == "16" || DoctorSpec.Code == "22" || DoctorSpec.Code == "27")
                    {
                        vidPomoshiCode = 1;
                    }
                    else
                    {
                        // другие значения
                    }
                    this.VidPom = Session.FindObject<Dictionaries.VidMedPomoshi>(CriteriaOperator.Parse("Code=?", vidPomoshiCode));
                }
            }
            
            // текущая версия классификатора специальностей.
            this.VersionSpecClassifier = "V015";
            this.StatusOplati = Oplata.NetResheniya;
        }

        /// <summary>
        /// Цель посещения
        /// </summary>
        [XafDisplayName("Цель посещения")]
        public CelPosescheniya Cel { get; set; }

        /// <summary>
        /// Место обслуживания
        /// </summary>
        [XafDisplayName("Место обслуживания")]
        public MestoObsluzhivaniya Mesto
        {
            get { return mesto; }
            set
            {
                SetPropertyValue("Mesto", ref mesto, value);
                OnChanged("Resultat");
            }
        }

        /// <summary>
        /// Основной диагноз
        /// </summary>
        [XafDisplayName("Основной диагноз")]
        public MKBWithType MainDiagnose 
        {
            get
            { 
                if (mainDiagnose == null)
                {
                    mainDiagnose = Services.OfType<CommonService>()
                        .SelectMany(s => s.Diagnoses)
                        .SingleOrDefault(d => d.Type == TipDiagnoza.Main);
                }
                return mainDiagnose;
            }
        }

        /// <summary>
        /// Создать случай посещения с услугой для пациента
        /// </summary>
        /// <param name="objectSpace"></param>
        /// <param name="pacient"></param>
        /// <param name="doctor"></param>
        /// <param name="dateIn"></param>
        public static VisitCase CreateVisitCase(IObjectSpace objectSpace, Pacient pacient, Doctor doctor, DateTime dateIn)
        {
            Doctor currentDoctor = objectSpace.GetObject((Doctor)SecuritySystem.CurrentUser);
            VisitCase newVisitCase = objectSpace.CreateObject<VisitCase>();
            newVisitCase.DateIn = dateIn;
            newVisitCase.Doctor = doctor;
            newVisitCase.Pacient = pacient;
            MedService newMedService = objectSpace.CreateObject<MedService>();
            newMedService.Case = newVisitCase;
            newMedService.DateIn = dateIn;
            newMedService.Doctor = doctor;
            return newVisitCase;
        }

        #region overriden

        public override CriteriaOperator ResultatCriteria
        {
            get
            {
                var codeList = Enumerable.Range(minCodeForResultat, maxCodeForResultat).Select(e => e.ToString());
                var criteriaOperators = new List<CriteriaOperator>();
                criteriaOperators.Add(new InOperator("Code", codeList));
                
                // если помощь оказывается вне ЛПУ, то также доступные результаты уровня 4
                string notInLpuCode = "4";
                if (Mesto != MestoObsluzhivaniya.LPU)
                    criteriaOperators.Add(CriteriaOperator.Parse("DlUslov=?", notInLpuCode));

                return CriteriaOperator.Or(criteriaOperators);
            }
        }

        public override bool IsValidForReestr()
        {
            throw new NotImplementedException();
        }

        public override System.Xml.Linq.XElement GetReestrElement()
        {
            throw new NotImplementedException();
        }

        public System.Xml.Linq.XElement GetReestrElement(int zapNumber, string lpuCode = null)
        {
            // проверяем поля услуги
            //if (IsValidForReestr() == false)
            //    return null;

            const int isBaby = 0;
            //string lpuCode = Settings.MOSettings.GetCurrentMOCode(Session);
            string lpuCode_1 = lpuCode;
            const string dateTimeFormat = "{0:yyyy-MM-dd}";
            const string decimalFormat = "n2";
            var age = this.Pacient.GetAge();

            var zap = new XElement("ZAP");
            // номер записи в счете
            zap.Add(new XElement("N_ZAP", zapNumber));
            // признак новой записи: 0, 1
            // в зависимости от результата оплаты (если 0, то запись новая)
            zap.Add(new XElement("PR_NOV", 0)); // ResultatOplati - в стационаре

            // данные пациента
            var polis = this.Pacient.Polises.FirstOrDefault(t => (t.DateEnd == null) || (t.DateEnd != null && DateTime.Now <= t.DateEnd));
            zap.Add(new XElement("PACIENT",
                            new XElement("ID_PAC", Pacient.Oid), // GUID!
                             // вид полиса. Классификатор
                            new XElement("VPOLIS", polis != null && polis.Type != null ? polis.Type.Code : string.Empty),
                            // серия полиса
                            new XElement("SPOLIS", polis != null ? polis.Serial : string.Empty),
                            // номер полиса
                            new XElement("NPOLIS", polis != null ? polis.Number : string.Empty),
                            // код СМО
                            new XElement("SMO", polis != null && polis.SMO != null ? polis.SMO.Code : string.Empty),
                            // признак новорожденного
                            new XElement("NOVOR", isBaby)));

            Decimal tarif = Settings.TarifSettings.GetDnevnoyStacionarTarif(Session);
            //var paymentCode = 43;
            var childFlag = (Pacient.GetAge() < 18) ? 1 : 0;

            XElement sluchElement = new XElement("SLUCH");
            // Номер записи в реестре случаев
            sluchElement.Add(new XElement("IDCASE", zapNumber));
            // Условия оказания мед. помощи
            sluchElement.Add(new XElement("USL_OK", this.UsloviyaPomoshi.Code));
            // Вид мед. помощи
            sluchElement.Add(new XElement("VIDPOM", this.VidPom.Code));
            // Форма мед. помощи
            sluchElement.Add(new XElement("FOR_POM", this.FormaPomoshi.Code));

            // Направившее МО
            if (FromLPU != null)
                sluchElement.Add(new XElement("NRP_MO", this.FromLPU.Code));
            // Код МО
            sluchElement.Add(new XElement("LPU", this.LPU.Code));

            string podr = lpuCode + (Profil != null ? (int?)Profil.Code : null) +
                (Otdelenie != null ? Otdelenie.Code : null);

            if (!string.IsNullOrEmpty(this.LPU_1))
                // код подразделения МО
                sluchElement.Add(new XElement("LPU_1", this.LPU_1));
            // Код отделения
            sluchElement.Add(new XElement("PODR", podr));
            // Профиль
            if (Profil != null)
                sluchElement.Add(new XElement("PROFIL", Profil.Code));
            // Детский профиль
            sluchElement.Add(new XElement("DET", (int)this.DetProfil));
            // Номер истории болезни/талона амбулаторного пациента
            sluchElement.Add(new XElement("NHISTORY", this.Oid));
            // Даты лечения
            sluchElement.Add(new XElement("DATE_1", string.Format(dateTimeFormat, this.DateIn)));
            sluchElement.Add(new XElement("DATE_2", string.Format(dateTimeFormat, this.DateOut)));
            // Первичный диагноз
            if (PreDiagnose != null && PreDiagnose.Diagnose != null)
                sluchElement.Add(new XElement("DS0", PreDiagnose.Diagnose.MKB));
            // основной диагноз
            if (MainDiagnose != null && MainDiagnose.Diagnose != null)
                sluchElement.Add(new XElement("DS1", MainDiagnose.Diagnose.MKB));

            // Сопутствующие диагнозы
            foreach(var ds2 in SoputsDiagnoses)
                sluchElement.Add(new XElement("DS2", ds2.MKB));
            // Диагнозы осложнений
            foreach(var ds3 in OslozhDiagnoses)
                sluchElement.Add(new XElement("DS3", ds3.MKB));

            // проверить карту пациента
            // Вес при рождении
            if (this.VesPriRozhdenii != 0)
                sluchElement.Add(new XElement("VNOV_M", this.VesPriRozhdenii));

            /*// Коды МЭС
            element.Add(new XElement("CODE_MES1", ));

            // Коды МЭС сопутствующих заболеваний
            element.Add(new XElement("CODE_MES2", ));*/

            // Результат обращения 
            sluchElement.Add(new XElement("RSLT", Resultat != null ? Resultat.Code : ""));
            // Исход заболевания
            sluchElement.Add(new XElement("ISHOD", Ishod != null ? Ishod.Code : ""));
            // Специальность леч. врача
            sluchElement.Add(new XElement("PRVS", this.DoctorSpec.Code));
            // Код классификатора мед. спец-й
            sluchElement.Add(new XElement("VERS_SPEC", this.VersionSpecClassifier));
            // Код врача, закрывшего случай
            sluchElement.Add(new XElement("IDDOCT", this.Doctor.SNILS));

            // Особые случаи
            //sluchElement.Add(new XElement("OS_SLUCH", (int)this.OsobiySluchay));

            // Способ оплаты мед. помощи
            if (SposobOplMedPom != null)
                sluchElement.Add(new XElement("IDSP", this.SposobOplMedPom.Code));

            /*// Кол-во единиц оплаты мед. помощи
            element.Add(new XElement("ED_COL", this.MedPomCount));*/

            // Тариф
            if (this.Tarif != 0)
                sluchElement.Add(new XElement("TARIF", this.Tarif));
            // Сумма
            sluchElement.Add(new XElement("SUMV", this.TotalSum.ToString(decimalFormat).Replace(",", ".")));
            // Тип оплаты
            sluchElement.Add(new XElement("OPLATA", (int)this.StatusOplati));

            // Данные по услугам
            int serviceCounter = 1;
            foreach (var usl in Services.OfType<MedService>())
                sluchElement.Add(usl.GetReestrElement(serviceCounter++, lpuCode));

            if (!string.IsNullOrEmpty(this.Comment))
                // Служебное поле
                sluchElement.Add(new XElement("COMMENTSL", this.Comment));

            zap.Add(sluchElement);
            return zap;
        }

        /// <summary>
        /// Критерий диагнозов
        /// </summary>
        public override CriteriaOperator DiagnoseCriteria
        {
            get { return CriteriaOperator.Parse("1=1"); }
        }

        #endregion
    }
}
