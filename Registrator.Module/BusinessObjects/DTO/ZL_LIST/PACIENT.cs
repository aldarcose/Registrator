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
    public partial class PACIENT : BaseObject
    {
        public PACIENT(Session session)
            : base(session)
        {
        }

        private string iD_PACField;

        private string vPOLISField;

        private string sPOLISField;

        private string nPOLISField;

        private string sMOField;

        private string nOVORField;

        /// <summary>
        /// Код записи о паци-енте
        /// Возможно использо-вание уникального идентификатора (учетного кода) паци-ента.
        /// Необходим для связи с файлом персональ-ных данных.
        /// </summary>
        [Size(36)]
        public string ID_PAC
        {
            get { return this.iD_PACField; }
            set { SetPropertyValue("ID_PAC", ref this.iD_PACField, value); }
        }

        /// <summary>
        /// Тип документа, подтверждающего факт страхования по ОМС
        /// Заполняется в соот-ветствии с F008 При-ложения А.
        /// </summary>
        [Size(1)]
        public string VPOLIS
        {
            get { return this.vPOLISField; }
            set { this.vPOLISField = value; }
        }

        /// <summary>
        /// Серия документа, подтверждающего факт страхования по ОМС
        /// </summary>
        [Size(10)]
        public string SPOLIS
        {
            get { return this.sPOLISField; }
            set { SetPropertyValue("NOVOR", ref this.sPOLISField, value); }
        }

        /// <summary>
        /// Номер документа, подтверждающего факт страхования по ОМС
        /// Для полисов единого образца указывается ЕНП
        /// </summary>
        [Size(20)]
        public string NPOLIS
        {
            get { return this.nPOLISField; }
            set { SetPropertyValue("NOVOR", ref this.nPOLISField, value); }
        }

        private string st_okato;
        /// <summary>
        /// Указывается ОКАТО территории выдачи ДПФС для полисов старого образца при наличии данных
        /// </summary>
        [Size(20)]
        public string ST_OKATO
        {
            get { return this.st_okato; }
            set { SetPropertyValue("ST_OKATO", ref this.st_okato, value); }
        }

        /// <summary>
        /// Реестровый номер СМО. 
        /// Заполняется в соот-ветствии со справоч-ником F002 Прило-жения А. При отсут-ствии сведений может не заполняться.
        /// </summary>
        [Size(5)]
        public string SMO
        {
            get { return this.sMOField; }
            set { SetPropertyValue("SMO", ref this.sMOField, value); }
        }

        private string smo_ogrn;
        /// <summary>
        /// ОГРН СМО
        /// Заполняются при не-возможности указать реестровый номер СМО.
        /// </summary>
        [Size(15)]
        public string SMO_OGRN
        {
            get { return this.sMOField; }
            set { SetPropertyValue("SMO_OGRN", ref smo_ogrn, value); }
        }

        private string smo_ok;
        /// <summary>
        /// ОКАТО территории страхования
        /// Заполняются при не-возможности указать реестровый номер СМО.
        /// </summary>
        [Size(5)]
        public string SMO_OK
        {
            get { return this.smo_ok; }
            set { SetPropertyValue("SMO_OK", ref this.smo_ok, value); }
        }

        private string smo_nam;
        /// <summary>
        /// Наименование СМО 
        /// Заполняется при не-возможности указать ни реестровый номер, ни ОГРН СМО.
        /// </summary>
        [Size(100)]
        public string SMO_NAM
        {
            get { return this.smo_nam; }
            set { SetPropertyValue("SMO_NAM", ref this.smo_nam, value); }
        }

        /// <summary>
        /// Признак новорож-дённого
        /// Указывается в случае оказания медицин-ской помощи ребёнку до государственной регистрации рожде-ния.
        /// 0 – признак отсутст-вует.
        /// Если значение при-знака отлично от ну-ля, он заполняется по следующему шабло-ну:
        /// ПДДММГГН, где
        /// П – пол ребёнка в со-ответствии с класси-фикатором V005 Приложения А;
        /// ДД – день рождения;
        /// ММ – месяц рожде-ния;
        /// </summary>
        [Size(9)]
        public string NOVOR
        {
            get { return this.nOVORField; }
            set { SetPropertyValue("NOVOR", ref this.nOVORField, value); }
        }

        private string vnov_d;
        /// <summary>
        /// Вес при рождении
        /// Указывается при ока-зании медицинской помощи недоношен-ным и маловесным детям.
        /// Поле заполняется, если в качестве пациента указан ребёнок.
        /// </summary>
        [Size(4)]
        public string VNOV_D
        {
            get { return this.vnov_d; }
            set { SetPropertyValue("VNOV_D", ref this.vnov_d, value); }
        }
    }

}
