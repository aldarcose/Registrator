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
    public partial class USL : BaseObject
    {
        public USL(Session session)
            : base(session)
        {
        }

        [Association("SLUCH_USL")]
        public SLUCH SLUCH { get; set; }
        
        /// <summary>
        /// Номер записи в реестре услуг
        /// Уникален в пределах случая
        /// </summary>
        [Size(36)]
        public string IDSERV { get; set; }

        /// <summary>
        /// Код МО
        /// МО лечения, указывается в соответствии с реестром F003
        /// </summary>
        [Size(6)]
        public string LPU { get; set; }

        /// <summary>
        /// Подразделение  МО
        /// Подразделение МО лечения из регионального справочника
        /// </summary>
        [Size(8)]
        public string LPU_1 { get; set; }

        /// <summary>
        /// Код отделения
        /// Отделение МО лечения из регионального справочника
        /// </summary>
        [Size(8)]
        public string PODR { get; set; }

        /// <summary>
        /// Профиль
        /// Классификатор V002Приложения А.
        /// </summary>
        [Size(3)]
        public string PROFIL { get; set; }

        /// <summary>
        /// Вид медицинского вмешательства
        /// Указывается в соответствии с номенклатурой медицинских услуг (V001)
        /// </summary>
        [Size(15)]
        public string VID_VME { get; set; }

        /// <summary>
        /// Признак детского профиля
        /// 0-нет, 1-да.
        /// Заполняется в зависимости от профиля оказанной медицинской помощи.
        /// </summary>
        [Size(1)]
        public string DET { get; set; }

        /// <summary>
        /// Дата начала оказания услуги
        /// </summary>
        [Size(10)]
        public string DATE_IN { get; set; }

        /// <summary>
        /// Дата окончания оказания услуги
        /// </summary>
        [Size(10)]
        public string DATE_OUT { get; set; }

        /// <summary>
        /// Диагноз
        /// Код из справочника МКБ до уровня подрубрики
        /// </summary>
        [Size(10)]
        public string DS { get; set; }

        /// <summary>
        /// Код услуги
        /// Территориальный классификаторуслуг
        /// </summary>
        [Size(20)]
        public string CODE_USL { get; set; }

        /// <summary>
        /// Количество услуг (кратность услуги)
        /// </summary>
        [Size(10)]
        public string KOL_USL { get; set; }

        /// <summary>
        /// Тариф 
        /// </summary>
        [Size(20)]
        public string TARIF { get; set; }

        /// <summary>
        /// Стоимость медицинской услуги, принятая к оплате (руб.)
        /// </summary>
        [Size(20)]
        public string SUMV_USL { get; set; }

        /// <summary>
        /// Специальность медработника, выполнившего услугу
        /// </summary>
        [Size(9)]
        public string PRVS { get; set; }

        /// <summary>
        /// Код медицинского работника, оказавшего медицинскую услугу
        /// В соответствии с территориальным справочником
        /// </summary>
        [Size(25)]
        public string CODE_MD { get; set; }

        /// <summary>
        /// Служебное поле
        /// </summary>
        [Size(250)]
        public string COMENTU { get; set; }
    }

}
