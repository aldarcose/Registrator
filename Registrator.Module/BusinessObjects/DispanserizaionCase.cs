using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Registrator.Module.BusinessObjects.Abstract;
using Registrator.Module.BusinessObjects.Dictionaries;
using System;
using Registrator.Module.BusinessObjects.Dictionaries.BaseDispancerization;
using Registrator.Module.BusinessObjects.Dictionaries.BaseMedical;
using Registrator.Module.BusinessObjects.Interfaces;

namespace Registrator.Module.BusinessObjects
{
    [DefaultClassOptions]
    [XafDisplayName("Диспансеризации")]
    public class DispanserizaionCase : DispCase, IReestrFederalPortalChildren
    {
        public DispanserizaionCase(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            Rehab = new Rehabilitation(Session);
        }

        public DispType Type { get; set; }

        [DevExpress.Xpo.Aggregated]
        public Rehabilitation Rehab { get; set; }

        [XafDisplayName("Группа здоровья до обследования")]
        public HealthGroupObs HealthGroupObsBefore { get; set; }
        [XafDisplayName("Группа здоровья для занятия физ. культурой до обследования")]
        public HealthGroupForSportObs HealthGroupForSportObsBefore { get; set; }

        [XafDisplayName("Группа здоровья после обследования")]
        public HealthGroupObs HealthGroupObsAfter { get; set; }
        [XafDisplayName("Группа здоровья для занятия физ. культурой после обследования")]
        public HealthGroupForSportObs HealthGroupForSportObsAfter { get; set; }

        [XafDisplayName("Пациент здоров")]
        public bool IsPacientHealthy { get; set; }

        [XafDisplayName("Код осмотра")]
        public MKB10 InspectionResult { get; set; }

