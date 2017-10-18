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
using System.ComponentModel;
using DevExpress.Data.Filtering;

namespace Registrator.Module.BusinessObjects
{
    /// <summary>
    /// Единый реестр медицинских организаций, осуществляющих деятельность в сфере ОМС субъектов РФ
    /// </summary>
    [DefaultClassOptions]
    [DefaultProperty("ShortName")]
    public class MedOrg : BaseObject, IOrganization
    {
        public MedOrg(Session session) : base(session) { }

        public string GetTfOkato()
        {
            return this.TF_OKATO;
        }

        /// <summary>
        /// Код медицинской организации
        /// </summary>
        [XafDisplayName("Реестровый код")]
        [VisibleInDetailView(false)]
        [VisibleInListView(true)]
        [VisibleInLookupListView(true)]
        [Size(20)]
        public string Code { get; set; }

        /// <summary>
        /// Полное наименование медицинской организации
        /// </summary>
        [Size(450)]
        [XafDisplayName("Полное наименование")]
        [VisibleInDetailView(true)]
        [VisibleInListView(true)]
        [VisibleInLookupListView(true)]
        public string FullName { get; set; }

        /// <summary>
        /// Краткое наименование медицинской организации
        /// </summary>
        [Size(255)]
        [XafDisplayName("Краткое наименование")]
        [VisibleInDetailView(true)]
        [VisibleInListView(true)]
        [VisibleInLookupListView(true)]
        public string ShortName { get; set; }

