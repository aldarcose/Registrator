using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    [XafDisplayName("Услуга с параметрами")]
    public class ServiceWithInfo : BaseObject
    {
        public ServiceWithInfo(Session session) : base(session) { }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            IsExportable = true;
            IsMain = false;
            IsDefault = true;
        }

        /// <summary>
        /// Услуга
        /// </summary>
        [XafDisplayName("Оказываемая услуга")]
        [RuleRequiredField]
        public TerritorialUsluga Service { get; set; }

        [NonPersistent]
        [XafDisplayName("Код услуги")]
        public string ServiceCode
        {
            get { return Service.Code; }
        }

        /// <summary>
        /// Признак того, что экспортируется в реестр
        /// </summary>
        [XafDisplayName("Услуга экспортируется в реестр")]
        public bool IsExportable { get; set; }

        /// <summary>
        /// Основная услуга
        /// </summary>
        [XafDisplayName("Основная услуга")]
        public bool IsMain { get; set; }

        /// <summary>
        /// Обязательная услуга
        /// </summary>
        [XafDisplayName("Обязательная услуга")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Услуга может являться обязательной для некоторых видов диспансеризации
        /// </summary>
        [Association("DispServiceList-ServiceWithInfo")]
        public DispsServiceList DispServiceList { get; set; }
    }
}
