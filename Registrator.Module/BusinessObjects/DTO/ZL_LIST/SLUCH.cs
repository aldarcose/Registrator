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
    public partial class SLUCH : BaseObject
    {
        public SLUCH(Session session)
            : base(session)
        {
        }

        [Association("ZAP_SLUCH")]
        public ZAP ZAP { get; set; }

        /// <summary>
        /// Номер записи в реестре случаев
        /// Соответствует поряд-ковому номеру записи реестра счёта на бумажном носителе при его предоставлении.
        /// </summary>
        [Size(11)]
        public string IDCASE { get; set; }


        /// <summary>
        /// Условия оказания медицинской помощи
        /// Классификатор условий оказания медицинской помощи (V006 Приложения А).
        /// </summary>
        [Size(2)]
        public string USL_OK { get; set; }

        /// <summary>
        /// Вид медицинской помощи
        /// Классификатор видов медицинской помощи. Справочник V008 Приложения А.
        /// </summary>
        [Size(4)]
        public string VIDPOM { get; set; }

        /// <summary>
        /// Форма оказания медицинской помощи
        /// Классификатор форм оказания медицинской помощи. Спра-вочник V014 Приложения А
        /// </summary>
        [Size(1)]
        public string FOR_POM { get; set; }

        /// <summary>
        /// Код МО, направившего на лечение (диагностику, консультацию)
        /// Код МО – юридического лица. Заполня-ется в соответствии со справочником F003 Приложения А. При отсутствии сведений может не заполняться.
        /// </summary>
        [Size(6)]
        public string NPR_MO { get; set; }

        /// <summary>
        /// Направление (гос-питализация)
        /// 1 –плановая; 2 – экс-тренная
        /// </summary>
        [Size(2)]
        public string EXTR { get; set; }

        /// <summary>
        /// Код МО
        /// МО лечения, указы-вается в соответствии с реестром F003
        /// </summary>
        [Size(6)]
        public string LPU { get; set; }

        /// <summary>
        /// Подразделение МО
        /// Подразделение МО лечения из регио-нального справочни-ка.
        /// </summary>
        [Size(8)]
        public string LPU_1 { get; set; }

        /// <summary>
        /// Код отделения
        /// Отделение МО лече-ния из регионального справочника.
        /// </summary>
        [Size(8)]
        public string PODR { get; set; }

        /// <summary>
        /// Профиль
        /// Классификатор V002 Приложения А
        /// </summary>
        [Size(3)]
        public string PROFIL { get; set; }

        /// <summary>
        /// Признак детского профиля
        /// 0-нет, 1-да.
        /// Заполняется в зави-симости от профиля оказанной медицин-ской помощи.
        /// </summary>
        [Size(1)]
        public string DET { get; set; }

        /// <summary>
        /// Номер истории бо-лезни/ талона ам-булаторного паци-ента/ карты вызова скорой медицин-ской помощи
        /// </summary>
        [Size(50)]
        public string NHISTORY { get; set; }

        /// <summary>
        /// Дата начала лече-ния
        /// </summary>
        [Size(10)]
        public string DATE_1 { get; set; }

        /// <summary>
        /// Дата окончания ле-чения
        /// </summary>
        [Size(10)]
        public string DATE_2 { get; set; }

        /// <summary>
        /// Диагноз первичный
        /// Код из справочника МКБ до уровня под-рубрики. Указывается при наличии
        /// </summary>
        [Size(10)]
        public string DS0 { get; set; }
        
        /// <summary>
        /// Диагноз основной
        /// Код из справочника МКБ до уровня под-рубрики. 
        /// </summary>
        [Size(10)]
        public string DS1 { get; set; }

        //private List<string> ds2;
        ///// <summary>
        ///// Диагноз сопутст-вующего заболева-ния
        ///// Код из справочника МКБ до уровня под-рубрики. Указывается в случае установле-ния в соответствии с медицинской доку-ментацией.
        ///// </summary>
        //public List<string> DS2
        //{
        //    get
        //    {
        //        if (ds2 == null)
        //        {
        //            ds2 = new List<string>();
        //        }
        //        return ds2;
        //    }
        //    set { SetPropertyValue("DS2", ref ds2, value); }
        //}

        //private List<string> ds3;
        ///// <summary>
        ///// Диагноз осложне-ния заболевания
        ///// Код из справочника МКБ до уровня под-рубрики. Указывается в случае установле-ния в соответствии с медицинской доку-ментацией.
        ///// </summary>
        //public List<string> DS3
        //{
        //    get
        //    {
        //        if (ds3 == null)
        //        {
        //            ds3 = new List<string>();
        //        }
        //        return ds3;
        //    }
        //    set { SetPropertyValue("DS3", ref ds3, value); }
        //}

        /// <summary>
        /// Результат обраще-ния/ госпитализа-ции
        /// Классификатор ре-зультатов обращения за медицинской по-мощью (Приложение А V009).
        /// </summary>
        [Size(3)]
        public string RSLT { get; set; }

        /// <summary>
        /// Исход заболевания
        /// Классификатор исхо-дов заболевания (Приложение А V012).
        /// </summary>
        [Size(3)]
        public string ISHOD { get; set; }

        /// <summary>
        /// Специальность ле-чащего врача/ вра-ча, закрывшего та-лон
        /// Классификатор меди-цинских специально-стей (Приложение А V015).Указывается значение параметра «Code»
        /// </summary>
        [Size(4)]
        public string PRVS { get; set; }

        /// <summary>
        /// Код классификато-рамедицинских специальностей
        /// Указывается код ис-пользуемого справоч-ника медицинских специальностей. От-сутствие поля обо-значает использова-ние справочника V004
        /// </summary>
        [Size(4)]
        public string VERS_SPEC { get; set; }


        /// <summary>
        /// Код врача, закрыв-шего та-лон/историю бо-лезни
        /// Территориальный справочник
        /// </summary>
        [Size(25)]
        public string IDDOKT { get; set; }

        /// <summary>
        /// Код способа опла-ты медицинской помощи
        /// Классификатор спо-собов оплаты меди-цинской помощи V010
        /// </summary>
        [Size(2)]
        public string IDSP { get; set; }

        /// <summary>
        /// Количество единиц оплаты медицин-ской помощи
        /// </summary>
        [Size(10)]
        public string ED_COL { get; set; }

        /// <summary>
        /// Тариф
        /// </summary>
        [Size(20)]
        public string TARIF { get; set; }

        /// <summary>
        /// Сумма, выставлен-ная к оплате
        /// </summary>
        [Size(20)]
        public string SUMV { get; set; }

        /// <summary>
        /// Тип оплаты
        /// Оплата случая оказа-ния медпомощи:
        /// 0- не принято реше-ние об оплате
        /// 1 – полная;
        /// 2 – полный отказ;
        /// 3 – частичный отказ
        /// </summary>
        [Size(1)]
        public string OPLATA { get; set; }
        
        /// <summary>
        /// Сумма, принятая к оплате СМО (ТФОМС)
        /// </summary>
        [Size(20)]
        public string SUMP { get; set; }

        /// <summary>
        /// Служебное поле
        /// </summary>
        [Size(250)]
        public string COMENTSL { get; set; }

        /// <summary>
        /// Сведения об услуге
        /// Описывает услуги, оказанные в рамках данного случая
        /// </summary>
        [Aggregated, Association("SLUCH_USL")]
        public XPCollection<USL> USL
        {
            get { return GetCollection<USL>("USL"); }
        }

        public string VNOV_M { get; set; }

        public string CODE_MES1 { get; set; }

        public string CODE_MES2 { get; set; }

        public string OS_SLUCH { get; set; }

        public string SANK_IT { get; set; }

        public string SANK { get; set; }
    }

}
