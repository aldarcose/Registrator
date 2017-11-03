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
    /// ���������
    /// </summary>
    [DefaultClassOptions]
    [XafDisplayName("���������")]
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
        [Browsable(false)]
        public CelPosescheniya Cel { get; set; }

        /// <summary>
        /// ����� ������������
        /// </summary>
        [XafDisplayName("����� ������������")]
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
            int index = Pacient.Cases.IndexOf(this);
            return GetReestrElement(index);
        }

        public override System.Xml.Linq.XElement GetReestrElement(int zapNumber)
        {
            // ��������� ���� ������
            if (IsValidForReestr() == false)
                return null;

            const string dateTimeFormat = "{0:yyyy-MM-dd}";

            XElement element = new XElement("SLUCH");

            // ����� ������ � ������� �������
            element.Add(new XElement("IDCASE", zapNumber));

            // ������� �������� ���. ������
            element.Add(new XElement("USL_OK", this.UsloviyaPomoshi.Code));

            // ��� ���. ������
            element.Add(new XElement("VIDPOM", this.VidPom.Code));

            // ����� ���. ������
            element.Add(new XElement("FOR_POM", this.FormaPomoshi.Code));

            if (this.FromLPU!=null)
                // ����������� ��
                element.Add(new XElement("NRP_MO", this.FromLPU.Code));
            
            // ����������� (��������������)
            //element.Add(new XElement("EXTR", ));

            // ��� ��
            element.Add(new XElement("LPU", this.LPU.Code));

            if (!string.IsNullOrEmpty(this.LPU_1))
                // ��� ������������� ��
                element.Add(new XElement("LPU_1", this.LPU_1));

            if (this.Otdelenie != null)
                // ��� ���������
                element.Add(new XElement("PODR", this.Otdelenie.Code));

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

            // 
            element.Add(new XElement("DS1", this.MainDiagnose.Diagnose.CODE));

            // ������������� ��������
            foreach(var ds2 in this.SoputsDiagnoses)
                element.Add(new XElement("DS2", ds2.CODE));

            // �������� ����������
            foreach(var ds3 in this.OslozhDiagnoses)
                element.Add(new XElement("DS3", ds3.CODE));

            // ��������� ����� ��������
            if (this.VesPriRozhdenii != 0)
                // ��� ��� ��������
                element.Add(new XElement("VNOV_M", this.VesPriRozhdenii));

            /*// ���� ���
            element.Add(new XElement("CODE_MES1", ));

            // ���� ��� ������������� �����������
            element.Add(new XElement("CODE_MES2", ));*/

            // ��������� ��������� 
            element.Add(new XElement("RSLT", this.Resultat.Code));

            // ����� �����������
            element.Add(new XElement("ISHOD", this.Ishod.Code));

            // ������������� ���. �����
            element.Add(new XElement("PRVS", this.DoctorSpec.Code));

            // ��� �������������� ���. ����-�
            element.Add(new XElement("VERS_SPEC", this.VersionSpecClassifier));

            // ��� �����, ���������� ������
            element.Add(new XElement("IDDOCT", this.Doctor.InnerCode));

            /*// ������ ������
            element.Add(new XElement("OS_SLUCH", (int)this.OsobiySluchay));*/

            // ������ ������ ���. ������
            element.Add(new XElement("IDSP", this.SposobOplMedPom));

            /*// ���-�� ������ ������ ���. ������
            element.Add(new XElement("ED_COL", this.MedPomCount));*/

            if (this.Tarif!=0)
                // �����
                element.Add(new XElement("TARIF", this.Tarif));

            // �����
            element.Add(new XElement("SUMV", this.TotalSum));

            // ��� ������
            element.Add(new XElement("OPLATA", (int)this.StatusOplati));

            // ������ �� �������
            int serviceCounter = 1;
            foreach (var usl in Services)
                element.Add(new XElement("USL", usl.GetReestrElement(serviceCounter++)));

            if (!string.IsNullOrEmpty(this.Comment))
                // ��������� ����
                element.Add(new XElement("COMMENTSL", this.Comment));

            return element;
        }

        /// <summary>
        /// �������� ���������
        /// </summary>
        public override CriteriaOperator DiagnoseCriteria
        {
            get
            {
                // ������� ��� ��������
                return CriteriaOperator.Parse("1=1");
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
