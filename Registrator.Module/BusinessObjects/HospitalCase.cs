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
    /// ���������
    /// </summary>
    /// <ToDo>�������� �������� ������� (MainDiagnose) � HospitalCase</ToDo>
    [DefaultClassOptions]
    [XafDisplayName("��������������")]
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
            // ���������� �������� ������������
            var createdBy = SecuritySystem.CurrentUser as Doctor;
            if (createdBy != null)
            {
                // ������� ������� � ����� �� �������
                Doctor = Session.FindObject<Doctor>(CriteriaOperator.Parse("UserName=?", createdBy.UserName));
            }

            string MOCode = Settings.MOSettings.GetCurrentMOCode(Session);
            this.LPU = Session.FindObject<MedOrg>(CriteriaOperator.Parse("Code=?", MOCode));
            this.LPU_1 = MOCode;

            // ���� ���������� ���� ������������� �������
            this.NHistory = this.Oid.ToString();

            /*
             * Code = 29 - "�� ��������� � �����������"
             * Code = 30 - "�� ��������� (����������� ������) � �����������"
             */
            /*var sposobCode = (this.cel == CelPosescheniya.ProfOsmotr) ? 29 : 30;
            this.SposobOplMedPom = Session.FindObject<Dictionaries.SposobOplatiMedPom>(CriteriaOperator.Parse("Code=?", sposobCode));*/

            /*
             * Code = 1 - "����������"
             * Code = 2 - "� ������� ����������"
             * Code = 3 - "�����������" 
             * Code = 4 - "��� ����������� �����������"
             */
            this.UsloviyaPomoshi = Session.FindObject<Dictionaries.VidUsloviyOkazMedPomoshi>(CriteriaOperator.Parse("Code=?", 1));

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

            this.Hospitalizacia = Napravlenie.Planovaya;

            // ���������, ��� ������ �� ��������� �������, ���� ��� - �������
            if (Services.Count == 0)
            {
                Services.Add(new MedService(Session) { IsMainService = true, AutoOpen = false });
            }

            // ��������� ������ � ��������������� ���������, ����� ����� ����������� ��������� ���� ������ �� View ������
            Service = Services[0];
        }

        [XafDisplayName("�����������/��������������")]
        public Napravlenie Hospitalizacia { get; set; }

        /// <summary>
        /// ������� �� ����
        /// </summary>
        [XafDisplayName("�� ����")]
        public bool NaDomy { get; set; }

        /// <summary>
        /// ��� ��������������
        /// </summary>
        [XafDisplayName("��� ����������")]
        [RuleRequiredField(DefaultContexts.Save)]
        /*
         * � ����������� �� ���������� ����, � ����� ����������� ����. ��������:
         * �������, �� <CODE_USL> - 098305 ��� ��������, 
         * ��������� - 198305 ��� �����
         * ����������, �� <CODE_USL> - 098304 ��� ��������, 198304 ��� �����
         * ���������������, �� <CODE_USL> - 098308 ��� ��������, 198308 ��� �����
        */
        public TipStacionara Type { get; set; }

        public override CriteriaOperator DiagnoseCriteria
        {
            get
            {
                // �������� ��� �������� ������� ���
                // ���������� ����. �������� ��� ��������� ���� ���������
                var KSGs = new XPCollection<ClinicStatGroups>(Session).ToList();
                return new InOperator("MKB", KSGs.Select(t => t.Diagnose.MKB));
            }
        }

        /// <summary>
        /// � �������������� ���� ������, ������� ��������������� ��� �������� ������
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
            // ��������� ���� ������
            if (IsValidForReestr() == false)
                return null;

            const string dateTimeFormat = "{0:yyyy-MM-dd}";
            //var ksg = Session.FindObject<ClinicStatGroups>(CriteriaOperator.Parse("MainDiagnose.MKB=?", this.MainDiagnose.Diagnose.MKB));
            Decimal tarif = Settings.TarifSettings.GetDnevnoyStacionarTarif(Session);

            XElement element = new XElement("SLUCH");

            // ����� ������ � ������� �������
            element.Add(new XElement("IDCASE", zapNumber));

            // ������� �������� ���. ������
            element.Add(new XElement("USL_OK", this.UsloviyaPomoshi.Code));

            // ��� ���. ������
            element.Add(new XElement("VIDPOM", this.VidPom.Code));

            // ����� ���. ������
            element.Add(new XElement("FOR_POM", this.FormaPomoshi.Code));

            if (this.FromLPU != null)
                // ����������� ��
                element.Add(new XElement("NPR_MO", this.FromLPU.Code));

            // ����������� (��������������)
            element.Add(new XElement("EXTR", (int)this.Hospitalizacia));

            // ��� ��
            element.Add(new XElement("LPU", this.LPU.Code));

            if (!string.IsNullOrEmpty(this.LPU_1))
                // ��� ������������� ��
                element.Add(new XElement("LPU_1", this.LPU_1));

            string podr = lpuCode + (Profil != null ? (int?)Profil.Code : null) +
                            (Otdelenie != null ? Otdelenie.Code : null);

            // ��� ���������
            element.Add(new XElement("PODR", podr));

            // �������
            element.Add(new XElement("PROFIL", this.Profil.Code));

            // ������� �������
            element.Add(new XElement("DET", (int)this.DetProfil));

            // ����� ������� �������/������ ������������� ��������
            element.Add(new XElement("NHISTORY", this.Oid));

            // ���� �������
            element.Add(new XElement("DATE_1", string.Format(dateTimeFormat, this.DateIn)));
            element.Add(new XElement("DATE_2", string.Format(dateTimeFormat, this.DateOut)));

            if (this.PreDiagnose != null)
                // ��������� �������
                element.Add(new XElement("DS0", this.PreDiagnose.Diagnose.CODE));

            // todo: �������� �������� ������� (MainDiagnose) � HospitalCase !!!!
          //  element.Add(new XElement("DS1", this.MainDiagnose.Diagnose.CODE));

            // ������������� ��������
            foreach (var ds2 in this.SoputsDiagnoses)
                element.Add(new XElement("DS2", ds2.CODE));

            // �������� ����������
            foreach (var ds3 in this.OslozhDiagnoses)
                element.Add(new XElement("DS3", ds3.CODE));

            // ��������� ����� ��������
            if (this.VesPriRozhdenii != 0)
                // ��� ��� ��������
                element.Add(new XElement("VNOV_M", this.VesPriRozhdenii));

            // ���� ��� !!!
            //element.Add( new XElement("CODE_MES1", ksg.Number));

            // ���� ��� ������������� �����������
            //element.Add(new XElement("CODE_MES2", ));

            // ��������� ��������� 
            element.Add(new XElement("RSLT", this.Resultat.Code));

            // ����� �����������
            element.Add(new XElement("ISHOD", this.Ishod.Code));

            // ������������� ���. �����
            element.Add(new XElement("PRVS", this.DoctorSpec.Code));

            // ��� �������������� ���. ����-�
            element.Add(new XElement("VERS_SPEC", this.VersionSpecClassifier));

            // ��� �����, ���������� ������
            element.Add(new XElement("IDDOKT", this.Doctor.SNILS));

            /*// ������ ������
            element.Add(new XElement("OS_SLUCH", (int)this.OsobiySluchay));*/

            // ������ ������ ���. ������
            element.Add(new XElement("IDSP", this.SposobOplMedPom));

            // ���-�� ������ ������ ���. ������
            element.Add(new XElement("ED_COL", this.MedPomCount));

            //!!!!
            //this.Tarif = tarif * Convert.ToDecimal(ksg.KoeffZatrat);
            // �����
            element.Add(new XElement("TARIF", this.Tarif));

            // �����
            element.Add(new XElement("SUMV", this.TotalSum));

            // ��� ������
            element.Add(new XElement("OPLATA", (int)this.StatusOplati));

            // ������ �� �������
            int serviceCounter = 1;
            foreach (var usl in Services.OfType<MedService>())
                element.Add(new XElement("USL", usl.GetReestrElement(serviceCounter++, lpuCode)));

            if (!string.IsNullOrEmpty(this.Comment))
                // ��������� ����
                element.Add(new XElement("COMMENTSL", this.Comment));

            return element;
        }
    }
}
