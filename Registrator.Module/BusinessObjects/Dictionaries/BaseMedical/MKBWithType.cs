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
    /// Диагноз с указанием его типа
    /// </summary>
    [XafDisplayName("Данные диагноза")]
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
                // находим доктора с таким же Логином
                var doctor = Session.FindObject<Doctor>(CriteriaOperator.Parse("UserName=?", createdBy.UserName));
                if (doctor != null)
                    this.Doctor = doctor;
            }
        }

        /// <summary>
        /// Диагноз
        /// </summary>
        [XafDisplayName("Диагноз")]
        [ImmediatePostData]
        public MKB10 Diagnose { get; set; }

        /// <summary>
        /// Тип диагноза
        /// </summary>
        [XafDisplayName("Тип")]
        [Appearance("TypeVisibility", Context = "DetailView", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(Diagnose)")]
        public TipDiagnoza Type { get; set; }

        /// <summary>
        /// Тип диагноза
        /// </summary>
        [XafDisplayName("Характер")]
        [Appearance("CharacterVisibility", Context = "DetailView", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(Diagnose)")]
        public KharakterDiagnoza Character { get; set; }

        /// <summary>
        /// Тип диагноза
        /// </summary>
        [XafDisplayName("Стадия")]
        [Appearance("StadiaVisibility", Context = "DetailView", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(Diagnose)")]
        public StadiaDiagnoza Stadia { get; set; }

        /// <summary>
        /// Выявлен впервые
        /// </summary>
        [XafDisplayName("Выявлен впервые")]
        [Appearance("TimeVisibility", Context = "DetailView", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(Diagnose)")]
        public bool FirstTime { get; set; }

        public override string ToString()
        {
            if (Diagnose == null)
            {
                return "Нет диагноза";
            }
            return this.Diagnose.ToString();
        }

        [Association("CommonService-Diagnoses")]
        public CommonService Service { get; set; }

        /// <summary>
        /// Врач, поставивший диагноз
        /// </summary>
        [XafDisplayName("Врач, поставивший диагноз")]
        [Appearance("DoctorVisibility", Context = "DetailView", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(Diagnose)")]
        public Doctor Doctor { get; set; }
    }
}