        public void AddDefaultServices(Pacient pacient, DateTime? date = null)
        {
            // получаем список данных по услугам для текущего типа диспансеризации
            CriteriaOperator criteria = CriteriaOperator.Parse("Type=?", Type);
            var servicesToAddInfo = new XPCollection<DispsServiceList>(Session, criteria).ToList();
            if (servicesToAddInfo.Count > 0)
            {
                // находим среди них тот, который удовлетворяем заданным критериям
                var serviceToAddInfo = servicesToAddInfo.FirstOrDefault(
                    t => t.CheckPacient(pacient, date));

                // если инфа по услугам для данного типа диспансеризации найдены
                if (serviceToAddInfo != null)
                {
                    // добавляем услуги в случай
                    foreach (var serviceWithInfo in serviceToAddInfo.Services)
                    {
                        var myService = new DispanserizationService(Session);

                        if (date.HasValue)
                        {
                            myService.DateIn = date.Value;
                        }

                        // добавляем к услуге
                        this.Services.Add(myService);

                        // привязываем услугу. создается протокол, который использует случай
                        myService.Usluga = serviceWithInfo.Service;
                    }
                }
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
            const int isBaby = 0;
            //string lpuCode = Settings.MOSettings.GetCurrentMOCode(Session);
            string lpuCode_1 = lpuCode;
            const string dateTimeFormat = "{0:yyyy-MM-dd}";
            const string decimalFormat = "n2";
            var age = this.Pacient.GetAge();

            var xZap = new XElement("ZAP");
            // номер записи в счете
            xZap.Add(new XElement("N_ZAP", zapNumber));
            // признак новой записи: 0, 1
            // в зависимости от результата оплаты (если 0, то запись новая)
            xZap.Add(new XElement("PR_NOV", 0));

            // данные пациента
            var polis = this.Pacient.Polises.FirstOrDefault(t => (t.DateEnd == null) || (t.DateEnd != null && DateTime.Now <= t.DateEnd));
            xZap.Add(new XElement("PACIENT",
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
            // sluchElement.Add(new XElement("USL_OK", ));
            // Вид мед. помощи
            if (VidPom != null)
                sluchElement.Add(new XElement("VIDPOM", VidPom.Code));
            // Форма мед. помощи
            // sluchElement.Add(new XElement("FOR_POM", ));

            // Направившее МО
            //if (FromLPU != null)
            //    sluchElement.Add(new XElement("NPR_MO", this.FromLPU.Code));
            // Код МО
            sluchElement.Add(new XElement("LPU", this.LPU.Code));

            if (!string.IsNullOrEmpty(this.LPU_1))
                // код подразделения МО
                sluchElement.Add(new XElement("LPU_1", this.LPU_1));
            // Код отделения
            //if (Otdelenie != null)
            //    sluchElement.Add(new XElement("PODR", this.Otdelenie.Code));
            // Профиль
            //if (Profil != null)
            //    sluchElement.Add(new XElement("PROFIL", Profil.Code));
            // Детский профиль
            //sluchElement.Add(new XElement("DET", (int)this.DetProfil));
            // Номер истории болезни/талона амбулаторного пациента
            sluchElement.Add(new XElement("NHISTORY", this.Oid));
            // Даты лечения
            sluchElement.Add(new XElement("DATE_1", string.Format(dateTimeFormat, this.DateIn)));
            sluchElement.Add(new XElement("DATE_2", string.Format(dateTimeFormat, this.DateOut)));
            // Первичный диагноз
            if (PreDiagnose != null && PreDiagnose.Diagnose != null)
                sluchElement.Add(new XElement("DS0", PreDiagnose.Diagnose.CODE));
            // основной диагноз
            //if (MainDiagnose != null && MainDiagnose.Diagnose != null)
            //    sluchElement.Add(new XElement("DS1", MainDiagnose.Diagnose.CODE));

            // Сопутствующие диагнозы
            //foreach (var ds2 in SoputsDiagnoses)
            //    sluchElement.Add(new XElement("DS2", ds2.CODE));
            // Диагнозы осложнений
            //foreach (var ds3 in OslozhDiagnoses)
            //    sluchElement.Add(new XElement("DS3", ds3.CODE));

            // проверить карту пациента
            // Вес при рождении
            //if (this.VesPriRozhdenii != 0)
            //    sluchElement.Add(new XElement("VNOV_M", this.VesPriRozhdenii));

            /*// Коды МЭС
            element.Add(new XElement("CODE_MES1", ));

            // Коды МЭС сопутствующих заболеваний
            element.Add(new XElement("CODE_MES2", ));*/

            // Результат обращения 
          //  sluchElement.Add(new XElement("RSLT", this.Resultat.Code));
            // Исход заболевания
           // sluchElement.Add(new XElement("ISHOD", this.Ishod.Code));
            // Специальность леч. врача
           // sluchElement.Add(new XElement("PRVS", this.DoctorSpec.Code));
            // Код классификатора мед. спец-й
           // sluchElement.Add(new XElement("VERS_SPEC", this.VersionSpecClassifier));
            // Код врача, закрывшего случай
           // sluchElement.Add(new XElement("IDDOKT", this.Doctor.InnerCode));

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
            foreach (var usl in Services.OfType<DispanserizationService>())
                sluchElement.Add(usl.GetReestrElement(serviceCounter++, lpuCode));

            if (!string.IsNullOrEmpty(this.Comment))
                // Служебное поле
                sluchElement.Add(new XElement("COMMENTSL", this.Comment));

            xZap.Add(sluchElement);

            return xZap;
        }

        public override CriteriaOperator DiagnoseCriteria
        {
            get { return CriteriaOperator.Parse("1=1"); }
        }

        public XElement GetCardBlock()
        {
            var protocols = new List<ProtocolRecord>();
            var diagnosesBefore = new List<MKBWithDispInfoBefore>();
            var diagnosesAfter = new List<MKBWithDispInfoAfter>();
            foreach (var dispService in this.Services)
            {
                diagnosesBefore.AddRange(dispService.DiagnosesBefore);
                diagnosesAfter.AddRange(dispService.DiagnosesAfter);
                protocols.AddRange(dispService.EditableProtocol.Records);
            }

            var pediatrService = this.Services.First(t => t.Usluga.Code.Equals("161014"));

             /* idType - вид карты обследования
             * Для детей-сирот:
             * 1 — карта диспансеризации
             * Для всех:
             * 2 — профилактический осмотр
             * 3 — предварительный осмотр (при указании образовательного учреждения)
             * 4 — периодический осмотр (при указании образовательного учреждения)
             */

            int idType = 1;
            switch (this.Type)
            {
                case DispType.ProfOsmotrAdult:
                    // только дети
                    return null;
                case DispType.DOGVN1:
                    // только дети
                    return null;
                case DispType.DOGVN2:
                    // только дети
                    return null;
                case DispType.ProfOsmotrChild:
                    idType = 2;
                    break;
                case DispType.PreProfOsmotrChild:
                    idType = 3;
                    break;
                case DispType.PeriodProfOsmotrChild:
                    idType = 4;
                    break;
                case DispType.DispStacionarChildOrphan1:
                    // пока сирот не трогаем
                    return null;
                case DispType.DispStacionarChildOrphan12:
                    // пока сирот не трогаем
                    return null;
                case DispType.DispChildOrphan1:
                    // пока сирот не трогаем
                    return null;
                case DispType.DispChildOrphan12:
                    // пока сирот не трогаем
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            XElement card = new XElement("card");

            card.Add(new XElement("idInternal", this.Oid.ToString()));

            card.Add(new XElement("dateOfObsled", this.DateIn.Date.ToString("yyyy-MM-ddZ")));

            //card.Add(new XElement("ageObsled", this.Pacient.GetAge(this.DateIn)));

            card.Add(new XElement("idType", idType));

            // height
            var heightProtocol = protocols.First(t => t.Type.Code.Equals("33"));
            card.Add(new XElement("height", heightProtocol.Value));

            // weight
            var weightProtocol = protocols.First(t => t.Type.Code.Equals("24"));
            card.Add(new XElement("weight", weightProtocol.Value.Replace(",", ".")));
            // head size
            var headSizeProtocol = protocols.FirstOrDefault(t => t.Type.Code.Equals("27"));
            if (headSizeProtocol!=null)
                card.Add(new XElement("headSize", headSizeProtocol.Value));

            /*healthProblems >
                problem
                  1 — дефицит массы тела
                  2 — избыток массы тела
                  3 — низкий рост
                  4 — высокий рост

                  1 и 2, 3 и 4 — взаимоисключающие значения.
            */
            var healthProblemsProtocol = protocols.FirstOrDefault(t => t.Type.Code.Equals("26"));
            var problem1 =
                healthProblemsProtocol._children.Where(t => t.Type.Code.Equals("26.1")).Select(t => t.Value).First();
            var problem2 =
                healthProblemsProtocol._children.Where(t => t.Type.Code.Equals("26.2")).Select(t => t.Value).First();
            
            bool hasProblems = false;
            var healthProblem = new XElement("healthProblems");
            if (!problem1.Equals("Нет нарушения"))
            {
                hasProblems = true;
                var probProtocol = healthProblemsProtocol._children.First(t => t.Type.Code.Equals("26.1"));
                var listValue = probProtocol.Type.ListValues.First(t => t.Value.Equals(problem1));
                healthProblem.Add(new XElement("problem", probProtocol.Type.ListValues.IndexOf(listValue)));
            }
            if (!problem2.Equals("Нет нарушения"))
            {
                hasProblems = true;
                var probProtocol = healthProblemsProtocol._children.First(t => t.Type.Code.Equals("26.2"));
                var listValue = probProtocol.Type.ListValues.First(t => t.Value.Equals(problem2));
                healthProblem.Add(new XElement("problem", probProtocol.Type.ListValues.IndexOf(listValue) + 2));
            }

            if (hasProblems)
            {
                card.Add(healthProblem);
            }

            /*
            pshycDevelopment minOccurs="0"
            Оценка возраста психического развития для детей от 0 до 4 лет в месяцах
             * poznav
               познавательная функция
             * motor
               моторная функция
             * emot
               эмоциональная и социальная (контакт с окружающим миром) функции
             * rech
               предречевое и речевое развитие
             */
            var pshycDevelopmentProtocol = protocols.FirstOrDefault(t => t.Type.Code.Equals("28"));
            if (pshycDevelopmentProtocol != null)
            {
                var poznav =
                    pshycDevelopmentProtocol._children.Where(t => t.Type.Code.Equals("28.1")).Select(t => t.Value).First();
                var motor =
                    pshycDevelopmentProtocol._children.Where(t => t.Type.Code.Equals("28.2")).Select(t => t.Value).First();
                var emot =
                    pshycDevelopmentProtocol._children.Where(t => t.Type.Code.Equals("28.3")).Select(t => t.Value).First();
                var rech =
                    pshycDevelopmentProtocol._children.Where(t => t.Type.Code.Equals("28.4")).Select(t => t.Value).First();
                
                var root = new XElement("pshycDevelopment");

                root.Add(new XElement("poznav", poznav));
                root.Add(new XElement("motor", motor));
                root.Add(new XElement("emot", emot));
                root.Add(new XElement("rech", rech));

                card.Add(root);
            }
            /*
            pshycState minOccurs="0"
            Оценка состояния психического развития для детей от 5 лет
             * psihmot
               Психомоторная сфера
             * intel
               Интеллект
             * emotveg
               Эмоционально-вегетативная сфера
             */
            var pshycStateProtocol = protocols.FirstOrDefault(t => t.Type.Code.Equals("29"));
            if (pshycStateProtocol != null)
            {
                var psihmot =
                    pshycStateProtocol._children.Where(t => t.Type.Code.Equals("29.1")).Select(t => t.Value).First();
                var intel =
                    pshycStateProtocol._children.Where(t => t.Type.Code.Equals("29.2")).Select(t => t.Value).First();
                var emotveg =
                    pshycStateProtocol._children.Where(t => t.Type.Code.Equals("29.3")).Select(t => t.Value).First();
                
                var root = new XElement("pshycState");

                root.Add(new XElement("psihmot", psihmot.Equals("Норма") ? 0 : 1));
                root.Add(new XElement("intel", intel.Equals("Норма") ? 0 : 1));
                root.Add(new XElement("emotveg", emotveg.Equals("Норма") ? 0 : 1));

                card.Add(root);
            }
            /*
            sexFormulaMale minOccurs="0"
            Половая формула (муж.)
            Поле заполняется обязательно для мальчиков от 10 лет
             * P
             * Ax
             * Fa
             */
            var sexFormulaMaleProtocol = protocols.FirstOrDefault(t => t.Type.Code.Equals("31"));
            if (sexFormulaMaleProtocol != null)
            {
                var P =
                    sexFormulaMaleProtocol._children.Where(t => t.Type.Code.Equals("31.1")).Select(t => t.Value).First();
                var Ax =
                    sexFormulaMaleProtocol._children.Where(t => t.Type.Code.Equals("31.2")).Select(t => t.Value).First();
                var Fa =
                    sexFormulaMaleProtocol._children.Where(t => t.Type.Code.Equals("31.3")).Select(t => t.Value).First();

                var root = new XElement("sexFormulaMale");

                root.Add(new XElement("P", P));
                root.Add(new XElement("Ax", Ax));
                root.Add(new XElement("Fa", Fa));

                card.Add(root);
            }
            /*
            sexFormulaFemale minOccurs="0"
            Половая формула (жен.)
            Поле заполняется обязательно для девочек от 10 лет          
             * P
             * Ma
             * Ax
             * Me
             */
            var sexFormulaFemaleProtocol = protocols.FirstOrDefault(t => t.Type.Code.Equals("30"));
            if (sexFormulaFemaleProtocol != null)
            {
                var P =
                    sexFormulaFemaleProtocol._children.Where(t => t.Type.Code.Equals("30.1")).Select(t => t.Value).First();
                var Ma =
                    sexFormulaFemaleProtocol._children.Where(t => t.Type.Code.Equals("30.3")).Select(t => t.Value).First();
                var Ax =
                    sexFormulaFemaleProtocol._children.Where(t => t.Type.Code.Equals("30.2")).Select(t => t.Value).First();
                var Me =
                    sexFormulaFemaleProtocol._children.Where(t => t.Type.Code.Equals("30.4")).Select(t => t.Value).First();

                var root = new XElement("sexFormulaFemale");

                root.Add(new XElement("P", P));
                root.Add(new XElement("Ma", Ma));
                root.Add(new XElement("Ax", Ax));
                root.Add(new XElement("Me", Me));
                
                card.Add(root);
            }
            /*
            menses minOccurs="0"
            Менструальная функция
            Поле заполняется обязательно для девочек от 10 лет
             * menarhe
               Menarhe в месяцах
             * characters >
                * char
                Характеристика
                1 — регулярные
                2 — нерегулярные
                3 — обильные
                4 — скудные
                5 — умеренные
                6 — болезненные
                7 — безболезненные
                1 и 2; 3, 4 и 5; 6 и 7 — взаимоисключающие значения
            */
            var mensesProtocol = protocols.FirstOrDefault(t => t.Type.Code.Equals("30"));
            if (mensesProtocol != null)
            {
                var mensFunction =
                    mensesProtocol._children.Where(t => t.Type.Code.Equals("30.5")).Select(t => t.Value).First();
                if (mensFunction.Equals("Присутствует"))
                {
                    var menarhe =
                        mensesProtocol._children.Where(t => t.Type.Code.Equals("30.6")).Select(t => t.Value).First();
                    var reg =
                        mensesProtocol._children.Where(t => t.Type.Code.Equals("30.7")).Select(t => t.Value).First();
                    var county =
                        mensesProtocol._children.Where(t => t.Type.Code.Equals("30.8")).Select(t => t.Value).First();
                    var pain =
                        mensesProtocol._children.Where(t => t.Type.Code.Equals("30.9")).Select(t => t.Value).First();

                    var root = new XElement("menses");

                    root.Add(new XElement("menarhe", menarhe));

                    var rootChar = new XElement("characters");
                    rootChar.Add(new XElement("char", reg.Equals("регулярные") ? 1 : 2));
                    rootChar.Add(new XElement("char", county.Equals("обильные") ? 3 : (county.Equals("скудные") ? 4 : 5)));
                    rootChar.Add(new XElement("char", pain.Equals("болезненные") ? 6 : 7));

                    root.Add(rootChar);

                    card.Add(root);
                }
            }

            card.Add(new XElement("healthGroupBefore", (int)this.HealthGroupObsBefore));
            if (HealthGroupForSportObsBefore!=0)
                card.Add(new XElement("fizkultGroupBefore", (int)this.HealthGroupForSportObsBefore));

            /*
            diagnosisBefore minOccurs="0" >
            Диагнозы до проведения обследования
             * diagnosis >
                * mkb
                * dispNablud
                Диспансерное наблюдение
                1 — установлено ранее
                2 — установлено впервые
                3 — не установлено
                * lechen minOccurs="0" >
                        Лечение назначено
                    * condition
                    * organ
                    * notDone
                * reabil minOccurs="0" > 
                Медицинская реабилитация/санаторно-курортное лечение назначены
                    * condition
                    * organ
                    * notDone minOccurs="0"
                Медицинская реабилитация/санаторно-курортное лечение не выполнены в соответствии с назначением
                * vmp
                Высокотехнологичная медицинская помощь
                1 — рекомендована и оказана
                2 — рекомендована и не оказана
                0 — не рекомендована
             * 
             * diagNotDone >
                * reason
                Причина невыполнения
                1 — отсутствие на момент проведения диспансеризации
                2 — отказ от медицинского вмешательства
                3 — смена места жительства
                4 — выполнение не в полном объёме
                5 — проблемы организации медицинской помощи
                10 — прочие
                * reasonOther
                Иная причина невыполнения
             */
            var diagnBefore = new XElement("diagnosisBefore");
            foreach (var mkbWithDispInfoBefore in diagnosesBefore)
            {
                var diagn = new XElement("diagnosis");
                diagn.Add(new XElement("mkb", mkbWithDispInfoBefore.MKB10.MKB));
                diagn.Add(new XElement("dispNablud", (int)mkbWithDispInfoBefore.DispObser + 1)); // значения для перечисления c 0

                if (mkbWithDispInfoBefore.HealingEnabled)
                {
                    var lechen = new XElement("lechen");
                    lechen.Add(new XElement("condition", (int) mkbWithDispInfoBefore.Healing.HealCondition + 1));
                        // значения для перечисления c 0
                    lechen.Add(new XElement("organ", (int) mkbWithDispInfoBefore.Healing.MedOrganization + 1));
                        // значения для перечисления c 0
                    if (mkbWithDispInfoBefore.Healing.NotDone)
                    {
                        var notDone = new XElement("notDone");
                        notDone.Add(new XElement("reason", (int) mkbWithDispInfoBefore.Healing.NotDoneReason + 1));
                        if (!string.IsNullOrEmpty(mkbWithDispInfoBefore.Healing.AnotherReason))
                            notDone.Add(new XElement("reasonOther", mkbWithDispInfoBefore.Healing.AnotherReason));

                        lechen.Add(notDone);
                    }
                    diagn.Add(lechen);
                }

                if (mkbWithDispInfoBefore.RehabEnabled)
                {
                    var rehab = new XElement("reabil");
                    rehab.Add(new XElement("condition", (int) mkbWithDispInfoBefore.Rehabilitation.HealCondition + 1));
                        // значения для перечисления c 0
                    rehab.Add(new XElement("organ", (int) mkbWithDispInfoBefore.Rehabilitation.MedOrganization + 1));
                        // значения для перечисления c 0
                    if (mkbWithDispInfoBefore.Rehabilitation.NotDone)
                    {
                        var notDone = new XElement("notDone");
                        notDone.Add(new XElement("reason", (int) mkbWithDispInfoBefore.Rehabilitation.NotDoneReason + 1));
                        if (!string.IsNullOrEmpty(mkbWithDispInfoBefore.Rehabilitation.AnotherReason))
                            notDone.Add(new XElement("reasonOther", mkbWithDispInfoBefore.Rehabilitation.AnotherReason));

                        rehab.Add(notDone);
                    }
                    diagn.Add(rehab);
                }

                diagn.Add(new XElement("vmp", (int)mkbWithDispInfoBefore.HighTechRecommend));

                diagnBefore.Add(diagn);
            }
            if (diagnosesBefore.Count > 0)
                card.Add(diagnBefore);
            /*
            healthyMKB minOccurs="0"
            Код осмотра, если ребёнок здоров
            Код должен находиться в диапозоне Z00-Z10.
             */
            
            // !!
            
            /*
            diagnosisAfter >
             Диагнозы после обследования
             * diagnosis
                * mkb
                * firstTime
                        Выявлен впервые
                        1 — да
                        0 — нет
                * dispNablud
                        Диспансерное наблюдение
                        1 — установлено ранее
                        2 — установлено впервые
                        0 — не установлено
                * lechen minOccurs="0" >
                        Лечение назначено 
                    * condition
                    * organ
                * reabil minOccurs="0" >
                        Реабилитация/санаторно-курортное лечение назначены
                    * condition
                    * organ
                * consul" minOccurs="0" >
                        Дополнительные консультации и исследования назначены
                    * condition
                    * organ
                    * state
                              Дополнительные консультации и исследования выполнены
                              0 — не выполнены в соответствии с назначением
                              1 — выполнены в полном объёме
                              2 — выполнены в неполном объёме
                * needVMP
                        Рекомендована ВМП
                * needSMP
                        Рекомендована СМП
                * needSKL
                        Рекомендовано СКЛ
                * recommendNext
                        Рекомендации по диспансерному наблюдению, лечению, медицинской реабилитации и санаторно-курортному лечению с указанием вида медицинской организации и специальности врача
             */
            var diagnAfter = new XElement("diagnosisAfter");
            foreach (var mkbWithDispInfoAfter in diagnosesAfter)
            {
                var diagn = new XElement("diagnosis");
                diagn.Add(new XElement("mkb", mkbWithDispInfoAfter.MKB10.MKB));
                diagn.Add(new XElement("firstTime", mkbWithDispInfoAfter.IsNew ? 0 : 1));
                var dispNablud = (int) mkbWithDispInfoAfter.DispObser + 1;
                diagn.Add(new XElement("dispNablud", dispNablud == 3 ? 0 : dispNablud)); // значения для перечисления c 0

                if (mkbWithDispInfoAfter.HealingEnabled)
                {
                    var lechen = new XElement("lechen");
                    lechen.Add(new XElement("condition", (int) mkbWithDispInfoAfter.Healing.HealCondition + 1));
                        // значения для перечисления c 0
                    lechen.Add(new XElement("organ", (int) mkbWithDispInfoAfter.Healing.MedOrganization + 1));
                        // значения для перечисления c 0
                    diagn.Add(lechen);
                }
                if (mkbWithDispInfoAfter.RehabEnabled)
                {
                    var rehab = new XElement("reabil");
                    rehab.Add(new XElement("condition", (int) mkbWithDispInfoAfter.Rehabilitation.HealCondition + 1));
                        // значения для перечисления c 0
                    rehab.Add(new XElement("organ", (int) mkbWithDispInfoAfter.Rehabilitation.MedOrganization + 1));
                        // значения для перечисления c 0
                    diagn.Add(rehab);
                }
                if (mkbWithDispInfoAfter.AddConsultEnabled)
                {
                    var consul = new XElement("consul");
                    consul.Add(new XElement("condition", (int) mkbWithDispInfoAfter.AdditionalConsult.HealCondition + 1));
                        // значения для перечисления c 0
                    consul.Add(new XElement("organ", (int) mkbWithDispInfoAfter.AdditionalConsult.MedOrganization + 1));
                        // значения для перечисления c 0
                    consul.Add(new XElement("state", (int) mkbWithDispInfoAfter.AdditionalConsult.AdditionalConsultation));
                    diagn.Add(consul);
                }
                diagn.Add(new XElement("needVMP", mkbWithDispInfoAfter.RecommendsHighTech ? 1 : 0));
                diagn.Add(new XElement("needSMP", mkbWithDispInfoAfter.RecommendsAmbulance ? 1 : 0));
                diagn.Add(new XElement("needSKL", mkbWithDispInfoAfter.RecommendsResort ? 1 : 0));
                var recom = string.IsNullOrEmpty(mkbWithDispInfoAfter.Recommendation)
                    ? "нет"
                    : mkbWithDispInfoAfter.Recommendation;
                diagn.Add(new XElement("recommendNext", recom));

                diagnAfter.Add(diagn);
            }
            if (diagnosesAfter.Count > 0)
                card.Add(diagnAfter);

            card.Add(new XElement("healthGroup", (int)this.HealthGroupObsAfter));
            if (HealthGroupForSportObsAfter!=0)
                card.Add(new XElement("fizkultGroup", (int)this.HealthGroupForSportObsAfter));
            /*
            invalid minOccurs="0" >
            Информация об инвалидности
             * type
                  Вид инвалидности
                  1 — с рождения
                  2 — приобретённая
             * dateFirstDetected
                  Дата первого освидетельствования
             * dateLastConfirmed
                  Дата последнего освидетельствования
             * illnesses
                  Заболевания, обусловившие возникновение инвалидности
                  1 — Некоторые инфекционные и паразитарные, из них:
                  2 — туберкулез
                  3 — сифилис
                  4 — ВИЧ
                  5 — Новообразования
                  6 — Болезни крови, кроветворных органов и отдельные нарушения, вовлекающие имунный механизм, в том числе:
                  9 — СПИД
                  10 — Болезни эндокринной системы, расстройства питания и нарушения обмена веществ, из них:
                  13 — сахарный диабет
                  14 — Психические расстройства и расстройства поведения, в том числе:
                  15 — умственная отсталость
                  16 — Болезни нервной системы, из них:
                  17 — церебральный паралич и др. паралитические синдромы
                  18 — Болезни глаза и его придаточного аппарата
                  19 — Болезни уха и сосцевидного отростка
                  20 — Болезни системы кровообращения
                  21 — Болезни органов дыхания, из них:
                  22 — астма
                  23 — астматический статус
                  24 — Болезни органов пищеварения
                  25 — Болезни кожи и подкожной клетчатки
                  26 — Болезни костно-мышечной системы и соединительной ткани
                  27 — Болезни мочеполовой системы
                  28 — Отдельные состояния, возникающие в перинатальном периоде
                  29 — Врожденные аномалии, из них:
                  30 — аномалии нервной системы
                  31 — аномалии системы кровообращения
                  32 — аномалии опорно-двигательного аппарата
                  33 — Последствия травм, отравлений и других воздействий внешних причин
                
             * defects
                  Виды нарушений в состоянии здоровья
                  1 — умственные
                  2 — другие психологические
                  3 — языковые и речевые
                  4 — слуховые и вестибулярные
                  5 — зрительные
                  6 — висцеральные и метаболические расстройства питания
                  7 — двигательные
                  8 — уродующие
                  9 — общие и генерализованные
             */

            var disabilityProtocol = protocols.FirstOrDefault(t => t.Type.Code.Equals("25"));
            if (disabilityProtocol != null)
            {
                var hasDisablity =
                    disabilityProtocol._children.Where(t => t.Type.Code.Equals("25.1")).Select(t => t.Value).First();
                if (hasDisablity.ToLower().Equals("да"))
                {
                    var dateFirstDetected =
                        disabilityProtocol._children.Where(t => t.Type.Code.Equals("25.2")).Select(t => t.Value).First();
                    var dateLastConfirmed =
                        disabilityProtocol._children.Where(t => t.Type.Code.Equals("25.3")).Select(t => t.Value).First();
                    var type =
                        disabilityProtocol._children.Where(t => t.Type.Code.Equals("25.6")).Select(t => t.Value).First();

                    var illnessessProtocol =
                        disabilityProtocol._children.First(t => t.Type.Code.Equals("25.4"));
                    var illnessess = illnessessProtocol.Value;
                    var defectsProtocol =
                        disabilityProtocol._children.First(t => t.Type.Code.Equals("25.5"));
                    var defects = defectsProtocol.Value;
                    
                    var root = new XElement("invalid");

                    root.Add(new XElement("type", type.Equals("С рождения") ? 1 : 2));
                    root.Add(new XElement("dateFirstDetected", DateTime.Parse(dateFirstDetected).Date.ToString("yyyy-MM-ddZ")));
                    root.Add(new XElement("dateLastConfirmed", DateTime.Parse(dateLastConfirmed).Date.ToString("yyyy-MM-ddZ")));

                    
                    var rootIllness = new XElement("illnesses");
                    foreach (var ill in illnessessProtocol.Type.ListValues)
                    {
                        if (illnessess.Contains(ill.Value))
                            rootIllness.Add(new XElement("illness", getIllnessCode(ill.Value)));
                    }
                    
                    var rootDefects = new XElement("defects");
                    foreach (var def in defectsProtocol.Type.ListValues)
                    {
                        if (defects.Contains(def.Value))
                            rootDefects.Add(new XElement("defect", getDefectsCode(def.Value)));
                    }

                    if (rootIllness.HasElements)
                        root.Add(rootIllness);
                    if (rootDefects.HasElements)
                        root.Add(rootDefects);

                    card.Add(root);
                }
            }

            /*
            issled
            Проведённые исследования
             * basic minOccurs="0" >
                  Обязательные исследования
                * record >
                    * id
                              Идентификатор обязательного исследования
                              1 — Общий анализ крови
                              2 — Общий анализ мочи
                              3 — Общий анализ кала
                              4 — Исследование уровня глюкозы в крови
                              5 — Исследование уровня гормонов в крови
                              6 — УЗИ органов брюшной полости
                              7 — УЗИ сердца
                              8 — УЗИ щитовидной железы
                              9 — УЗИ органов репродуктивной сферы
                              10 — УЗИ тазобедренных суставов
                              11 — Нейросонография
                              12 — Флюорография
                              13 — Электрокардиография
                              14 — Неонатальный скрининг на врожденный гипотиреоз, фенилкетонурию, адреногенитальный синдром, муковисцидоз и галактоземию
                              15 — Аудиологический скрининг
                              16 — Анализ кала на яйца глистов
                              17 — Анализ окиси углерода выдыхаемого воздуха с определением карбоксигемоглобина
                              18 — УЗИ почек
                              19 — УЗИ печени
                    * date
                              Дата исследования
                    * result
                              Результат исследования
             * other minOccurs="0" >
                  Дополнительные исследования
                * record >
                    * date
                              Дата проведения исследования
                * name
                              Название исследования
                * result
                              Результат исследования
             */
            var issled = new XElement("issled");
            var osmotri = new XElement("osmotri");

            var issledCodes = new Dictionary<string, int>();
            issledCodes.Add("021731", 1); // 1 — Общий анализ крови
            issledCodes.Add("021733", 2); // 2 — Общий анализ мочи
            issledCodes.Add("1", 3); // 3 — Общий анализ кала
            issledCodes.Add("063432", 4); // 4 — Исследование уровня глюкозы в крови
            issledCodes.Add("2", 5); // 5 — Исследование уровня гормонов в крови
            issledCodes.Add("021769", 6); // 6 — УЗИ органов брюшной полости
            issledCodes.Add("021771", 7); // 7 — УЗИ сердца
            issledCodes.Add("021773", 8); // 8 — УЗИ щитовидной железы
            issledCodes.Add("021775", 9); // 9 — УЗИ органов репродуктивной сферы
            issledCodes.Add("3", 10); // 10 — УЗИ тазобедренных суставов
            issledCodes.Add("021781", 11); // 11 — Нейросонография
            issledCodes.Add("063434", 12); // 12 — Флюорография
            issledCodes.Add("021774", 13); // 13 — Электрокардиография
            issledCodes.Add("4", 14); // 14 — Неонатальный скрининг на врожденный гипотиреоз, фенилкетонурию, адреногенитальный синдром, муковисцидоз и галактоземию
            issledCodes.Add("5", 15); // 15 — Аудиологический скрининг
            issledCodes.Add("6", 16); // 16 — Анализ кала на яйца глистов
            issledCodes.Add("7", 17); // 17 — Анализ окиси углерода выдыхаемого воздуха с определением карбоксигемоглобина
            issledCodes.Add("8", 18); // 18 — УЗИ почек
            issledCodes.Add("9", 19); // 19 — УЗИ печени

            var osmotriCodes = new Dictionary<string, int>();
            osmotriCodes.Add("161014", 1); // 1 — Педиатр
            osmotriCodes.Add("161001", 2); // 2 — Невролог
            osmotriCodes.Add("161006", 3); // 3 — Офтальмолог
            osmotriCodes.Add("161031", 4); // 4 — Детский хирург
            osmotriCodes.Add("161010", 5); // 5 — Оториноларинголог
            osmotriCodes.Add("161042", 6); // 6 — Травмотолог-ортопед
            osmotriCodes.Add("161045", 7); // 7 — Психиатр
            osmotriCodes.Add("161046", 8); // 8 — Детский стоматолог
            osmotriCodes.Add("161036", 9); // 9 — Детский эндокринолог
            osmotriCodes.Add("161047", 10); // 10 — Детский уролог-андролог
            osmotriCodes.Add("161026", 11); // 11 — Акушер-гинеколог

            var basicIss = new XElement("basic");
            
            foreach (var dispService in this.Services)
            {
                var record = new XElement("record");
                if (issledCodes.ContainsKey(dispService.Usluga.Code))
                {
                    record.Add(new XElement("id", issledCodes[dispService.Usluga.Code]));
                    var dateProtocol = dispService.EditableProtocol.Records.FirstOrDefault(t => t.Type.Code.Equals("34"));
                    DateTime date = DateTime.Parse(dateProtocol.Value);
                    record.Add(new XElement("date", date.ToString("yyyy-MM-ddZ")));
                    var result = dispService.EditableProtocol.Records.FirstOrDefault(t => t.Type.Code.Equals("35"));
                    record.Add(new XElement("result", result.Value));

                    basicIss.Add(record);
                }
                if (osmotriCodes.ContainsKey(dispService.Usluga.Code))
                {
                    record.Add(new XElement("id", osmotriCodes[dispService.Usluga.Code]));
                    record.Add(new XElement("date", dispService.DateIn.Date.ToString("yyyy-MM-ddZ")));

                    osmotri.Add(record);
                }
            }
            if (basicIss.HasElements)
                issled.Add(basicIss);
            card.Add(issled);

            card.Add(new XElement("zakluchDate", this.DateOut.Date.ToString("yyyy-MM-ddZ")));

            var zakluchDoctor = new XElement("zakluchVrachName");
            zakluchDoctor.Add(new XElement("last", pediatrService.Doctor.LastName));
            zakluchDoctor.Add(new XElement("first", pediatrService.Doctor.FirstName));
            if (!string.IsNullOrEmpty(pediatrService.Doctor.MiddleName))
                zakluchDoctor.Add(new XElement("middle", pediatrService.Doctor.MiddleName));

            card.Add(zakluchDoctor);
            /*
            osmotri
            Осмотры врачей
             * record >
                * id
                        Идентификатор осмотра
                        1 — Педиатр
                        2 — Невролог
                        3 — Офтальмолог
                        4 — Детский хирург
                        5 — Оториноларинголог
                        6 — Травмотолог-ортопед
                        7 — Психиатр
                        8 — Детский стоматолог
                        9 — Детский эндокринолог
                        10 — Детский уролог-андролог
                        11 — Акушер-гинеколог
                * date
                        Дата осмотра
             */
            card.Add(osmotri);

            //recommendZOZH

            card.Add(new XElement("recommendZOZH", pediatrService.CommonProtocol.Recommendation));

            /*
            reabilitation minOccurs="0" >
            Программа реабилитация
             * date
                  Дата назначения
             * state
                  Степень выполнения:
                  1 — полностью
                  2 — частично
                  3 — начато
                  4 — не выполнено
            */
            if (this.Rehab.IsNeed)
            {
                var rehab = new XElement("reabilitation");

                rehab.Add(new XElement("date", this.Rehab.SetDate.Date.ToString("yyyy-MM-ddZ")));
                rehab.Add(new XElement("state", (int)this.Rehab.Progress + 1));

                card.Add(rehab);
            }

            /*
            privivki
            Проведение вакцинации
             * state
                  1 — привит по возрасту
                  2 — не привит по медицинским показаниям: полностью
                  3 — не привит по медицинским показаниям: частично
                  4 — не привит по другим причинам: полностью
                  5 — не привит по другим причинам: частично
             * privs" minOccurs="0"
                  Нуждается в проведении вакцинации/ревакцинации
                * priv
                        6 — БЦЖ - V
                        7 — БЦЖ - R1
                        8 — БЦЖ - R2
                        9 — Полиомиелит - V1
                        10 — Полиомиелит - V2
                        11 — Полиомиелит - V3
                        12 — Полиомиелит - R1
                        13 — Полиомиелит - R2
                        14 — Полиомиелит - R3
                        15 — АКДС - V1
                        16 — АКДС - V2
                        17 — АКДС - V3
                        18 — АКДС - АДСМ
                        19 — АКДС - АДМ
                        20 — Корь - V
                        21 — Корь - R
                        22 — Эпид.паротит - V
                        23 — Эпид.паротит - R
                        24 — Краснуха - V
                        25 — Краснуха - R
                        26 — Гепатит В - V1
                        27 — Гепатит В - V2
                        28 — Гепатит В - V3
             */

            var privivkiProtocol = protocols.FirstOrDefault(t => t.Type.Code.Equals("32"));
            var privivki = new XElement("privivki");
            if (privivkiProtocol != null)
            {
                var stateProtocol = privivkiProtocol._children.First(t => t.Type.Code.Equals("32.1"));

                var state = stateProtocol.Value;

                var listValue = stateProtocol.Type.ListValues.First(t => t.Value.Equals(state));

                privivki.Add(new XElement("state", (stateProtocol.Type.ListValues.IndexOf(listValue) + 1)));

                
                var vactinationsProt = privivkiProtocol._children.FirstOrDefault(t => t.Type.Code.Equals("32.2"));
                if (vactinationsProt != null)
                {
                    var vactinations = vactinationsProt.Value;

                    var rootPrivs = new XElement("privs");
                    foreach (var vact in privivkiProtocol.Type.ListValues)
                    {
                        if (vactinations.Contains(vact.Value))
                            rootPrivs.Add(new XElement("priv", getPrivCode(vact.Value)));
                    }

                    if (rootPrivs.HasElements)
                        privivki.Add(rootPrivs);
                }
            }
            else
                privivki.Add(new XElement("state", 1)); // пока не сделаем привики!

            card.Add(privivki);

            /* oms
            Состояние оплаты ОМС:
            0 — не указано
            1 — оплачено
            2 — не оплачено
             */

            if (this.StatusOplati == Oplata.Otkaz)
                card.Add(new XElement("oms", 2));

            if (this.StatusOplati == Oplata.Polnaya)
                card.Add(new XElement("oms", 1));

            if (this.StatusOplati == Oplata.NetResheniya)
                card.Add(new XElement("oms", 0));

            return card;
        }

        private int getIllnessCode(string value)
        {
            /* illnesses
            Заболевания, обусловившие возникновение инвалидности
            1 — Некоторые инфекционные и паразитарные, из них:
            2 — туберкулез
            3 — сифилис
            4 — ВИЧ
            5 — Новообразования
            6 — Болезни крови, кроветворных органов и отдельные нарушения, вовлекающие имунный механизм, в том числе:
            9 — СПИД
            10 — Болезни эндокринной системы, расстройства питания и нарушения обмена веществ, из них:
            13 — сахарный диабет
            14 — Психические расстройства и расстройства поведения, в том числе:
            15 — умственная отсталость
            16 — Болезни нервной системы, из них:
            17 — церебральный паралич и др. паралитические синдромы
            18 — Болезни глаза и его придаточного аппарата
            19 — Болезни уха и сосцевидного отростка
            20 — Болезни системы кровообращения
            21 — Болезни органов дыхания, из них:
            22 — астма
            23 — астматический статус
            24 — Болезни органов пищеварения
            25 — Болезни кожи и подкожной клетчатки
            26 — Болезни костно-мышечной системы и соединительной ткани
            27 — Болезни мочеполовой системы
            28 — Отдельные состояния, возникающие в перинатальном периоде
            29 — Врожденные аномалии, из них:
            30 — аномалии нервной системы
            31 — аномалии системы кровообращения
            32 — аномалии опорно-двигательного аппарата
            33 — Последствия травм, отравлений и других воздействий внешних причин
             */
            var normalValue = value.ToLower();
            if (normalValue.Equals("некоторые инфекционные и паразитарные, из них:")) return 1;
            if (normalValue.Equals("туберкулез")) return 2;
            if (normalValue.Equals("сифилис")) return 3;
            if (normalValue.Equals("вич")) return 4;
            if (normalValue.Equals("новообразования")) return 5;
            if (normalValue.Equals("болезни крови, кроветворных органов и отдельные нарушения, вовлекающие имунный механизм, в том числе:")) return 6;
            if (normalValue.Equals("спид")) return 9;
            if (normalValue.Equals("болезни эндокринной системы, расстройства питания и нарушения обмена веществ, из них:")) return 10;
            if (normalValue.Equals("сахарный диабет")) return 13;
            if (normalValue.Equals("психические расстройства и расстройства поведения, в том числе:")) return 14;
            if (normalValue.Equals("умственная отсталость")) return 15;
            if (normalValue.Equals("болезни нервной системы, из них:")) return 16;
            if (normalValue.Equals("церебральный паралич и др. паралитические синдромы")) return 17;
            if (normalValue.Equals("болезни глаза и его придаточного аппарата")) return 18;
            if (normalValue.Equals("болезни уха и сосцевидного отростка")) return 19;
            if (normalValue.Equals("болезни системы кровообращения")) return 20;
            if (normalValue.Equals("болезни органов дыхания, из них:")) return 21;
            if (normalValue.Equals("астма")) return 22;
            if (normalValue.Equals("астматический статус")) return 23;
            if (normalValue.Equals("болезни органов пищеварения")) return 24;
            if (normalValue.Equals("болезни кожи и подкожной клетчатки")) return 25;
            if (normalValue.Equals("болезни костно-мышечной системы и соединительной ткани")) return 26;
            if (normalValue.Equals("болезни мочеполовой системы")) return 27;
            if (normalValue.Equals("отдельные состояния, возникающие в перинатальном периоде")) return 28;
            if (normalValue.Equals("врожденные аномалии, из них:")) return 29;
            if (normalValue.Equals("аномалии нервной системы")) return 30;
            if (normalValue.Equals("аномалии системы кровообращения")) return 31;
            if (normalValue.Equals("аномалии опорно-двигательного аппарата")) return 32;
            if (normalValue.Equals("последствия травм, отравлений и других воздействий внешних причин")) return 33;
            return - 1;
        }

        private int getDefectsCode(string value)
        {
            /* defects
            Виды нарушений в состоянии здоровья
            1 — умственные
            2 — другие психологические
            3 — языковые и речевые
            4 — слуховые и вестибулярные
            5 — зрительные
            6 — висцеральные и метаболические расстройства питания
            7 — двигательные
            8 — уродующие
            9 — общие и генерализованные
             */
            var normalValue = value.ToLower();
            if (normalValue.Equals("умственные")) return 1;
            if (normalValue.Equals("другие психологические")) return 2;
            if (normalValue.Equals("языковые и речевые")) return 3;
            if (normalValue.Equals("слуховые и вестибулярные")) return 4;
            if (normalValue.Equals("зрительные")) return 5;
            if (normalValue.Equals("висцеральные и метаболические расстройства питания")) return 6;
            if (normalValue.Equals("двигательные")) return 7;
            if (normalValue.Equals("уродующие")) return 8;
            if (normalValue.Equals("общие и генерализованные")) return 9;
            return -1;
        }

        private int getPrivCode(string value)
        {
            var normalValue = value.ToLower();

            /*
            6 — бцж - v
            7 — бцж - r1
            8 — бцж - r2
            */

            if (normalValue.Contains("бцж"))
            {
                if (normalValue.Contains("v"))
                    return 6;
                if (normalValue.Contains("r1"))
                    return 7;
                if (normalValue.Contains("r2"))
                    return 8;
            }
            /*
            9 — полиомиелит - v1
            10 — полиомиелит - v2
            11 — полиомиелит - v3
            12 — полиомиелит - r1
            13 — полиомиелит - r2
            14 — полиомиелит - r3
             */
            if (normalValue.Contains("полиомиелит"))
            {
                if (normalValue.Contains("v1"))
                    return 9;
                if (normalValue.Contains("v2"))
                    return 10;
                if (normalValue.Contains("v3"))
                    return 11;

                if (normalValue.Contains("r1"))
                    return 12;
                if (normalValue.Contains("r2"))
                    return 13;
                if (normalValue.Contains("r3"))
                    return 14;
            }

            /*
            15 — акдс - v1
            16 — акдс - v2
            17 — акдс - v3
            18 — акдс - адсм
            19 — акдс - адм
            20 — корь - v
            21 — корь - r
             */
            if (normalValue.Contains("акдс"))
            {
                if (normalValue.Contains("v1"))
                    return 15;
                if (normalValue.Contains("v2"))
                    return 16;
                if (normalValue.Contains("v3"))
                    return 17;

                if (normalValue.Contains("адсм"))
                    return 18;
                if (normalValue.Contains("адм"))
                    return 19;
            }

            /*
            22 — эпид.паротит - v
            23 — эпид.паротит - r
            24 — краснуха - v
            25 — краснуха - r
            26 — гепатит в - v1
            27 — гепатит в - v2
            28 — гепатит в - v3 
             */

            if (normalValue.Contains("паротит"))
            {
                if (normalValue.Contains("v"))
                    return 22;
                return 23;
            }

            if (normalValue.Contains("краснуха"))
            {
                if (normalValue.Contains("v"))
                    return 24;
                return 25;
            }

            if (normalValue.Contains("гепатит"))
            {
                if (normalValue.Contains("v1"))
                    return 26;
                if (normalValue.Contains("v2"))
                    return 27;
                if (normalValue.Contains("v3"))
                    return 28;
            }

            return -1;
        }

        public XElement GetChildBlock()
        {
            var pacient = Pacient;

            var childBlock = new XElement("child");

            /*
            idInternal minOccurs="0"
            Внутренний идентификатор карты ребёнка предоставляющей системы
            */
            childBlock.Add(new XElement("idInternal", pacient.Oid.ToString()));

            /*
            idType
            Тип карты ребёнка: 1 — сирота, 3 — несовершенолетний
            */
            childBlock.Add(new XElement("idType", 3));
            /*
            name >
            Фамилия, имя, отчество
            Данное поле может быть изменено при повторной загрузке.
             * last
                  Фамилия
             * first
                  Имя
             * middle
                  Отчество
             */
            var name = new XElement("name");
            name.Add(new XElement("last", pacient.LastName));
            name.Add(new XElement("first", pacient.FirstName));
            if (!string.IsNullOrEmpty(pacient.MiddleName))
                name.Add(new XElement("middle", pacient.MiddleName));

            childBlock.Add(name);
            /*
            idSex
            Пол ребёнка: 1 — мужской, 2 — женский
            Данное поле может быть изменено при повторной загрузке.
             */
            childBlock.Add(new XElement("idSex", pacient.Gender == Gender.Male ? 1 : 2));
            /*
            dateOfBirth
            Дата рождения
            */
            childBlock.Add(new XElement("dateOfBirth", pacient.Birthdate.Value.Date.ToString("yyyy-MM-ddZ")));

            /*
            idCategory
            Категория ребёнка:
            1 — ребёнок-сирота
            2 — ребёнок, находящийся в трудной жизненной ситуации
            3 — ребёнок, оставшийся без попечения родителей
            4 — нет категории
            Данное поле может быть изменено при повторной загрузке.
             */
            childBlock.Add(new XElement("idCategory", 4));
            /*
            idDocument
            Документ, удостоверяющий личность:
            3 — Свидетельство о рождении
            14 — Паспорт гражданина РФ
            documentSer
            Серия документа, удостоверяющий личность
            documentNum
            Номер документа, удостоверяющий личность
             */
            if (pacient.Document.Type.Code == 3 || pacient.Document.Type.Code == 14)
            {
                childBlock.Add(new XElement("idDocument", pacient.Document.Type.Code));
                childBlock.Add(new XElement("documentSer", pacient.Document.Serial));
                childBlock.Add(new XElement("documentNum", pacient.Document.Number));
            }
            /*
            snils minOccurs="0"
            Номер СНИЛС
            [0-9]{3}-[0-9]{3}-[0-9]{3}-[0-9]{2}
            */
            if (pacient.SNILS!=null)
                childBlock.Add(new XElement("snils", pacient.SNILS));

            /*
            idPolisType minOccurs="0"
            Формат страхового полиса:
            1 — Полис старого образца
            2 — Полис нового образца
            Если поле опущено, принимается значение 2.
            polisSer minOccurs="0"
            Серия страхового полиса
            polisNum
            Номер страхового полиса
            idInsuranceCompany
            Справочный идентификатор страховой компании
             */
            if (pacient.CurrentPolis.Type.Code.Equals("1") || pacient.CurrentPolis.Type.Code.Equals("3"))
            {
                if (pacient.CurrentPolis.Type.Code.Equals("1"))
                {
                    childBlock.Add(new XElement("idPolisType", pacient.CurrentPolis.Type.Code));
                    childBlock.Add(new XElement("polisSer", pacient.CurrentPolis.Serial));
                }
                else
                {
                    childBlock.Add(new XElement("polisSer", "-"));
                }
                childBlock.Add(new XElement("polisNum", pacient.CurrentPolis.Number));

                var code = string.Empty;
                switch (pacient.CurrentPolis.SMO.Code)
                {
                    case "03101":
                        code = "115";
                        break;
                    case "03102":
                        code = "91";
                        break;
                    case "03103":
                        code = "108";
                        break;
                    default:
                        code = pacient.CurrentPolis.SMO.Code;
                        break;
                }
                childBlock.Add(new XElement("idInsuranceCompany", code));
            }

            /*
            medSanName
            Наименование медицинской организации, выбранной для получения первичной медико-санитарной помощи
            medSanAddress
            Юридический адрес медицинской организации, выбранной для получения первичной медико-санитарной помощи
             */

            var org = Session.FindObject<MedOrg>(CriteriaOperator.Parse("Code=?", Settings.MOSettings.GetCurrentMOCode(Session)));
            childBlock.Add(new XElement("medSanName", org.FullName));
            childBlock.Add(new XElement("medSanAddress", org.AddressJur));

            /*
            address >
            Адрес места постоянного пребывания
             * index" minOccurs="0"
                  Почтовый индекс
                  [1-9][0-9]{5}
             * kladrNP
                  Код населённого пункта проживания по КЛАДР
                  [0-9]{13}
             * kladrStreet" minOccurs="0"
                  Код улицы проживания по КЛАДР
                  [0-9]{17}
             * house minOccurs="0"
                  Номер дома
             * building minOccurs="0"
                  Номер строения
             * appartment minOccurs="0"
                  Номер квартиры
             */
            var address = pacient.Address;

            var addressElement = new XElement("address");

            var postCode = address.GetPostCode();
            addressElement.Add(new XElement("index", postCode));

            var kladrNp = new XElement("kladrNP");
            
            if (address.Level2 != null && address.Level2.IsCity)
            {
                kladrNp.Value = address.Level2.Code;
            }
            if (address.Level3 != null)
            {
                kladrNp.Value = address.Level3.Code;
            }
            if (address.Level4!=null)
                kladrNp.Value = address.Level4.Code;
            addressElement.Add(kladrNp);

            if (address.Street!=null)
                addressElement.Add(new XElement("kladrStreet", address.Street.Code));
            if (!string.IsNullOrEmpty(address.House))
                addressElement.Add(new XElement("house", address.House));
            if (!string.IsNullOrEmpty(address.Build))
                addressElement.Add(new XElement("building", address.Build));
            if (!string.IsNullOrEmpty(address.Flat))
                addressElement.Add(new XElement("appartment", address.Flat));
            childBlock.Add(addressElement);

            /*
            education minOccurs="0" >
            Данные образовательного учреждения
            Данное поле имеет приоритет над полем idEducationOrg.
            Данное поле может быть изменено при повторной загрузке.
             * kladrDistr minOccurs="0"
                  Код района по КЛАДР, в котором находится образовательное учреждение
                  <xs:pattern value="[0-9]{13}"
             * idEducType" minOccurs="0"
                  Вид образовательного учреждения:
                  1 — Дошкольные образовательные учреждения
                  2 — Общеобразовательные (начального общего, основного общего, среднего (полного) общего образования) образовательные учреждения
                  3 — Образовательные учреждения начального профессионального, среднего профессионального, высшего профессионального образования
                  4 — Специальные (коррекционные) образовательные учреждения для обучающихся, воспитанников с ограниченными возможностями здоровья
                  5 — Образовательные учреждения для детей-сирот и детей, оставшихся без попечения родителей (законных представителей)
             * educOrgName"
                  Наименование образовательного учреждения
            idEducationOrg minOccurs="0"
            Справочный идентификатор образовательного учреждения

            idOrphHabitation minOccurs="0"
            0 — стационарное учреждение
            1 — опека
            2 — попечительство
            3 — усыновление (удочерение)
            4 — передан в приёмную семью
            8 — передан в патронатную семью

            Поле заполняется обязательно для ребёнка-сироты.
            dateOrphHabitation minOccurs="0"
            Дата поступление в место текущего нахождения
            Поле заполняется обязательно для ребёнка-сироты.
            idStacOrg minOccurs="0"
            Справочный идентификатор стационарного учреждения
            Поле заполняется обязательно при значении idOrphHabitation равном 0.
            */

            var cards = new XElement("cards");

            foreach (var source in pacient.DispanserizaionCases.Where(t=>t.Type == DispType.ProfOsmotrChild))
            {
                try
                {
                    var card = source.GetCardBlock();
                    cards.Add(card);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }

            childBlock.Add(cards);

            return childBlock;
        }
    }
}
