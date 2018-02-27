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
    /// ���������
    /// </summary>
    [DefaultClassOptions]
    [XafDisplayName("���������")]
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
             * Code = 29 - "�� ��������� � �����������"
             * Code = 30 - "�� ��������� (����������� ������) � �����������"
             */
            var sposobCode = (this.Cel == CelPosescheniya.ProfOsmotr) ? 29 : 30;
            this.SposobOplMedPom = Session.FindObject<Dictionaries.SposobOplatiMedPom>(CriteriaOperator.Parse("Code=?", sposobCode));

            /*
             * Code = 1 - "����������"
             * Code = 2 - "� ������� ����������"
             * Code = 3 - "�����������" 
             * Code = 4 - "��� ����������� �����������"
             */
            this.UsloviyaPomoshi = Session.FindObject<Dictionaries.VidUsloviyOkazMedPomoshi>(CriteriaOperator.Parse("Code=?", 3));

            /*
             * Code = "1" - "����������"
             * Code = "2" - "����������"
             * Code = "3" - "��������"
             */
            this.FormaPomoshi = Session.FindObject<Dictionaries.FormaMedPomoshi>(CriteriaOperator.Parse("Code=?", 3));

            if (Pacient != null)
            {
                this.DetProfil = Pacient.GetAge() >= 18 ? PriznakDetProfila.No : PriznakDetProfila.Yes;

                // ���� � �������� �������� ������ ����, �� ����������� ���. � ��������� �� ������������ (��������)
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
                     * ��������� ���.-���������� ������ (��� 1) ������������� ��� ���������� (��� 27), ���������� (��� 22), ������ ���. �������� (��� 16)
                     */
                    var vidPomoshiCode = 1;
                    if (DoctorSpec.Code == "16" || DoctorSpec.Code == "22" || DoctorSpec.Code == "27")
                    {
                        vidPomoshiCode = 1;
                    }
                    else
                    {
                        // ������ ��������
                    }
                    this.VidPom = Session.FindObject<Dictionaries.VidMedPomoshi>(CriteriaOperator.Parse("Code=?", vidPomoshiCode));
                }
            }
            
            // ������� ������ �������������� ��������������.
            this.VersionSpecClassifier = "V015";
            this.StatusOplati = Oplata.NetResheniya;
        }

        /// <summary>
        /// ���� ���������
        /// </summary>
        [XafDisplayName("���� ���������")]
        public CelPosescheniya Cel { get; set; }

        /// <summary>
        /// ����� ������������
        /// </summary>
        [XafDisplayName("����� ������������")]
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
        /// �������� �������
        /// </summary>
        [XafDisplayName("�������� �������")]
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
        /// ������� ������ ��������� � ������� ��� ��������
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
                
                // ���� ������ ����������� ��� ���, �� ����� ��������� ���������� ������ 4
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
            // ��������� ���� ������
            //if (IsValidForReestr() == false)
            //    return null;

            const int isBaby = 0;
            //string lpuCode = Settings.MOSettings.GetCurrentMOCode(Session);
            string lpuCode_1 = lpuCode;
            const string dateTimeFormat = "{0:yyyy-MM-dd}";
            const string decimalFormat = "n2";
            var age = this.Pacient.GetAge();

            var zap = new XElement("ZAP");
            // ����� ������ � �����
            zap.Add(new XElement("N_ZAP", zapNumber));
            // ������� ����� ������: 0, 1
            // � ����������� �� ���������� ������ (���� 0, �� ������ �����)
            zap.Add(new XElement("PR_NOV", 0)); // ResultatOplati - � ����������

            // ������ ��������
            var polis = this.Pacient.Polises.FirstOrDefault(t => (t.DateEnd == null) || (t.DateEnd != null && DateTime.Now <= t.DateEnd));
            zap.Add(new XElement("PACIENT",
                            new XElement("ID_PAC", Pacient.Oid), // GUID!
                             // ��� ������. �������������
                            new XElement("VPOLIS", polis != null && polis.Type != null ? polis.Type.Code : string.Empty),
                            // ����� ������
                            new XElement("SPOLIS", polis != null ? polis.Serial : string.Empty),
                            // ����� ������
                            new XElement("NPOLIS", polis != null ? polis.Number : string.Empty),
                            // ��� ���
                            new XElement("SMO", polis != null && polis.SMO != null ? polis.SMO.Code : string.Empty),
                            // ������� ��������������
                            new XElement("NOVOR", isBaby)));

            Decimal tarif = Settings.TarifSettings.GetDnevnoyStacionarTarif(Session);
            //var paymentCode = 43;
            var childFlag = (Pacient.GetAge() < 18) ? 1 : 0;

            XElement sluchElement = new XElement("SLUCH");
            // ����� ������ � ������� �������
            sluchElement.Add(new XElement("IDCASE", zapNumber));
            // ������� �������� ���. ������
            sluchElement.Add(new XElement("USL_OK", this.UsloviyaPomoshi.Code));
            // ��� ���. ������
            sluchElement.Add(new XElement("VIDPOM", this.VidPom.Code));
            // ����� ���. ������
            sluchElement.Add(new XElement("FOR_POM", this.FormaPomoshi.Code));

            // ����������� ��
            if (FromLPU != null)
                sluchElement.Add(new XElement("NRP_MO", this.FromLPU.Code));
            // ��� ��
            sluchElement.Add(new XElement("LPU", this.LPU.Code));

            string podr = lpuCode + (Profil != null ? (int?)Profil.Code : null) +
                (Otdelenie != null ? Otdelenie.Code : null);

            if (!string.IsNullOrEmpty(this.LPU_1))
                // ��� ������������� ��
                sluchElement.Add(new XElement("LPU_1", this.LPU_1));
            // ��� ���������
            sluchElement.Add(new XElement("PODR", podr));
            // �������
            if (Profil != null)
                sluchElement.Add(new XElement("PROFIL", Profil.Code));
            // ������� �������
            sluchElement.Add(new XElement("DET", (int)this.DetProfil));
            // ����� ������� �������/������ ������������� ��������
            sluchElement.Add(new XElement("NHISTORY", this.Oid));
            // ���� �������
            sluchElement.Add(new XElement("DATE_1", string.Format(dateTimeFormat, this.DateIn)));
            sluchElement.Add(new XElement("DATE_2", string.Format(dateTimeFormat, this.DateOut)));
            // ��������� �������
            if (PreDiagnose != null && PreDiagnose.Diagnose != null)
                sluchElement.Add(new XElement("DS0", PreDiagnose.Diagnose.MKB));
            // �������� �������
            if (MainDiagnose != null && MainDiagnose.Diagnose != null)
                sluchElement.Add(new XElement("DS1", MainDiagnose.Diagnose.MKB));

            // ������������� ��������
            foreach(var ds2 in SoputsDiagnoses)
                sluchElement.Add(new XElement("DS2", ds2.MKB));
            // �������� ����������
            foreach(var ds3 in OslozhDiagnoses)
                sluchElement.Add(new XElement("DS3", ds3.MKB));

            // ��������� ����� ��������
            // ��� ��� ��������
            if (this.VesPriRozhdenii != 0)
                sluchElement.Add(new XElement("VNOV_M", this.VesPriRozhdenii));

            /*// ���� ���
            element.Add(new XElement("CODE_MES1", ));

            // ���� ��� ������������� �����������
            element.Add(new XElement("CODE_MES2", ));*/

            // ��������� ��������� 
            sluchElement.Add(new XElement("RSLT", Resultat != null ? Resultat.Code : ""));
            // ����� �����������
            sluchElement.Add(new XElement("ISHOD", Ishod != null ? Ishod.Code : ""));
            // ������������� ���. �����
            sluchElement.Add(new XElement("PRVS", this.DoctorSpec.Code));
            // ��� �������������� ���. ����-�
            sluchElement.Add(new XElement("VERS_SPEC", this.VersionSpecClassifier));
            // ��� �����, ���������� ������
            sluchElement.Add(new XElement("IDDOCT", this.Doctor.SNILS));

            // ������ ������
            //sluchElement.Add(new XElement("OS_SLUCH", (int)this.OsobiySluchay));

            // ������ ������ ���. ������
            if (SposobOplMedPom != null)
                sluchElement.Add(new XElement("IDSP", this.SposobOplMedPom.Code));

            /*// ���-�� ������ ������ ���. ������
            element.Add(new XElement("ED_COL", this.MedPomCount));*/

            // �����
            if (this.Tarif != 0)
                sluchElement.Add(new XElement("TARIF", this.Tarif));
            // �����
            sluchElement.Add(new XElement("SUMV", this.TotalSum.ToString(decimalFormat).Replace(",", ".")));
            // ��� ������
            sluchElement.Add(new XElement("OPLATA", (int)this.StatusOplati));

            // ������ �� �������
            int serviceCounter = 1;
            foreach (var usl in Services.OfType<MedService>())
                sluchElement.Add(usl.GetReestrElement(serviceCounter++, lpuCode));

            if (!string.IsNullOrEmpty(this.Comment))
                // ��������� ����
                sluchElement.Add(new XElement("COMMENTSL", this.Comment));

            zap.Add(sluchElement);
            return zap;
        }

        /// <summary>
        /// �������� ���������
        /// </summary>
        public override CriteriaOperator DiagnoseCriteria
        {
            get { return CriteriaOperator.Parse("1=1"); }
        }

        #endregion
    }
}
