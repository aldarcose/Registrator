using DevExpress.ExpressApp.DC;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    public enum DispType
    {
        [XafDisplayName("Профосмотр взрослого населения")]
        ProfOsmotrAdult = 0,
        [XafDisplayName("Диспансеризация определенных групп взрослого населения (1 этап)")]
        DOGVN1 = 1,
        [XafDisplayName("Диспансеризация определенных групп взрослого населения (2 этап)")]
        DOGVN2 = 2,
        [XafDisplayName("Профосмотр несовершеннолетних")]
        ProfOsmotrChild = 3,
        [XafDisplayName("Предварительный профосмотр несовершеннолетних")]
        PreProfOsmotrChild = 4,
        [XafDisplayName("Периодический профосмотр несовершеннолетних")]
        PeriodProfOsmotrChild = 5,
        [XafDisplayName("Диспансеризация пребывающих в стационарных учреждениях детей-сирот и детей в трудной жизненной ситуации (1 этап)")]
        DispStacionarChildOrphan1 = 6,
        [XafDisplayName("Диспансеризация пребывающих в стационарных учреждениях детей-сирот и детей в трудной жизненной ситуации (1 и 2 этап)")]
        DispStacionarChildOrphan12 = 7,
        [XafDisplayName("Диспансеризация детей-сирот и детей, оставшихся без попечения родителей (1 этап)")]
        DispChildOrphan1 = 8,
        [XafDisplayName("Диспансеризация детей-сирот и детей, оставшихся без попечения родителей (1 и 2 этап)")]
        DispChildOrphan12 = 9
    }
}
