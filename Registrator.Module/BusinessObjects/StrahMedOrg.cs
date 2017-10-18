using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System.Xml.Linq;
using DevExpress.ExpressApp.DC;
using Registrator.Module.BusinessObjects.Interfaces;

namespace Registrator.Module.BusinessObjects
{
    /// <summary>
    /// Единый реестр страховых медицинских организаций, осуществляющих деятельность в сфере ОМС субъектов РФ
    /// </summary>
    [DefaultClassOptions]
    public class StrahMedOrg : BaseObject, IOrganization
    {
        public StrahMedOrg(Session session) : base(session) { }

        public string GetTfOkato()
        {
            return this.TF_OKATO;
        }

        /// <summary>
        /// Код СМО
        /// </summary>
        [XafDisplayName("Реестровый код")]
        [Size(20)]
        [VisibleInDetailView(false)]
        [VisibleInListView(true)]
        [VisibleInLookupListView(true)]
        public string Code { get; set; }

        /// <summary>
        /// Полное наименование СМО
        /// </summary>
        [Size(450)]
        [XafDisplayName("Полное наименование")]
        [VisibleInDetailView(true)]
        [VisibleInListView(true)]
        [VisibleInLookupListView(true)]
        public string FullName { get; set; }

        /// <summary>
        /// Краткое наименование СМО
        /// </summary>
        [Size(255)]
        [XafDisplayName("Краткое наименование")]
        [VisibleInDetailView(true)]
        [VisibleInListView(true)]
        [VisibleInLookupListView(true)]
        public string ShortName { get; set; }

