using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    [DefaultClassOptions]
    public class Document : BaseObject
    {
        public Document(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            var defaultCode = 14; // паспорт РФ
            this.Type = Session.FindObject<VidDocumenta>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", defaultCode));
            Serial = string.Empty;
            Number = string.Empty;
        }

        [XafDisplayName("Тип")]
        public VidDocumenta Type { get; set; }

        /// <summary>
        /// Серия документа
        /// </summary>
        [Size(20)]
        [XafDisplayName("Серия")]
        public string Serial { get; set; }

        /// <summary>
        /// Номер документа
        /// </summary>
        [Size(50)]
        [XafDisplayName("Номер")]
        public string Number { get; set; }

        /// <summary>
        /// Дата начала действия документа
        /// </summary>
        [XafDisplayName("Выдан")]
        public DateTime? DateBegin { get; set; }

        /// <summary>
        /// Дата окончания действия документа
        /// </summary>
        [XafDisplayName("Годен до")]
        public DateTime? DateEnd { get; set; }
        
        public override string ToString()
        {
            return String.Format("{0} серия {1} №{2} от {3}", Type, Serial, Number, DateBegin);
        }
    }
}
