using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Registrator.Module.BusinessObjects.Abstract;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// ������� � ��������� ��� ����
    /// </summary>
    [XafDisplayName("������ ��������")]
    public class MKBWithType : BaseObject
    {
        public MKBWithType(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            var createdBy = SecuritySystem.CurrentUser as Doctor;
            if (createdBy != null)
            {
                // ������� ������� � ����� �� �������
                var doctor = Session.FindObject<Doctor>(CriteriaOperator.Parse("UserName=?", createdBy.UserName));
                if (doctor != null)
                    this.Doctor = doctor;
            }
        }

        /// <summary>
        /// �������
        /// </summary>
        [XafDisplayName("�������")]
        [ImmediatePostData]
        public MKB10 Diagnose { get; set; }

        /// <summary>
        /// ��� ��������
        /// </summary>
        [XafDisplayName("���")]
        [Appearance("TypeVisibility", Context = "DetailView", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(Diagnose)")]
        public TipDiagnoza Type { get; set; }

        /// <summary>
        /// ��� ��������
        /// </summary>
        [XafDisplayName("��������")]
        [Appearance("CharacterVisibility", Context = "DetailView", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(Diagnose)")]
        public KharakterDiagnoza Character { get; set; }

        /// <summary>
        /// ��� ��������
        /// </summary>
        [XafDisplayName("������")]
        [Appearance("StadiaVisibility", Context = "DetailView", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(Diagnose)")]
        public StadiaDiagnoza Stadia { get; set; }

        /// <summary>
        /// ������� �������
        /// </summary>
        [XafDisplayName("������� �������")]
        [Appearance("TimeVisibility", Context = "DetailView", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(Diagnose)")]
        public bool FirstTime { get; set; }

        public override string ToString()
        {
            if (Diagnose == null)
            {
                return "��� ��������";
            }
            return this.Diagnose.ToString();
        }

        [Association("CommonService-Diagnoses")]
        public CommonService Service { get; set; }

        /// <summary>
        /// ����, ����������� �������
        /// </summary>
        [XafDisplayName("����, ����������� �������")]
        [Appearance("DoctorVisibility", Context = "DetailView", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(Diagnose)")]
        public Doctor Doctor { get; set; }
    }
}
