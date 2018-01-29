using DevExpress.Persistent.BaseImpl;
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

        public ServiceTemplate() { }
        public ServiceTemplate(Session session) : base(session) { }

        /// <summary>Название</summary>
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
    }
}
