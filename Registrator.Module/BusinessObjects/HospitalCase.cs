using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using Registrator.Module.BusinessObjects.Abstract;
using Registrator.Module.BusinessObjects.Interfaces;
using System.Xml.Linq;
using Registrator.Module.BusinessObjects.Dictionaries;

namespace Registrator.Module.BusinessObjects
{
    /// <summary>
    /// Посещение
    /// </summary>
    /// <ToDo>Добавить основной диагноз (MainDiagnose) в HospitalCase</ToDo>
    [DefaultClassOptions]
    [XafDisplayName("Госпитализация")]
    public class HospitalCase : CommonCase
    {
        public HospitalCase(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            // определяем текущего пользователя
            var createdBy = SecuritySystem.CurrentUser as Doctor;
            if (createdBy != null)
            {
                // находим доктора с таким же Логином
                Doctor = Session.FindObject<Doctor>(CriteriaOperator.Parse("UserName=?", createdBy.UserName));
            }

            string MOCode = Settings.MOSettings.GetCurrentMOCode(Session);
            this.LPU = Session.FindObject<MedOrg>(CriteriaOperator.Parse("Code=?", MOCode));
            this.LPU_1 = MOCode;

            // пока используем ГУИД идентификатор объекта
            this.NHistory = this.Oid.ToString();

            /*
             * Code = 29 - "За посещение в поликлинике"
             * Code = 30 - "За обращение (законченный случай) в поликлинике"
             */
            /*var sposobCode = (this.cel == CelPosescheniya.ProfOsmotr) ? 29 : 30;
            this.SposobOplMedPom = Session.FindObject<Dictionaries.SposobOplatiMedPom>(CriteriaOperator.Parse("Code=?", sposobCode));*/

            /*
             * Code = 1 - "Стационаро"
             * Code = 2 - "В дневном стационаре"
             * Code = 3 - "Амбулаторно" 
             * Code = 4 - "Вне медицинской организации"
             */
            this.UsloviyaPomoshi = Session.FindObject<Dictionaries.VidUsloviyOkazMedPomoshi>(CriteriaOperator.Parse("Code=?", 1));

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
                //    VesPriRozhdenii = 0;
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

            this.Hospitalizacia = Napravlenie.Planovaya;

            // Проверяем, что услуга по умолчанию создана, если нет - создаем
            if (Services.Count == 0)
            {
                Services.Add(new MedService(Session) { IsMainService = true, AutoOpen = false });
            }

            // связываем данные с аггрегированным свойством, чтобы иметь возможность добавлять поля услуги во View случая
            Service = Services[0];
        }

        [XafDisplayName("Направление/Госпитализация")]
        public Napravlenie Hospitalizacia { get; set; }

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

        public override CriteriaOperator DiagnoseCriteria
        {
            get
            {
                // получаем все инстансы объекта КСГ
                // используем след. критерий для получения всех инстансев
                var KSGs = new XPCollection<ClinicStatGroups>(Session).ToList();
                return new InOperator("MKB", KSGs.Select(t => t.Diagnose.MKB));
            }
        }

        /// <summary>
        /// У госпитализации одна услуга, которая устанавливается при создании случая
        /// </summary>
        [DevExpress.Xpo.Aggregated]
        public CommonService Service { get; set; }

        public override bool IsValidForReestr()
        {
            throw new NotImplementedException();
        }

        public override System.Xml.Linq.XElement GetReestrElement()
        {
            var cases = Pacient.Cases.ToList<AbstractCase>();
            int index = cases.IndexOf((AbstractCase)this);
            return GetReestrElement(index);
        }

