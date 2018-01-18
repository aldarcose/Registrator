using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.Module.BusinessObjects
{
    /// <summary>
    /// Статус оплаты СМО. Заполняется ТФОМС. При отправке используем 0
    /// </summary>
    public enum Oplata
    {
        [XafDisplayName("Не принято решение об оплате")]
        NetResheniya = 0,
        [XafDisplayName("Полная")]
        Polnaya = 1,
        [XafDisplayName("Отказ")]
        Otkaz = 2,
        [XafDisplayName("Частичный отказ")]
        ChastichniyOtkaz = 3
    }

    /// <summary>
    /// Пол пациента
    /// </summary>
    public enum Gender
    {
        [XafDisplayName("Женский")]
        Female,
        [XafDisplayName("Мужской")]
        Male
    }

    /// <summary>
    /// Признак детского профиля
    /// </summary>
    public enum PriznakDetProfila
    {
        [XafDisplayName("Нет")]
        No = 0,
        [XafDisplayName("Да")]
        Yes = 1
    }

    /// <summary>
    /// Признак особого случая
    /// </summary>
    public enum PriznakOsobogoSluchaya
    {
        /*
        1 – медицинская помощь оказана новорожденному ребенку до государственной регистрации рождения при многоплодных родах;
        2 – в документе, удостоверяющем личность пациента /родителя (представителя) пациента, отсутствует отчество.*/
        [XafDisplayName("Новорожд. до гос. регистрации при многоплодных родах")]
        NovorozhdeniyDoRegistracii = 1,
        [XafDisplayName("Отсутствует отчество")]
        NetOtchestva = 2
    }

    /// <summary>
    /// Инвалидность
    /// </summary>
    public enum Invalidnost
    {
        [XafDisplayName("Не установлена")]
        Net = 0,
        [XafDisplayName("Установлена впервые")]
        Vpervie = 1,
        [XafDisplayName("Установлена повторно")]
        Povtorno = 2
    }

    public enum GrupaInvalidnosti
    {
        [XafDisplayName("Инвалиды до 18 лет")]
        Deti = 0,
        [XafDisplayName("1 группа")]
        Pervaya = 1,
        [XafDisplayName("2 группа")]
        Vtoraya = 2,
        [XafDisplayName("3 группа")]
        Tretiya = 3
    }

    /// <summary>
    /// Цель посещения, используется только для случаев посещения (может быть и нет)
    /// </summary>
    public enum CelPosescheniya
    {
        [XafDisplayName("Лечебно-диагностическая")]
        LechebnoDiagnosticheskaya = 0,
        [XafDisplayName("Консультативная")]
        Konsultativnaya = 1,
        [XafDisplayName("Осмотр по МСЭК")]
        OsmotrMSEK = 2,
        [XafDisplayName("Патронаж")]
        Patronazh = 3,
        [XafDisplayName("Профосмотр")]
        ProfOsmotr = 4,
        [XafDisplayName("Прочее")]
        Prochee = 5
    }

    /// <summary>
    /// Место обслуживания пациента
    /// </summary>
    public enum MestoObsluzhivaniya
    {
        [XafDisplayName("В поликлинике")]
        LPU = 0,
        [XafDisplayName("На дому")]
        Doma = 1,
        [XafDisplayName("На дому активно")]
        DomaAktivno = 2
    }

    /// <summary>
    /// Тип диагноза для услуг
    /// </summary>
    public enum TipDiagnoza
    {
        [XafDisplayName("Основной")]
        Main = 1,
        [XafDisplayName("Сопутствующий")]
        Soputstvuyschiy = 2,
        [XafDisplayName("Осложнение")]
        Oslozhnenie = 3,
        [XafDisplayName("Фоновое")]
        Phonovoe = 4,
        [XafDisplayName("Конкурируещее")]
        Konkurirueschee = 5,
        [XafDisplayName("Подозрение")]
        Podozrenie = 6
    }

    /// <summary>
    /// Характер диагноза
    /// </summary>
    public enum KharakterDiagnoza
    {
        [XafDisplayName("Нет")]
        Net = 0,
        [XafDisplayName("Острый")]
        Ostriy = 1,
        [XafDisplayName("Хронический")]
        Khronichskiy = 2,
        [XafDisplayName("Хронический, выявленный впервые")]
        KhronicheskiyVpervie = 3,
    }

    /// <summary>
    /// Стадия диагноза
    /// </summary>
    public enum StadiaDiagnoza
    {
        [XafDisplayName("Нет")]
        Net = 0,
        [XafDisplayName("Ранее известное хроническое")]
        IzvestnoeKhronic = 1,
        [XafDisplayName("Выявленное во время ДД")]
        DD = 2,
        [XafDisplayName("Выявленное во время ДД на поздней стадии")]
        DDPozdnoyayaStadia= 3
    }

    public enum HealthGroupObs
    {
        [XafDisplayName("1")]
        One = 1,
        [XafDisplayName("2")]
        Two = 2,
        [XafDisplayName("3")]
        Three = 3,
        [XafDisplayName("4")]
        Four = 4,
        [XafDisplayName("5")]
        Five = 5
    }

    public enum HealthGroupForSportObs
    {
        [XafDisplayName("-1")]
        NegOne = -1,
        [XafDisplayName("1")]
        One = 1,
        [XafDisplayName("2")]
        Two = 2,
        [XafDisplayName("3")]
        Three = 3,
        [XafDisplayName("4")]
        Four = 4,
    }
}
