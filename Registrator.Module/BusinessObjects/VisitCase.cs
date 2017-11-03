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

namespace Registrator.Module.BusinessObjects
{
    /// <summary>
    /// Посещение
    /// </summary>
    [DefaultClassOptions]
    [XafDisplayName("Посещение")]
    public class VisitCase : CommonCase
    {
        private MestoObsluzhivaniya _mesto;
        private const int minCodeForResultat = 301;
        private const int maxCodeForResultat = 315;

        public VisitCase(Session session)
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
        [Browsable(false)]
        public CelPosescheniya Cel { get; set; }

        /// <summary>
        /// Место обслуживания
        /// </summary>
        [XafDisplayName("Место обслуживания")]
        [Browsable(false)]
        public MestoObsluzhivaniya Mesto
        {
            get { return _mesto; }
            set
            {
                SetPropertyValue("Mesto", ref _mesto, value);
                OnChanged("Resultat");
            }
        }

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
            int index = Pacient.Cases.IndexOf(this);
            return GetReestrElement(index);
        }

        public override System.Xml.Linq.XElement GetReestrElement(int zapNumber)
        {
            // проверяем поля услуги
            if (IsValidForReestr() == false)
                return null;

            const string dateTimeFormat = "{0:yyyy-MM-dd}";

            XElement element = new XElement("SLUCH");

            // Номер записи в реестре случаев
            element.Add(new XElement("IDCASE", zapNumber));

            // Условия оказания мед. помощи
            element.Add(new XElement("USL_OK", this.UsloviyaPomoshi.Code));

            // Вид мед. помощи
            element.Add(new XElement("VIDPOM", this.VidPom.Code));

            // Форма мед. помощи
            element.Add(new XElement("FOR_POM", this.FormaPomoshi.Code));

            if (this.FromLPU!=null)
                // Направившее МО
                element.Add(new XElement("NRP_MO", this.FromLPU.Code));
            
            // Направление (госпитализация)
            //element.Add(new XElement("EXTR", ));

            // Код МО
            element.Add(new XElement("LPU", this.LPU.Code));

            if (!string.IsNullOrEmpty(this.LPU_1))
                // код подразделения МО
                element.Add(new XElement("LPU_1", this.LPU_1));

            if (this.Otdelenie != null)
                // Код отделения
                element.Add(new XElement("PODR", this.Otdelenie.Code));

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

            // 
            element.Add(new XElement("DS1", this.MainDiagnose.Diagnose.CODE));

            // Сопутствующие диагнозы
            foreach(var ds2 in this.SoputsDiagnoses)
                element.Add(new XElement("DS2", ds2.CODE));

            // Диагнозы осложнений
            foreach(var ds3 in this.OslozhDiagnoses)
                element.Add(new XElement("DS3", ds3.CODE));

            // проверить карту пациента
            if (this.VesPriRozhdenii != 0)
                // Вес при рождении
                element.Add(new XElement("VNOV_M", this.VesPriRozhdenii));

            /*// Коды МЭС
            element.Add(new XElement("CODE_MES1", ));

            // Коды МЭС сопутствующих заболеваний
            element.Add(new XElement("CODE_MES2", ));*/

            // Результат обращения 
            element.Add(new XElement("RSLT", this.Resultat.Code));

            // Исход заболевания
            element.Add(new XElement("ISHOD", this.Ishod.Code));

            // Специальность леч. врача
            element.Add(new XElement("PRVS", this.DoctorSpec.Code));

            // Код классификатора мед. спец-й
            element.Add(new XElement("VERS_SPEC", this.VersionSpecClassifier));

            // Код врача, закрывшего случай
            element.Add(new XElement("IDDOCT", this.Doctor.InnerCode));

            /*// Особые случаи
            element.Add(new XElement("OS_SLUCH", (int)this.OsobiySluchay));*/

            // Способ оплаты мед. помощи
            element.Add(new XElement("IDSP", this.SposobOplMedPom));

            /*// Кол-во единиц оплаты мед. помощи
            element.Add(new XElement("ED_COL", this.MedPomCount));*/

            if (this.Tarif!=0)
                // Тариф
                element.Add(new XElement("TARIF", this.Tarif));

            // Сумма
            element.Add(new XElement("SUMV", this.TotalSum));

            // Тип оплаты
            element.Add(new XElement("OPLATA", (int)this.StatusOplati));

            // Данные по услугам
            int serviceCounter = 1;
            foreach (var usl in Services)
                element.Add(new XElement("USL", usl.GetReestrElement(serviceCounter++)));

            if (!string.IsNullOrEmpty(this.Comment))
                // Служебное поле
                element.Add(new XElement("COMMENTSL", this.Comment));

            return element;
        }

        /// <summary>
        /// Критерий диагнозов
        /// </summary>
        public override CriteriaOperator DiagnoseCriteria
        {
            get
            {
                // Выводим все диагнозы
                return CriteriaOperator.Parse("1=1");
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