        /// <summary>
        /// Номер документа, дающее право на осуществление страховой медицинской деятельности
        /// </summary>
        [Size(50)]
        [XafDisplayName("Номер документа лицензии")]
        [VisibleInDetailView(true)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string License { get; set; }

        /// <summary>
        /// Текстовое представление
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [PersistentAlias("ShortName")]
        public string Text { get { return (string)EvaluateAlias("Text"); } }

        public override string ToString()
        {
            return ShortName;
        }

        #region Неиспользуемые данные
        [Size(10)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string Org { get; set; }
        [Size(10)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string Vedpri { get; set; }
        [Size(10)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string Name_E { get; set; }

        /// <summary>
        /// Дата внесения в реестр
        /// </summary>
        [XafDisplayName("Дата внесения в реестр")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public DateTime? D_Begin { get; set; }
        /// <summary>
        /// Дата исключения из реестра
        /// </summary>
        [XafDisplayName("Дата исключения из реестра")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public DateTime? D_End { get; set; }
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public DateTime? D_Edit { get; set; }

        /// <summary>
        /// Дата выдачи документа
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public DateTime? D_Start { get; set; }
        /// <summary>
        /// Дата окончания срока действия документа
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public DateTime? Data_E { get; set; }

        #endregion

        #region Данные для связи
        /// <summary>
        /// Сайт организации
        /// (Может быть несколько. Разделены ';')
        /// </summary>
        [Size(255)]
        [XafDisplayName("Вебсайт организации")]
        [VisibleInDetailView(true)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string WebSite { get; set; }

        /// <summary>
        /// Факс
        /// </summary>
        [Size(50)]
        [XafDisplayName("Факс организации")]
        [VisibleInDetailView(true)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string Fax { get; set; }

        /// <summary>
        /// Телефон
        /// </summary>
        [Size(50)]
        [XafDisplayName("Телефон организации")]
        [VisibleInDetailView(true)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string Phone { get; set; }

        /// <summary>
        /// Электронная почта организации
        /// </summary>
        [Size(255)]
        [XafDisplayName("Эл. почта организации")]
        [VisibleInDetailView(true)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string Email { get; set; }

        /// <summary>
        /// Юр. адрес организации
        /// </summary>
        [Size(255)]
        [XafDisplayName("Юр. адрес организации")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string AddressJur { get; set; }

        /// <summary>
        /// Почтовый индекс организации
        /// </summary>
        [Size(10)]
        [XafDisplayName("Почтовый индекс юр. адреса организации")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string IndexJur { get; set; }

        /// <summary>
        /// Факт. адрес организации
        /// </summary>
        [Size(255)]
        [XafDisplayName("Факт. адрес организации")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string AddressFact { get; set; }

        /// <summary>
        /// Почтовый индекс организации
        /// </summary>
        [Size(10)]
        [XafDisplayName("Почтовый индекс факт. адреса организации")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string IndexFact { get; set; }

        [XafDisplayName("Юр. адрес")]
        [VisibleInDetailView(true)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [NonPersistent()]
        public string JurAddress
        {
            get
            {
                return string.Format("{0}{1}", IndexJur == null ? string.Empty : IndexJur + ", ", AddressJur);
            }
        }

        [XafDisplayName("Факт. адрес")]
        [VisibleInDetailView(true)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [NonPersistent()]
        public string FactAddress
        {
            get
            {
                return string.Format("{0}{1}", IndexFact == null ? string.Empty : IndexFact + ", ", AddressFact);
            }
        }

        #endregion

        #region Коды организации
        /// <summary>
        /// Код субъекта по ОКАТО, где расположена мед. организация
        /// </summary>
        [Size(20)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string TF_OKATO { get; set; }

        /// <summary>
        /// КПП организации
        /// </summary>
        [Size(20)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string KPP { get; set; }

        /// <summary>
        /// ИНН организации
        /// </summary>
        [Size(20)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string INN { get; set; }

        /// <summary>
        /// ОГРН организации
        /// </summary>
        [Size(20)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string OGRN { get; set; }

        /// <summary>
        /// ОКОПФ организации
        /// </summary>
        [Size(20)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string OKOPF { get; set; }
        #endregion

        #region Руководитель

        /// <summary>
        /// Фамилия руководителя
        /// </summary>
        [Size(100)]
        [XafDisplayName("Фамилия руководителя")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string Ruk_LastName { get; set; }

        /// <summary>
        /// Имя руководителя
        /// </summary>
        [Size(100)]
        [XafDisplayName("Имя руководителя")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string Ruk_FirstName { get; set; }

        /// <summary>
        /// Отчество руководителя
        /// </summary>
        [Size(100)]
        [XafDisplayName("Отчество руководителя")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string Ruk_MiddleName { get; set; }

        [XafDisplayName("Руководитель организации")]
        [VisibleInDetailView(true)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [NonPersistent()]
        public string Ruk_FIO
        {
            get
            {
                return string.Format("{0} {1} {2}", Ruk_LastName, Ruk_FirstName, Ruk_MiddleName);
            }
        }

        #endregion

        /// <summary>
        /// Пока не заполняю
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public List<InsAdvice> Advices { get; set; }

        // пример записи из XML
        /* <insCompany>
         *  <tf_okato>01000</tf_okato>
         *  <smocod>22001</smocod>
         *  <nam_smop>АЛТАЙСКИЙ ФИЛИАЛ ОБЩЕСТВА С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ "СТРАХОВАЯ МЕДИЦИНСКАЯ КОМПАНИЯ РЕСО-МЕД"</nam_smop>
         *  <nam_smok>АЛТАЙСКИЙ ФИЛИАЛ ООО "СМК РЕСО-МЕД"</nam_smok>
         *  <inn>5035000265</inn>
         *  <Ogrn>1025004642519</Ogrn>
         *  <KPP>222302001</KPP>
         *  <jurAddress>
         *      <index_j>142500</index_j>
         *      <addr_j>МОСКОВСКАЯ ОБЛАСТЬ, Г.ПАВЛОВСКИЙ ПОСАД, УЛ.УРИЦКОГО, 26</addr_j>
         *  </jurAddress>
         *  <pstAddress>
         *      <index_f>656054</index_f>
         *      <addr_f>АЛТАЙСКИЙ КРАЙ, Г.БАРНАУЛ, УЛ.ОСТРОВСКОГО, 29</addr_f>
         *  </pstAddress>
         *  <okopf>30002</okopf>
         *  <fam_ruk>ОСИПОВ</fam_ruk>
         *  <im_ruk>ЕВГЕНИЙ</im_ruk>
         *  <ot_ruk>АНДРЕЕВИЧ</ot_ruk>
         *  <phone>(385-2)243637</phone>
         *  <fax>8(3852) 242972</fax>
         *  <e_mail>ALTAIRESOMED@GMAIL.COM</e_mail>
         *  <www>WWW.RESO-MED.COM</www>
         *  <licenziy>
         *      <n_doc>С №0879 50</n_doc>
         *      <d_start>29.09.2008</d_start>
         *      <data_e>31.12.2099</data_e>
         *  </licenziy>
         *  <org>2</org>
         *  <insInclude>
         *      <d_begin>01.01.2011</d_begin>
         *      <d_end></d_end>
         *      <name_e />
         *      <Nal_p></Nal_p>
         *   </insInclude>
         *   <insAdvice>
         *      <YEAR_WORK>2015</YEAR_WORK>
         *      <DUVED>16.07.2014</DUVED>
         *      <kol_zl>1481134</kol_zl>
         *   </insAdvice>
         *   ...
         *   <d_edit>23.01.2014</d_edit>
         * </insCompany>
        */
        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);
            var splitChars = new char[] { ',' };

            foreach (XElement element in doc.Root.Elements())
            {
                string id = element.Element("smocod").Value;
                StrahMedOrg obj = objSpace.FindObject<StrahMedOrg>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", id));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<StrahMedOrg>();
                    obj.Code = id;
                }

                obj.FullName = element.Element("nam_smop").Value;
                obj.ShortName = element.Element("nam_smok").Value;
                obj.License = element.Element("licenziy").Element("n_doc").Value;

                obj.TF_OKATO = element.Element("tf_okato").Value;
                obj.INN = element.Element("inn").Value;
                obj.OGRN = element.Element("Ogrn").Value;
                obj.OKOPF = element.Element("okopf").Value;
                obj.KPP = element.Element("KPP") == null ? string.Empty : element.Element("KPP").Value;

                obj.Ruk_LastName = (element.Element("fam_ruk") == null) ? string.Empty : element.Element("fam_ruk").Value;
                obj.Ruk_FirstName = (element.Element("im_ruk") == null) ? string.Empty : element.Element("im_ruk").Value;
                obj.Ruk_MiddleName = (element.Element("ot_ruk") == null) ? string.Empty : element.Element("ot_ruk").Value;

                obj.Fax = (element.Element("fax") == null) ? string.Empty : element.Element("fax").Value;
                obj.Phone = (element.Element("phone") == null) ? string.Empty : element.Element("phone").Value;
                obj.Email = (element.Element("e_mail") == null) ? string.Empty : element.Element("e_mail").Value;
                obj.WebSite = (element.Element("www") == null) ? string.Empty : element.Element("www").Value;

                obj.IndexJur = (element.Element("jurAddress").Element("index_j") == null) ? string.Empty : element.Element("jurAddress").Element("index_j").Value;
                obj.AddressJur = (element.Element("jurAddress").Element("addr_j") == null) ? string.Empty : element.Element("jurAddress").Element("addr_j").Value;

                obj.IndexFact = (element.Element("pstAddress").Element("index_f") == null) ? string.Empty : element.Element("pstAddress").Element("index_f").Value;
                obj.AddressFact = (element.Element("pstAddress").Element("addr_f") == null) ? string.Empty : element.Element("pstAddress").Element("addr_f").Value;

                obj.Org = (element.Element("org") == null) ? string.Empty : element.Element("org").Value;

                obj.Name_E = (element.Element("insInclude").Element("name_e") == null) ? string.Empty : element.Element("insInclude").Element("name_e").Value;

                var date = DateTime.MinValue;
                date = DateTime.MinValue;
                if (element.Element("licenziy").Element("d_start") != null)
                    DateTime.TryParse(element.Element("licenziy").Element("d_start").Value, out date);
                obj.D_Start = date == DateTime.MinValue ? null : (DateTime?)date;

                if (element.Element("licenziy").Element("data_e") != null)
                    DateTime.TryParse(element.Element("licenziy").Element("data_e").Value, out date);
                obj.Data_E = date == DateTime.MinValue ? null : (DateTime?)date;

                date = DateTime.MinValue;
                if (element.Element("insInclude").Element("d_begin") != null)
                    DateTime.TryParse(element.Element("insInclude").Element("d_begin").Value, out date);
                obj.D_Begin = date == DateTime.MinValue ? null : (DateTime?)date;

                date = DateTime.MinValue;
                if (element.Element("insInclude").Element("d_end") != null)
                    DateTime.TryParse(element.Element("insInclude").Element("d_end").Value, out date);
                obj.D_End = date == DateTime.MinValue ? null : (DateTime?)date;

                date = DateTime.MinValue;
                if (element.Element("d_edit") != null)
                    DateTime.TryParse(element.Element("d_edit").Value, out date);
                obj.D_Edit = date == DateTime.MinValue ? null : (DateTime?)date;
            }
        }
    }

    public class InsAdvice
    {
        public InsAdvice() {}

        public int YearWork {get;set;}
        public DateTime? Duved {get;set;}
        [Size(20)]
        public string KolZl {get;set;}
    }
}
