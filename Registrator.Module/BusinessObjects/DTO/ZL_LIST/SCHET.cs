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
    [DefaultClassOptions]
    public partial class SCHET : BaseObject
    {
        public SCHET(Session session)
            : base(session)
        {
        }
        
        private string cODEField;

        private string cODE_MOField;

        private string yEARField;

        private string mONTHField;

        private string nSCHETField;

        private string dSCHETField;

        private string sUMMAVField;

        private string cOMENTSField;

        /// <summary>
        /// Код записи счета
        /// Уникальный код (на-пример, порядковый номер).
        /// </summary>
        [Size(8)]
        public string CODE
        {
            get { return this.cODEField; }
            set { SetPropertyValue("CODE", ref this.cODEField, value); }
        }

        /// <summary>
        /// Реестровый номер медицинской орга-низации
        /// Код МО – юридиче-ского лица. Заполня-ется в соответствии со справочником F003 Приложения А.
        /// </summary>
        [Size(6)]
        public string CODE_MO
        {
            get { return this.cODE_MOField; }
            set { SetPropertyValue("CODE_MO", ref this.cODE_MOField, value); }
        }

        /// <summary>
        /// Отчетный год
        /// </summary>
        [Size(4)]
        public string YEAR
        {
            get { return this.yEARField; }
            set { SetPropertyValue("YEAR", ref this.yEARField, value); }
        }

        /// <summary>
        /// Отчетный месяц
        /// В счёт могут вклю-чаться случаи лече-ния за предыдущие периоды, если ранее они были отказаны по результатам МЭК, МЭЭ, ЭКМП
        /// </summary>
        [Size(2)]
        public string MONTH
        {
            get { return this.mONTHField; }
            set { SetPropertyValue("MONTH", ref this.mONTHField, value); }
        }

        /// <summary>
        /// Номер счёта
        /// </summary>
        [Size(15)]
        public string NSCHET
        {
            get { return this.nSCHETField; }
            set { SetPropertyValue("NSCHET", ref this.nSCHETField, value); }
        }

        /// <summary>
        /// Дата выставления счёта
        /// В формате ГГГГ-ММ-ДД
        /// </summary>
        [Size(10)]
        public string DSCHET
        {
            get { return this.dSCHETField; }
            set { SetPropertyValue("DSCHET", ref this.dSCHETField, value); }
        }

        /// <summary>
        /// Плательщик. Рее-стровый номер СМО. 
        /// Заполняется в соот-ветствии со справоч-ником F002 Прило-жения А. При отсут-ствии сведений может не заполняться.
        /// </summary>
        public string PLAT { get; set; }

        /// <summary>
        /// Сумма МО, вы-ставленная на оп-лату
        /// </summary>
        [Size(20)]
        public string SUMMAV
        {
            get { return this.sUMMAVField; }
            set { SetPropertyValue("SUMMAV", ref this.sUMMAVField, value); }
        }

        /// <summary>
        /// Служебное поле к счету
        /// </summary>
        [Size(50)]
        public string COMENTS
        {
            get { return this.cOMENTSField; }
            set { SetPropertyValue("COMENTS", ref this.cOMENTSField, value); }
        }

        private string sUMMAPField;
        /// <summary>
        /// Сумма, принятая к оплате СМО 
        /// Заполняется СМО (ТФОМС).
        /// </summary>
        [Size(20)]
        public string SUMMAP
        {
            get { return this.sUMMAPField; }
            set { SetPropertyValue("SUMMAP", ref this.sUMMAPField, value); }
        }

        /// <summary>
        /// Финансовые санкции (МЭК)
        /// Сумма, снятая с оплаты по результатам МЭК, заполняется после проведения МЭК.
        /// </summary>
        [Size(20)]
        public string SANK_MEK { get; set; }

        /// <summary>
        /// Финансовые санкции (МЭЭ)
        /// Сумма, снятая с оплаты по результатам МЭЭ, заполняется после проведения МЭЭ.
        /// </summary>
        [Size(20)]
        public string SANK_MEE { get; set; }

        /// <summary>
        /// Финансовые санкции (ЭКМП)
        /// Сумма, снятая с оплаты по результатам ЭКМП, заполняется после проведения ЭКМП.
        /// </summary>
        [Size(20)]
        public string SANK_EKMP { get; set; }
    }

}
