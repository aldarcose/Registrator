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
    [XafDisplayName("���������������")]
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

        [XafDisplayName("������ �������� �� ������������")]
        public HealthGroupObs HealthGroupObsBefore { get; set; }
        [XafDisplayName("������ �������� ��� ������� ���. ��������� �� ������������")]
        public HealthGroupForSportObs HealthGroupForSportObsBefore { get; set; }

        [XafDisplayName("������ �������� ����� ������������")]
        public HealthGroupObs HealthGroupObsAfter { get; set; }
        [XafDisplayName("������ �������� ��� ������� ���. ��������� ����� ������������")]
        public HealthGroupForSportObs HealthGroupForSportObsAfter { get; set; }

        [XafDisplayName("������� ������")]
        public bool IsPacientHealthy { get; set; }

        [XafDisplayName("��� �������")]
        public MKB10 InspectionResult { get; set; }

        public void AddDefaultServices(Pacient pacient, DateTime? date = null)
        {
            // �������� ������ ������ �� ������� ��� �������� ���� ���������������
            CriteriaOperator criteria = CriteriaOperator.Parse("Type=?", Type);
            var servicesToAddInfo = new XPCollection<DispsServiceList>(Session, criteria).ToList();
            if (servicesToAddInfo.Count > 0)
            {
                // ������� ����� ��� ���, ������� ������������� �������� ���������
                var serviceToAddInfo = servicesToAddInfo.FirstOrDefault(
                    t => t.CheckPacient(pacient, date));

                // ���� ���� �� ������� ��� ������� ���� ��������������� �������
                if (serviceToAddInfo != null)
                {
                    // ��������� ������ � ������
                    foreach (var serviceWithInfo in serviceToAddInfo.Services)
                    {
                        var myService = new DispanserizationService(Session);

                        if (date.HasValue)
                        {
                            myService.DateIn = date.Value;
                        }

                        // ��������� � ������
                        this.Services.Add(myService);

                        // ����������� ������. ��������� ��������, ������� ���������� ������
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
            // ����� ������ � �����
            xZap.Add(new XElement("N_ZAP", zapNumber));
            // ������� ����� ������: 0, 1
            // � ����������� �� ���������� ������ (���� 0, �� ������ �����)
            xZap.Add(new XElement("PR_NOV", 0));

            // ������ ��������
            var polis = this.Pacient.Polises.FirstOrDefault(t => (t.DateEnd == null) || (t.DateEnd != null && DateTime.Now <= t.DateEnd));
            xZap.Add(new XElement("PACIENT",
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
            // sluchElement.Add(new XElement("USL_OK", ));
            // ��� ���. ������
            if (VidPom != null)
                sluchElement.Add(new XElement("VIDPOM", VidPom.Code));
            // ����� ���. ������
            // sluchElement.Add(new XElement("FOR_POM", ));

            // ����������� ��
            //if (FromLPU != null)
            //    sluchElement.Add(new XElement("NPR_MO", this.FromLPU.Code));
            // ��� ��
            sluchElement.Add(new XElement("LPU", this.LPU.Code));

            if (!string.IsNullOrEmpty(this.LPU_1))
                // ��� ������������� ��
                sluchElement.Add(new XElement("LPU_1", this.LPU_1));
            // ��� ���������
            //if (Otdelenie != null)
            //    sluchElement.Add(new XElement("PODR", this.Otdelenie.Code));
            // �������
            //if (Profil != null)
            //    sluchElement.Add(new XElement("PROFIL", Profil.Code));
            // ������� �������
            //sluchElement.Add(new XElement("DET", (int)this.DetProfil));
            // ����� ������� �������/������ ������������� ��������
            sluchElement.Add(new XElement("NHISTORY", this.Oid));
            // ���� �������
            sluchElement.Add(new XElement("DATE_1", string.Format(dateTimeFormat, this.DateIn)));
            sluchElement.Add(new XElement("DATE_2", string.Format(dateTimeFormat, this.DateOut)));
            // ��������� �������
            if (PreDiagnose != null && PreDiagnose.Diagnose != null)
                sluchElement.Add(new XElement("DS0", PreDiagnose.Diagnose.CODE));
            // �������� �������
            //if (MainDiagnose != null && MainDiagnose.Diagnose != null)
            //    sluchElement.Add(new XElement("DS1", MainDiagnose.Diagnose.CODE));

            // ������������� ��������
            //foreach (var ds2 in SoputsDiagnoses)
            //    sluchElement.Add(new XElement("DS2", ds2.CODE));
            // �������� ����������
            //foreach (var ds3 in OslozhDiagnoses)
            //    sluchElement.Add(new XElement("DS3", ds3.CODE));

            // ��������� ����� ��������
            // ��� ��� ��������
            //if (this.VesPriRozhdenii != 0)
            //    sluchElement.Add(new XElement("VNOV_M", this.VesPriRozhdenii));

            /*// ���� ���
            element.Add(new XElement("CODE_MES1", ));

            // ���� ��� ������������� �����������
            element.Add(new XElement("CODE_MES2", ));*/

            // ��������� ��������� 
          //  sluchElement.Add(new XElement("RSLT", this.Resultat.Code));
            // ����� �����������
           // sluchElement.Add(new XElement("ISHOD", this.Ishod.Code));
            // ������������� ���. �����
           // sluchElement.Add(new XElement("PRVS", this.DoctorSpec.Code));
            // ��� �������������� ���. ����-�
           // sluchElement.Add(new XElement("VERS_SPEC", this.VersionSpecClassifier));
            // ��� �����, ���������� ������
           // sluchElement.Add(new XElement("IDDOKT", this.Doctor.InnerCode));

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
            foreach (var usl in Services.OfType<DispanserizationService>())
                sluchElement.Add(usl.GetReestrElement(serviceCounter++, lpuCode));

            if (!string.IsNullOrEmpty(this.Comment))
                // ��������� ����
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

             /* idType - ��� ����� ������������
             * ��� �����-�����:
             * 1 � ����� ���������������
             * ��� ����:
             * 2 � ���������������� ������
             * 3 � ��������������� ������ (��� �������� ���������������� ����������)
             * 4 � ������������� ������ (��� �������� ���������������� ����������)
             */

            int idType = 1;
            switch (this.Type)
            {
                case DispType.ProfOsmotrAdult:
                    // ������ ����
                    return null;
                case DispType.DOGVN1:
                    // ������ ����
                    return null;
                case DispType.DOGVN2:
                    // ������ ����
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
                    // ���� ����� �� �������
                    return null;
                case DispType.DispStacionarChildOrphan12:
                    // ���� ����� �� �������
                    return null;
                case DispType.DispChildOrphan1:
                    // ���� ����� �� �������
                    return null;
                case DispType.DispChildOrphan12:
                    // ���� ����� �� �������
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
                  1 � ������� ����� ����
                  2 � ������� ����� ����
                  3 � ������ ����
                  4 � ������� ����

                  1 � 2, 3 � 4 � ����������������� ��������.
            */
            var healthProblemsProtocol = protocols.FirstOrDefault(t => t.Type.Code.Equals("26"));
            var problem1 =
                healthProblemsProtocol._children.Where(t => t.Type.Code.Equals("26.1")).Select(t => t.Value).First();
            var problem2 =
                healthProblemsProtocol._children.Where(t => t.Type.Code.Equals("26.2")).Select(t => t.Value).First();
            
            bool hasProblems = false;
            var healthProblem = new XElement("healthProblems");
            if (!problem1.Equals("��� ���������"))
            {
                hasProblems = true;
                var probProtocol = healthProblemsProtocol._children.First(t => t.Type.Code.Equals("26.1"));
                var listValue = probProtocol.Type.ListValues.First(t => t.Value.Equals(problem1));
                healthProblem.Add(new XElement("problem", probProtocol.Type.ListValues.IndexOf(listValue)));
            }
            if (!problem2.Equals("��� ���������"))
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
            ������ �������� ������������ �������� ��� ����� �� 0 �� 4 ��� � �������
             * poznav
               �������������� �������
             * motor
               �������� �������
             * emot
               ������������� � ���������� (������� � ���������� �����) �������
             * rech
               ����������� � ������� ��������
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
            ������ ��������� ������������ �������� ��� ����� �� 5 ���
             * psihmot
               ������������� �����
             * intel
               ���������
             * emotveg
               ������������-������������ �����
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

                root.Add(new XElement("psihmot", psihmot.Equals("�����") ? 0 : 1));
                root.Add(new XElement("intel", intel.Equals("�����") ? 0 : 1));
                root.Add(new XElement("emotveg", emotveg.Equals("�����") ? 0 : 1));

                card.Add(root);
            }
            /*
            sexFormulaMale minOccurs="0"
            ������� ������� (���.)
            ���� ����������� ����������� ��� ��������� �� 10 ���
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
            ������� ������� (���.)
            ���� ����������� ����������� ��� ������� �� 10 ���          
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
            ������������� �������
            ���� ����������� ����������� ��� ������� �� 10 ���
             * menarhe
               Menarhe � �������
             * characters >
                * char
                ��������������
                1 � ����������
                2 � ������������
                3 � ��������
                4 � �������
                5 � ���������
                6 � �����������
                7 � ��������������
                1 � 2; 3, 4 � 5; 6 � 7 � ����������������� ��������
            */
            var mensesProtocol = protocols.FirstOrDefault(t => t.Type.Code.Equals("30"));
            if (mensesProtocol != null)
            {
                var mensFunction =
                    mensesProtocol._children.Where(t => t.Type.Code.Equals("30.5")).Select(t => t.Value).First();
                if (mensFunction.Equals("������������"))
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
                    rootChar.Add(new XElement("char", reg.Equals("����������") ? 1 : 2));
                    rootChar.Add(new XElement("char", county.Equals("��������") ? 3 : (county.Equals("�������") ? 4 : 5)));
                    rootChar.Add(new XElement("char", pain.Equals("�����������") ? 6 : 7));

                    root.Add(rootChar);

                    card.Add(root);
                }
            }

            card.Add(new XElement("healthGroupBefore", (int)this.HealthGroupObsBefore));
            if (HealthGroupForSportObsBefore!=0)
                card.Add(new XElement("fizkultGroupBefore", (int)this.HealthGroupForSportObsBefore));

            /*
            diagnosisBefore minOccurs="0" >
            �������� �� ���������� ������������
             * diagnosis >
                * mkb
                * dispNablud
                ������������ ����������
                1 � ����������� �����
                2 � ����������� �������
                3 � �� �����������
                * lechen minOccurs="0" >
                        ������� ���������
                    * condition
                    * organ
                    * notDone
                * reabil minOccurs="0" > 
                ����������� ������������/���������-��������� ������� ���������
                    * condition
                    * organ
                    * notDone minOccurs="0"
                ����������� ������������/���������-��������� ������� �� ��������� � ������������ � �����������
                * vmp
                ������������������� ����������� ������
                1 � ������������� � �������
                2 � ������������� � �� �������
                0 � �� �������������
             * 
             * diagNotDone >
                * reason
                ������� ������������
                1 � ���������� �� ������ ���������� ���������������
                2 � ����� �� ������������ �������������
                3 � ����� ����� ����������
                4 � ���������� �� � ������ ������
                5 � �������� ����������� ����������� ������
                10 � ������
                * reasonOther
                ���� ������� ������������
             */
            var diagnBefore = new XElement("diagnosisBefore");
            foreach (var mkbWithDispInfoBefore in diagnosesBefore)
            {
                var diagn = new XElement("diagnosis");
                diagn.Add(new XElement("mkb", mkbWithDispInfoBefore.MKB10.MKB));
                diagn.Add(new XElement("dispNablud", (int)mkbWithDispInfoBefore.DispObser + 1)); // �������� ��� ������������ c 0

                if (mkbWithDispInfoBefore.HealingEnabled)
                {
                    var lechen = new XElement("lechen");
                    lechen.Add(new XElement("condition", (int) mkbWithDispInfoBefore.Healing.HealCondition + 1));
                        // �������� ��� ������������ c 0
                    lechen.Add(new XElement("organ", (int) mkbWithDispInfoBefore.Healing.MedOrganization + 1));
                        // �������� ��� ������������ c 0
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
                        // �������� ��� ������������ c 0
                    rehab.Add(new XElement("organ", (int) mkbWithDispInfoBefore.Rehabilitation.MedOrganization + 1));
                        // �������� ��� ������������ c 0
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
            ��� �������, ���� ������ ������
            ��� ������ ���������� � ��������� Z00-Z10.
             */
            
            // !!
            
            /*
            diagnosisAfter >
             �������� ����� ������������
             * diagnosis
                * mkb
                * firstTime
                        ������� �������
                        1 � ��
                        0 � ���
                * dispNablud
                        ������������ ����������
                        1 � ����������� �����
                        2 � ����������� �������
                        0 � �� �����������
                * lechen minOccurs="0" >
                        ������� ��������� 
                    * condition
                    * organ
                * reabil minOccurs="0" >
                        ������������/���������-��������� ������� ���������
                    * condition
                    * organ
                * consul" minOccurs="0" >
                        �������������� ������������ � ������������ ���������
                    * condition
                    * organ
                    * state
                              �������������� ������������ � ������������ ���������
                              0 � �� ��������� � ������������ � �����������
                              1 � ��������� � ������ ������
                              2 � ��������� � �������� ������
                * needVMP
                        ������������� ���
                * needSMP
                        ������������� ���
                * needSKL
                        ������������� ���
                * recommendNext
                        ������������ �� ������������� ����������, �������, ����������� ������������ � ���������-���������� ������� � ��������� ���� ����������� ����������� � ������������� �����
             */
            var diagnAfter = new XElement("diagnosisAfter");
            foreach (var mkbWithDispInfoAfter in diagnosesAfter)
            {
                var diagn = new XElement("diagnosis");
                diagn.Add(new XElement("mkb", mkbWithDispInfoAfter.MKB10.MKB));
                diagn.Add(new XElement("firstTime", mkbWithDispInfoAfter.IsNew ? 0 : 1));
                var dispNablud = (int) mkbWithDispInfoAfter.DispObser + 1;
                diagn.Add(new XElement("dispNablud", dispNablud == 3 ? 0 : dispNablud)); // �������� ��� ������������ c 0

                if (mkbWithDispInfoAfter.HealingEnabled)
                {
                    var lechen = new XElement("lechen");
                    lechen.Add(new XElement("condition", (int) mkbWithDispInfoAfter.Healing.HealCondition + 1));
                        // �������� ��� ������������ c 0
                    lechen.Add(new XElement("organ", (int) mkbWithDispInfoAfter.Healing.MedOrganization + 1));
                        // �������� ��� ������������ c 0
                    diagn.Add(lechen);
                }
                if (mkbWithDispInfoAfter.RehabEnabled)
                {
                    var rehab = new XElement("reabil");
                    rehab.Add(new XElement("condition", (int) mkbWithDispInfoAfter.Rehabilitation.HealCondition + 1));
                        // �������� ��� ������������ c 0
                    rehab.Add(new XElement("organ", (int) mkbWithDispInfoAfter.Rehabilitation.MedOrganization + 1));
                        // �������� ��� ������������ c 0
                    diagn.Add(rehab);
                }
                if (mkbWithDispInfoAfter.AddConsultEnabled)
                {
                    var consul = new XElement("consul");
                    consul.Add(new XElement("condition", (int) mkbWithDispInfoAfter.AdditionalConsult.HealCondition + 1));
                        // �������� ��� ������������ c 0
                    consul.Add(new XElement("organ", (int) mkbWithDispInfoAfter.AdditionalConsult.MedOrganization + 1));
                        // �������� ��� ������������ c 0
                    consul.Add(new XElement("state", (int) mkbWithDispInfoAfter.AdditionalConsult.AdditionalConsultation));
                    diagn.Add(consul);
                }
                diagn.Add(new XElement("needVMP", mkbWithDispInfoAfter.RecommendsHighTech ? 1 : 0));
                diagn.Add(new XElement("needSMP", mkbWithDispInfoAfter.RecommendsAmbulance ? 1 : 0));
                diagn.Add(new XElement("needSKL", mkbWithDispInfoAfter.RecommendsResort ? 1 : 0));
                var recom = string.IsNullOrEmpty(mkbWithDispInfoAfter.Recommendation)
                    ? "���"
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
            ���������� �� ������������
             * type
                  ��� ������������
                  1 � � ��������
                  2 � ������������
             * dateFirstDetected
                  ���� ������� �������������������
             * dateLastConfirmed
                  ���� ���������� �������������������
             * illnesses
                  �����������, ������������ ������������� ������������
                  1 � ��������� ������������ � ������������, �� ���:
                  2 � ����������
                  3 � �������
                  4 � ���
                  5 � ���������������
                  6 � ������� �����, ������������ ������� � ��������� ���������, ����������� ������� ��������, � ��� �����:
                  9 � ����
                  10 � ������� ����������� �������, ������������ ������� � ��������� ������ �������, �� ���:
                  13 � �������� ������
                  14 � ����������� ������������ � ������������ ���������, � ��� �����:
                  15 � ���������� ����������
                  16 � ������� ������� �������, �� ���:
                  17 � ������������ ������� � ��. �������������� ��������
                  18 � ������� ����� � ��� ������������ ��������
                  19 � ������� ��� � ������������ ��������
                  20 � ������� ������� ��������������
                  21 � ������� ������� �������, �� ���:
                  22 � �����
                  23 � ������������� ������
                  24 � ������� ������� �����������
                  25 � ������� ���� � ��������� ���������
                  26 � ������� ������-�������� ������� � �������������� �����
                  27 � ������� ����������� �������
                  28 � ��������� ���������, ����������� � ������������� �������
                  29 � ���������� ��������, �� ���:
                  30 � �������� ������� �������
                  31 � �������� ������� ��������������
                  32 � �������� ������-������������� ��������
                  33 � ����������� �����, ���������� � ������ ����������� ������� ������
                
             * defects
                  ���� ��������� � ��������� ��������
                  1 � ����������
                  2 � ������ ���������������
                  3 � �������� � �������
                  4 � �������� � �������������
                  5 � ����������
                  6 � ������������ � �������������� ������������ �������
                  7 � ������������
                  8 � ���������
                  9 � ����� � ����������������
             */

            var disabilityProtocol = protocols.FirstOrDefault(t => t.Type.Code.Equals("25"));
            if (disabilityProtocol != null)
            {
                var hasDisablity =
                    disabilityProtocol._children.Where(t => t.Type.Code.Equals("25.1")).Select(t => t.Value).First();
                if (hasDisablity.ToLower().Equals("��"))
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

                    root.Add(new XElement("type", type.Equals("� ��������") ? 1 : 2));
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
            ���������� ������������
             * basic minOccurs="0" >
                  ������������ ������������
                * record >
                    * id
                              ������������� ������������� ������������
                              1 � ����� ������ �����
                              2 � ����� ������ ����
                              3 � ����� ������ ����
                              4 � ������������ ������ ������� � �����
                              5 � ������������ ������ �������� � �����
                              6 � ��� ������� ������� �������
                              7 � ��� ������
                              8 � ��� ���������� ������
                              9 � ��� ������� �������������� �����
                              10 � ��� ������������� ��������
                              11 � ���������������
                              12 � ������������
                              13 � �������������������
                              14 � ������������ �������� �� ���������� ����������, ��������������, ����������������� �������, ������������ � ������������
                              15 � ��������������� ��������
                              16 � ������ ���� �� ���� �������
                              17 � ������ ����� �������� ����������� ������� � ������������ �������������������
                              18 � ��� �����
                              19 � ��� ������
                    * date
                              ���� ������������
                    * result
                              ��������� ������������
             * other minOccurs="0" >
                  �������������� ������������
                * record >
                    * date
                              ���� ���������� ������������
                * name
                              �������� ������������
                * result
                              ��������� ������������
             */
            var issled = new XElement("issled");
            var osmotri = new XElement("osmotri");

            var issledCodes = new Dictionary<string, int>();
            issledCodes.Add("021731", 1); // 1 � ����� ������ �����
            issledCodes.Add("021733", 2); // 2 � ����� ������ ����
            issledCodes.Add("1", 3); // 3 � ����� ������ ����
            issledCodes.Add("063432", 4); // 4 � ������������ ������ ������� � �����
            issledCodes.Add("2", 5); // 5 � ������������ ������ �������� � �����
            issledCodes.Add("021769", 6); // 6 � ��� ������� ������� �������
            issledCodes.Add("021771", 7); // 7 � ��� ������
            issledCodes.Add("021773", 8); // 8 � ��� ���������� ������
            issledCodes.Add("021775", 9); // 9 � ��� ������� �������������� �����
            issledCodes.Add("3", 10); // 10 � ��� ������������� ��������
            issledCodes.Add("021781", 11); // 11 � ���������������
            issledCodes.Add("063434", 12); // 12 � ������������
            issledCodes.Add("021774", 13); // 13 � �������������������
            issledCodes.Add("4", 14); // 14 � ������������ �������� �� ���������� ����������, ��������������, ����������������� �������, ������������ � ������������
            issledCodes.Add("5", 15); // 15 � ��������������� ��������
            issledCodes.Add("6", 16); // 16 � ������ ���� �� ���� �������
            issledCodes.Add("7", 17); // 17 � ������ ����� �������� ����������� ������� � ������������ �������������������
            issledCodes.Add("8", 18); // 18 � ��� �����
            issledCodes.Add("9", 19); // 19 � ��� ������

            var osmotriCodes = new Dictionary<string, int>();
            osmotriCodes.Add("161014", 1); // 1 � �������
            osmotriCodes.Add("161001", 2); // 2 � ��������
            osmotriCodes.Add("161006", 3); // 3 � �����������
            osmotriCodes.Add("161031", 4); // 4 � ������� ������
            osmotriCodes.Add("161010", 5); // 5 � �����������������
            osmotriCodes.Add("161042", 6); // 6 � �����������-�������
            osmotriCodes.Add("161045", 7); // 7 � ��������
            osmotriCodes.Add("161046", 8); // 8 � ������� ����������
            osmotriCodes.Add("161036", 9); // 9 � ������� ������������
            osmotriCodes.Add("161047", 10); // 10 � ������� ������-��������
            osmotriCodes.Add("161026", 11); // 11 � ������-���������

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
            ������� ������
             * record >
                * id
                        ������������� �������
                        1 � �������
                        2 � ��������
                        3 � �����������
                        4 � ������� ������
                        5 � �����������������
                        6 � �����������-�������
                        7 � ��������
                        8 � ������� ����������
                        9 � ������� ������������
                        10 � ������� ������-��������
                        11 � ������-���������
                * date
                        ���� �������
             */
            card.Add(osmotri);

            //recommendZOZH

            card.Add(new XElement("recommendZOZH", pediatrService.CommonProtocol.Recommendation));

            /*
            reabilitation minOccurs="0" >
            ��������� ������������
             * date
                  ���� ����������
             * state
                  ������� ����������:
                  1 � ���������
                  2 � ��������
                  3 � ������
                  4 � �� ���������
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
            ���������� ����������
             * state
                  1 � ������ �� ��������
                  2 � �� ������ �� ����������� ����������: ���������
                  3 � �� ������ �� ����������� ����������: ��������
                  4 � �� ������ �� ������ ��������: ���������
                  5 � �� ������ �� ������ ��������: ��������
             * privs" minOccurs="0"
                  ��������� � ���������� ����������/������������
                * priv
                        6 � ��� - V
                        7 � ��� - R1
                        8 � ��� - R2
                        9 � ����������� - V1
                        10 � ����������� - V2
                        11 � ����������� - V3
                        12 � ����������� - R1
                        13 � ����������� - R2
                        14 � ����������� - R3
                        15 � ���� - V1
                        16 � ���� - V2
                        17 � ���� - V3
                        18 � ���� - ����
                        19 � ���� - ���
                        20 � ���� - V
                        21 � ���� - R
                        22 � ����.������� - V
                        23 � ����.������� - R
                        24 � �������� - V
                        25 � �������� - R
                        26 � ������� � - V1
                        27 � ������� � - V2
                        28 � ������� � - V3
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
                privivki.Add(new XElement("state", 1)); // ���� �� ������� �������!

            card.Add(privivki);

            /* oms
            ��������� ������ ���:
            0 � �� �������
            1 � ��������
            2 � �� ��������
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
            �����������, ������������ ������������� ������������
            1 � ��������� ������������ � ������������, �� ���:
            2 � ����������
            3 � �������
            4 � ���
            5 � ���������������
            6 � ������� �����, ������������ ������� � ��������� ���������, ����������� ������� ��������, � ��� �����:
            9 � ����
            10 � ������� ����������� �������, ������������ ������� � ��������� ������ �������, �� ���:
            13 � �������� ������
            14 � ����������� ������������ � ������������ ���������, � ��� �����:
            15 � ���������� ����������
            16 � ������� ������� �������, �� ���:
            17 � ������������ ������� � ��. �������������� ��������
            18 � ������� ����� � ��� ������������ ��������
            19 � ������� ��� � ������������ ��������
            20 � ������� ������� ��������������
            21 � ������� ������� �������, �� ���:
            22 � �����
            23 � ������������� ������
            24 � ������� ������� �����������
            25 � ������� ���� � ��������� ���������
            26 � ������� ������-�������� ������� � �������������� �����
            27 � ������� ����������� �������
            28 � ��������� ���������, ����������� � ������������� �������
            29 � ���������� ��������, �� ���:
            30 � �������� ������� �������
            31 � �������� ������� ��������������
            32 � �������� ������-������������� ��������
            33 � ����������� �����, ���������� � ������ ����������� ������� ������
             */
            var normalValue = value.ToLower();
            if (normalValue.Equals("��������� ������������ � ������������, �� ���:")) return 1;
            if (normalValue.Equals("����������")) return 2;
            if (normalValue.Equals("�������")) return 3;
            if (normalValue.Equals("���")) return 4;
            if (normalValue.Equals("���������������")) return 5;
            if (normalValue.Equals("������� �����, ������������ ������� � ��������� ���������, ����������� ������� ��������, � ��� �����:")) return 6;
            if (normalValue.Equals("����")) return 9;
            if (normalValue.Equals("������� ����������� �������, ������������ ������� � ��������� ������ �������, �� ���:")) return 10;
            if (normalValue.Equals("�������� ������")) return 13;
            if (normalValue.Equals("����������� ������������ � ������������ ���������, � ��� �����:")) return 14;
            if (normalValue.Equals("���������� ����������")) return 15;
            if (normalValue.Equals("������� ������� �������, �� ���:")) return 16;
            if (normalValue.Equals("������������ ������� � ��. �������������� ��������")) return 17;
            if (normalValue.Equals("������� ����� � ��� ������������ ��������")) return 18;
            if (normalValue.Equals("������� ��� � ������������ ��������")) return 19;
            if (normalValue.Equals("������� ������� ��������������")) return 20;
            if (normalValue.Equals("������� ������� �������, �� ���:")) return 21;
            if (normalValue.Equals("�����")) return 22;
            if (normalValue.Equals("������������� ������")) return 23;
            if (normalValue.Equals("������� ������� �����������")) return 24;
            if (normalValue.Equals("������� ���� � ��������� ���������")) return 25;
            if (normalValue.Equals("������� ������-�������� ������� � �������������� �����")) return 26;
            if (normalValue.Equals("������� ����������� �������")) return 27;
            if (normalValue.Equals("��������� ���������, ����������� � ������������� �������")) return 28;
            if (normalValue.Equals("���������� ��������, �� ���:")) return 29;
            if (normalValue.Equals("�������� ������� �������")) return 30;
            if (normalValue.Equals("�������� ������� ��������������")) return 31;
            if (normalValue.Equals("�������� ������-������������� ��������")) return 32;
            if (normalValue.Equals("����������� �����, ���������� � ������ ����������� ������� ������")) return 33;
            return - 1;
        }

        private int getDefectsCode(string value)
        {
            /* defects
            ���� ��������� � ��������� ��������
            1 � ����������
            2 � ������ ���������������
            3 � �������� � �������
            4 � �������� � �������������
            5 � ����������
            6 � ������������ � �������������� ������������ �������
            7 � ������������
            8 � ���������
            9 � ����� � ����������������
             */
            var normalValue = value.ToLower();
            if (normalValue.Equals("����������")) return 1;
            if (normalValue.Equals("������ ���������������")) return 2;
            if (normalValue.Equals("�������� � �������")) return 3;
            if (normalValue.Equals("�������� � �������������")) return 4;
            if (normalValue.Equals("����������")) return 5;
            if (normalValue.Equals("������������ � �������������� ������������ �������")) return 6;
            if (normalValue.Equals("������������")) return 7;
            if (normalValue.Equals("���������")) return 8;
            if (normalValue.Equals("����� � ����������������")) return 9;
            return -1;
        }

        private int getPrivCode(string value)
        {
            var normalValue = value.ToLower();

            /*
            6 � ��� - v
            7 � ��� - r1
            8 � ��� - r2
            */

            if (normalValue.Contains("���"))
            {
                if (normalValue.Contains("v"))
                    return 6;
                if (normalValue.Contains("r1"))
                    return 7;
                if (normalValue.Contains("r2"))
                    return 8;
            }
            /*
            9 � ����������� - v1
            10 � ����������� - v2
            11 � ����������� - v3
            12 � ����������� - r1
            13 � ����������� - r2
            14 � ����������� - r3
             */
            if (normalValue.Contains("�����������"))
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
            15 � ���� - v1
            16 � ���� - v2
            17 � ���� - v3
            18 � ���� - ����
            19 � ���� - ���
            20 � ���� - v
            21 � ���� - r
             */
            if (normalValue.Contains("����"))
            {
                if (normalValue.Contains("v1"))
                    return 15;
                if (normalValue.Contains("v2"))
                    return 16;
                if (normalValue.Contains("v3"))
                    return 17;

                if (normalValue.Contains("����"))
                    return 18;
                if (normalValue.Contains("���"))
                    return 19;
            }

            /*
            22 � ����.������� - v
            23 � ����.������� - r
            24 � �������� - v
            25 � �������� - r
            26 � ������� � - v1
            27 � ������� � - v2
            28 � ������� � - v3 
             */

            if (normalValue.Contains("�������"))
            {
                if (normalValue.Contains("v"))
                    return 22;
                return 23;
            }

            if (normalValue.Contains("��������"))
            {
                if (normalValue.Contains("v"))
                    return 24;
                return 25;
            }

            if (normalValue.Contains("�������"))
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
            ���������� ������������� ����� ������ ��������������� �������
            */
            childBlock.Add(new XElement("idInternal", pacient.Oid.ToString()));

            /*
            idType
            ��� ����� ������: 1 � ������, 3 � �����������������
            */
            childBlock.Add(new XElement("idType", 3));
            /*
            name >
            �������, ���, ��������
            ������ ���� ����� ���� �������� ��� ��������� ��������.
             * last
                  �������
             * first
                  ���
             * middle
                  ��������
             */
            var name = new XElement("name");
            name.Add(new XElement("last", pacient.LastName));
            name.Add(new XElement("first", pacient.FirstName));
            if (!string.IsNullOrEmpty(pacient.MiddleName))
                name.Add(new XElement("middle", pacient.MiddleName));

            childBlock.Add(name);
            /*
            idSex
            ��� ������: 1 � �������, 2 � �������
            ������ ���� ����� ���� �������� ��� ��������� ��������.
             */
            childBlock.Add(new XElement("idSex", pacient.Gender == Gender.Male ? 1 : 2));
            /*
            dateOfBirth
            ���� ��������
            */
            childBlock.Add(new XElement("dateOfBirth", pacient.Birthdate.Value.Date.ToString("yyyy-MM-ddZ")));

            /*
            idCategory
            ��������� ������:
            1 � ������-������
            2 � ������, ����������� � ������� ��������� ��������
            3 � ������, ���������� ��� ��������� ���������
            4 � ��� ���������
            ������ ���� ����� ���� �������� ��� ��������� ��������.
             */
            childBlock.Add(new XElement("idCategory", 4));
            /*
            idDocument
            ��������, �������������� ��������:
            3 � ������������� � ��������
            14 � ������� ���������� ��
            documentSer
            ����� ���������, �������������� ��������
            documentNum
            ����� ���������, �������������� ��������
             */
            if (pacient.Document.Type.Code == 3 || pacient.Document.Type.Code == 14)
            {
                childBlock.Add(new XElement("idDocument", pacient.Document.Type.Code));
                childBlock.Add(new XElement("documentSer", pacient.Document.Serial));
                childBlock.Add(new XElement("documentNum", pacient.Document.Number));
            }
            /*
            snils minOccurs="0"
            ����� �����
            [0-9]{3}-[0-9]{3}-[0-9]{3}-[0-9]{2}
            */
            if (pacient.SNILS!=null)
                childBlock.Add(new XElement("snils", pacient.SNILS));

            /*
            idPolisType minOccurs="0"
            ������ ���������� ������:
            1 � ����� ������� �������
            2 � ����� ������ �������
            ���� ���� �������, ����������� �������� 2.
            polisSer minOccurs="0"
            ����� ���������� ������
            polisNum
            ����� ���������� ������
            idInsuranceCompany
            ���������� ������������� ��������� ��������
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
            ������������ ����������� �����������, ��������� ��� ��������� ��������� ������-���������� ������
            medSanAddress
            ����������� ����� ����������� �����������, ��������� ��� ��������� ��������� ������-���������� ������
             */

            var org = Session.FindObject<MedOrg>(CriteriaOperator.Parse("Code=?", Settings.MOSettings.GetCurrentMOCode(Session)));
            childBlock.Add(new XElement("medSanName", org.FullName));
            childBlock.Add(new XElement("medSanAddress", org.AddressJur));

            /*
            address >
            ����� ����� ����������� ����������
             * index" minOccurs="0"
                  �������� ������
                  [1-9][0-9]{5}
             * kladrNP
                  ��� ���������� ������ ���������� �� �����
                  [0-9]{13}
             * kladrStreet" minOccurs="0"
                  ��� ����� ���������� �� �����
                  [0-9]{17}
             * house minOccurs="0"
                  ����� ����
             * building minOccurs="0"
                  ����� ��������
             * appartment minOccurs="0"
                  ����� ��������
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
            ������ ���������������� ����������
            ������ ���� ����� ��������� ��� ����� idEducationOrg.
            ������ ���� ����� ���� �������� ��� ��������� ��������.
             * kladrDistr minOccurs="0"
                  ��� ������ �� �����, � ������� ��������� ��������������� ����������
                  <xs:pattern value="[0-9]{13}"
             * idEducType" minOccurs="0"
                  ��� ���������������� ����������:
                  1 � ���������� ��������������� ����������
                  2 � ������������������� (���������� ������, ��������� ������, �������� (�������) ������ �����������) ��������������� ����������
                  3 � ��������������� ���������� ���������� �����������������, �������� �����������������, ������� ����������������� �����������
                  4 � ����������� (�������������) ��������������� ���������� ��� �����������, ������������� � ������������� ������������� ��������
                  5 � ��������������� ���������� ��� �����-����� � �����, ���������� ��� ��������� ��������� (�������� ��������������)
             * educOrgName"
                  ������������ ���������������� ����������
            idEducationOrg minOccurs="0"
            ���������� ������������� ���������������� ����������

            idOrphHabitation minOccurs="0"
            0 � ������������ ����������
            1 � �����
            2 � ��������������
            3 � ����������� (����������)
            4 � ������� � ������� �����
            8 � ������� � ����������� �����

            ���� ����������� ����������� ��� ������-������.
            dateOrphHabitation minOccurs="0"
            ���� ����������� � ����� �������� ����������
            ���� ����������� ����������� ��� ������-������.
            idStacOrg minOccurs="0"
            ���������� ������������� ������������� ����������
            ���� ����������� ����������� ��� �������� idOrphHabitation ������ 0.
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
