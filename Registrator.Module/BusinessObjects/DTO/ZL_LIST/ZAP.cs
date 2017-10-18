using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.Module.BusinessObjects.DTO
{

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [DefaultClassOptions]
    public partial class ZAP : BaseObject
    {
        public ZAP(Session session)
            : base(session)
        {
        }

        [Association("ZAPLIST_ZAP")]
        public ZL_LIST list { get; set; }

        private string n_ZAPField;

        private string pR_NOVField;
        
        /// <summary>
        /// Номер позиции записи
        /// Уникально иденти-фицирует запись в пределах счета.
        /// </summary>
        [Size(8)]
        public string N_ZAP
        {
            get { return this.n_ZAPField; }
            set { SetPropertyValue("N_ZAP", ref this.n_ZAPField, value); }
        }

        /// <summary>
        /// Признак исправленной записи
        /// 0 – сведения об оказанной медицинской помощи передаются впервые;
        /// 1 – запись передается повторно после исправления.
        /// </summary>
        [Size(1)]
        public string PR_NOV
        {
            get
            {
                return this.pR_NOVField;
            }
            set
            {
                this.pR_NOVField = value;
            }
        }

        private PACIENT pacient;
        /// <summary>
        /// Сведения о пациенте
        /// </summary>
        public PACIENT PACIENT
        {
            get { return pacient; }
            set { SetPropertyValue("PACIENT", ref pacient, value); }
        }
                
        /// <summary>
        /// Сведения о случае
        /// </summary>
        [Association("ZAP_SLUCH"), Aggregated]
        public XPCollection<SLUCH> SLUCH
        {
            get
            {
                return GetCollection<SLUCH>("SLUCH");
            }
        }
    }

}