        public System.Xml.Linq.XElement GetReestrElement(int zapNumber, string lpuCode = null)
        {
            // проверяем поля услуги
            if (IsValidForReestr() == false)
                return null;

            const string dateTimeFormat = "{0:yyyy-MM-dd}";
            //var ksg = Session.FindObject<ClinicStatGroups>(CriteriaOperator.Parse("MainDiagnose.MKB=?", this.MainDiagnose.Diagnose.MKB));
            Decimal tarif = Settings.TarifSettings.GetDnevnoyStacionarTarif(Session);

            XElement element = new XElement("SLUCH");

            // Номер записи в реестре случаев
            element.Add(new XElement("IDCASE", zapNumber));

            // Условия оказания мед. помощи
            element.Add(new XElement("USL_OK", this.UsloviyaPomoshi.Code));

            // Вид мед. помощи
            element.Add(new XElement("VIDPOM", this.VidPom.Code));

            // Форма мед. помощи
            element.Add(new XElement("FOR_POM", this.FormaPomoshi.Code));

            if (this.FromLPU != null)
                // Направившее МО
                element.Add(new XElement("NPR_MO", this.FromLPU.Code));

            // Направление (госпитализация)
            element.Add(new XElement("EXTR", (int)this.Hospitalizacia));

            // Код МО
            element.Add(new XElement("LPU", this.LPU.Code));

            if (!string.IsNullOrEmpty(this.LPU_1))
                // код подразделения МО
                element.Add(new XElement("LPU_1", this.LPU_1));

            string podr = lpuCode + (Profil != null ? (int?)Profil.Code : null) +
                            (Otdelenie != null ? Otdelenie.Code : null);

            // Код отделения
            element.Add(new XElement("PODR", podr));

            // Профиль
            element.Add(new XElement("PROFIL", this.Profil.Code));

            // Детский профиль
            element.Add(new XElement("DET", (int)this.DetProfil));

            // Номер истории болезни/талона амбулаторного пациента
            element.Add(new XElement("NHISTORY", this.Oid));

            // Даты лечения
            element.Add(new XElement("DATE_1", string.Format(dateTimeFormat, this.DateIn)));
            element.Add(new XElement("DATE_2", string.Format(dateTimeFormat, this.DateOut)));

            if (this.PreDiagnose != null)
                // Первичный диагноз
                element.Add(new XElement("DS0", this.PreDiagnose.Diagnose.CODE));

            // todo: Добавить основной диагноз (MainDiagnose) в HospitalCase !!!!
          //  element.Add(new XElement("DS1", this.MainDiagnose.Diagnose.CODE));

            // Сопутствующие диагнозы
            foreach (var ds2 in this.SoputsDiagnoses)
                element.Add(new XElement("DS2", ds2.CODE));

            // Диагнозы осложнений
            foreach (var ds3 in this.OslozhDiagnoses)
                element.Add(new XElement("DS3", ds3.CODE));

            // проверить карту пациента
            if (this.VesPriRozhdenii != 0)
                // Вес при рождении
                element.Add(new XElement("VNOV_M", this.VesPriRozhdenii));

            // Коды МЭС !!!
            //element.Add( new XElement("CODE_MES1", ksg.Number));

            // Коды МЭС сопутствующих заболеваний
            //element.Add(new XElement("CODE_MES2", ));

            // Результат обращения 
            element.Add(new XElement("RSLT", this.Resultat.Code));

            // Исход заболевания
            element.Add(new XElement("ISHOD", this.Ishod.Code));

            // Специальность леч. врача
            element.Add(new XElement("PRVS", this.DoctorSpec.Code));

            // Код классификатора мед. спец-й
            element.Add(new XElement("VERS_SPEC", this.VersionSpecClassifier));

            // Код врача, закрывшего случай
            element.Add(new XElement("IDDOKT", this.Doctor.SNILS));

            /*// Особые случаи
            element.Add(new XElement("OS_SLUCH", (int)this.OsobiySluchay));*/

            // Способ оплаты мед. помощи
            element.Add(new XElement("IDSP", this.SposobOplMedPom));

            // Кол-во единиц оплаты мед. помощи
            element.Add(new XElement("ED_COL", this.MedPomCount));

            //!!!!
            //this.Tarif = tarif * Convert.ToDecimal(ksg.KoeffZatrat);
            // Тариф
            element.Add(new XElement("TARIF", this.Tarif));

            // Сумма
            element.Add(new XElement("SUMV", this.TotalSum));

            // Тип оплаты
            element.Add(new XElement("OPLATA", (int)this.StatusOplati));

            // Данные по услугам
            int serviceCounter = 1;
            foreach (var usl in Services.OfType<MedService>())
                element.Add(new XElement("USL", usl.GetReestrElement(serviceCounter++, lpuCode)));

            if (!string.IsNullOrEmpty(this.Comment))
                // Служебное поле
                element.Add(new XElement("COMMENTSL", this.Comment));

            return element;
        }
    }
}
