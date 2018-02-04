using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// Шаблон абстрактной услуги
    /// </summary>
    public class ServiceTemplate : BaseObject
    {
        private string name;
        private string anamnez;
        private string complain;
        private string recommendations;
        private string objectiveStatus;
        private MKB10 diagnose;
        private TerritorialUsluga service;
        private DoctorSpecTree doctorSpec;

        public ServiceTemplate() { }
        public ServiceTemplate(Session session) : base(session) { }

        /// <inheritdoc/>
        public override void AfterConstruction()
        {
            base.AfterConstruction();

            Doctor currentDoctor = (Doctor)SecuritySystem.CurrentUser;
            if (currentDoctor.SpecialityTree != null)
            {
                this.DoctorSpec = Session.FindObject<DoctorSpecTree>(
                    DoctorSpecTree.Fields.Oid == currentDoctor.SpecialityTree.Oid);
            }
        }

        /// <summary>Название</summary>
        [RuleRequiredField("Registrator.Module.BusinessObjects.Dictionaries.ServiceTemplate.NameRequired", DefaultContexts.Save)]
        public string Name
        {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }

        /// <summary>Анамнез</summary>
        [Size(1000)]
        public string Anamnez
        {
            get { return anamnez; }
            set { SetPropertyValue("Anamnez", ref anamnez, value); }
        }

        /// <summary>Жалобы</summary>
        [Size(1000)]
        public string Complain
        {
            get { return complain; }
            set { SetPropertyValue("Complain", ref complain, value); }
        }

        /// <summary>Рекомендации</summary>
        [Size(1000)]
        public string Recommendations
        {
            get { return recommendations; }
            set { SetPropertyValue("Recommendations", ref recommendations, value); }
        }

        /// <summary>Объективный статус</summary>
        [Size(1000)]
        public string ObjectiveStatus
        {
            get { return objectiveStatus; }
            set { SetPropertyValue("ObjectiveStatus", ref objectiveStatus, value); }
        }

        /// <summary>Диагноз</summary>
        public MKB10 Diagnose
        {
            get { return diagnose; }
            set { SetPropertyValue("Diagnose", ref diagnose, value); }
        }

        /// <summary>Услуга</summary>
        public TerritorialUsluga Service
        {
            get { return service; }
            set { SetPropertyValue("Service", ref service, value); }
        }

        /// <summary>Специальность</summary>
        [RuleRequiredField("Registrator.Module.BusinessObjects.Dictionaries.ServiceTemplate.DoctorSpecRequired", DefaultContexts.Save)]
        public DoctorSpecTree DoctorSpec
        {
            get { return doctorSpec; }
            set { SetPropertyValue("DoctorSpec", ref doctorSpec, value); }
        }

        /// <summary>Операнды свойств класса</summary>
        public static new readonly FieldsClass Fields = new FieldsClass();
        /// <summary>Операнды свойств класса</summary>
        public new class FieldsClass : BaseObject.FieldsClass
        {
            /// <summary>Конструктор</summary>
            public FieldsClass() { }
            /// <summary>Конструктор</summary>
            /// <param name="propertyName">Название вложенного свойства</param>
            public FieldsClass(string propertyName) : base(propertyName) { }

            public OperandProperty Oid { get { return new OperandProperty(GetNestedName("Oid")); } }

            public DoctorSpecTree.FieldsClass DoctorSpec { get { return new DoctorSpecTree.FieldsClass("DoctorSpec"); } }

        }
    }
}