        /// <summary>
        /// Номер документа, дающее право на осуществление медицинской деятельности
        /// </summary>
        [Size(50)]
        [XafDisplayName("Номер документа лицензии")]
        [VisibleInDetailView(true)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string License { get; set; }
        
        /// <summary>
        /// Виды мед. помощи, оказываемые организацией
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public List<Dictionaries.VidMedPomoshi> VidiPomoshi { get; set; }

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
        public string Org {get;set;}
        [Size(10)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string Vedpri{get;set;}
        [Size(10)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string Name_E {get;set;}

        /// <summary>
        /// Дата внесения в реестр
        /// </summary>
        [XafDisplayName("Дата внесения в реестр")]
        [VisibleInDetailView(true)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public DateTime? D_Begin {get;set;}
        /// <summary>
        /// Дата исключения из реестра
        /// </summary>
        [XafDisplayName("Дата исключения из реестра")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public DateTime? D_End {get;set;}
        
        /// <summary>
        /// Дата редактирования в рестре
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public DateTime? D_Edit {get;set;}

        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public DateTime? Duved {get;set;}

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
        /// Представляет в строке и индекс и адрес.
        /// </summary>
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

        // пример записи из XML
        /* <REC
         *  TF_OKATO="01000"
         *  MCOD="220001"
         *  NAM_MOP="КРАЕВОЕ ГОСУДАРСТВЕННОЕ БЮДЖЕТНОЕ УЧРЕЖДЕНИЕ ЗДРАВООХРАНЕНИЯ &quot;АЛЕЙСКАЯ ЦЕНТРАЛЬНАЯ РАЙОННАЯ БОЛЬНИЦА&quot;"
         *  NAM_MOK="КГБУЗ &quot;АЛЕЙСКАЯ ЦРБ&quot;"
         *  INN="2201002996"
         *  OGRN="1022200509132"
         *  KPP="220101001"
         *  INDEX_J="658130"
         *  ADDR_J="АЛТАЙСКИЙ КРАЙ, АЛЕЙСК, УЛ.ОЛЕШКО, 30"
         *  OKOPF="72"
         *  VEDPRI="2"
         *  ORG="1"
         *  FAM_RUK="ПАСТУХОВА"
         *  IM_RUK="НАТАЛЬЯ"
         *  OT_RUK="ИВАНОВНА"
         *  PHONE="8(385-53) 22187"
         *  FAX="8(385-53) 24195"
         *  E_MAIL="ALS8066937@AB.RU"
         *  D_BEGIN="01.01.2011"
         *  D_END="15.03.2013"
         *  NAME_E="4"
         *  N_DOC="ЛО-22-01-000130"
         *  D_START="15.08.2008"
         *  DATA_E="31.12.2099"
         *  MP="1,2,3"
         *  WWW="WWW.BUS.GOV.RU; ALEYSKAYA-CRB.RU"
         *  DUVED="23.07.2013"
         *  D_EDIT="01.08.2013"
         * />
        */
        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);
            var splitChars = new char[] {','};

            foreach (XElement element in doc.Root.Elements())
            {
                string id = element.Attribute("MCOD").Value;
                MedOrg obj = objSpace.FindObject<MedOrg>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", id));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<MedOrg>();
                    obj.Code = id;
                }

                obj.FullName = element.Attribute("NAM_MOP").Value;
                obj.ShortName = element.Attribute("NAM_MOK").Value;
                obj.License = element.Attribute("N_DOC").Value;

                // получаем список оказываемых организацией кодов видов помощи
                if (element.Attribute("MP") != null)
                {
                    var mp = element.Attribute("MP").Value.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);

                    // формируем список оказываемых видов помощи из кодов
                    foreach (var mpCode in mp)
                    {
                        var vid = objSpace.FindObject<Dictionaries.VidMedPomoshi>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", mpCode));
                        if (obj.VidiPomoshi == null) obj.VidiPomoshi = new List<Dictionaries.VidMedPomoshi>();
                        if (vid != null && !obj.VidiPomoshi.Contains(vid))
                        {
                            obj.VidiPomoshi.Add(vid);
                        }
                    }
                }

                obj.TF_OKATO = element.Attribute("TF_OKATO").Value;
                obj.INN = element.Attribute("INN").Value;
                obj.OGRN = element.Attribute("OGRN").Value;
                obj.OKOPF = element.Attribute("OKOPF").Value;
                obj.KPP = element.Attribute("KPP") == null ? string.Empty : element.Attribute("KPP").Value;

                obj.Ruk_LastName = (element.Attribute("FAM_RUK") == null) ? string.Empty : element.Attribute("FAM_RUK").Value;
                obj.Ruk_FirstName = (element.Attribute("IM_RUK") == null) ? string.Empty : element.Attribute("IM_RUK").Value;
                obj.Ruk_MiddleName = (element.Attribute("OT_RUK") == null) ? string.Empty : element.Attribute("OT_RUK").Value;

                obj.Fax = (element.Attribute("FAX") == null) ? string.Empty : element.Attribute("FAX").Value;
                obj.Phone = (element.Attribute("PHONE") == null) ? string.Empty : element.Attribute("PHONE").Value;
                obj.Email = (element.Attribute("E_MAIL") == null) ? string.Empty : element.Attribute("E_MAIL").Value;
                obj.WebSite = (element.Attribute("WWW") == null) ? string.Empty : element.Attribute("WWW").Value;
                obj.IndexJur = (element.Attribute("INDEX_J") == null) ? string.Empty : element.Attribute("INDEX_J").Value;
                obj.AddressJur = (element.Attribute("ADDR_J") == null) ? string.Empty : element.Attribute("ADDR_J").Value;

                obj.Org = (element.Attribute("ORG") == null) ? string.Empty : element.Attribute("ORG").Value;
                obj.Vedpri = (element.Attribute("VEDPRI") == null) ? string.Empty : element.Attribute("VEDPRI").Value;
                obj.Name_E = (element.Attribute("NAME_E") == null) ? string.Empty : element.Attribute("NAME_E").Value;

                var date = DateTime.MinValue;
                if (element.Attribute("DATA_E") != null)
                    DateTime.TryParse(element.Attribute("DATA_E").Value, out date);
                obj.Data_E = date == DateTime.MinValue ? null : (DateTime?)date;

                date = DateTime.MinValue;
                if (element.Attribute("D_BEGIN") != null)
                    DateTime.TryParse(element.Attribute("D_BEGIN").Value, out date);
                obj.D_Begin = date == DateTime.MinValue ? null : (DateTime?)date;

                date = DateTime.MinValue;
                if (element.Attribute("D_END") != null)
                    DateTime.TryParse(element.Attribute("D_END").Value, out date);
                obj.D_End = date == DateTime.MinValue ? null : (DateTime?)date;

                date = DateTime.MinValue;
                if (element.Attribute("D_START") != null)
                    DateTime.TryParse(element.Attribute("D_START").Value, out date);
                obj.D_Start = date == DateTime.MinValue ? null : (DateTime?)date;

                date = DateTime.MinValue;
                if (element.Attribute("D_EDIT") != null)
                    DateTime.TryParse(element.Attribute("D_EDIT").Value, out date);
                obj.D_Edit = date == DateTime.MinValue ? null : (DateTime?)date;

                date = DateTime.MinValue;
                if (element.Attribute("DUVED") != null)
                    DateTime.TryParse(element.Attribute("DUVED").Value, out date);
                obj.Duved = date == DateTime.MinValue ? null : (DateTime?)date;
            }
        }
    }
}
