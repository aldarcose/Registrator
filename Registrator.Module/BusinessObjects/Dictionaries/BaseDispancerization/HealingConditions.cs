using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Registrator.Module.BusinessObjects.Dictionaries.BaseDispancerization
{
    /// <summary>
    /// Условия лечения/реабилитации
    /// </summary>
    public abstract class HealingConditions : BaseObject
    {
        protected HealingConditions(Session session) : base(session) { }
    }
    public class HealingConditionsBefore : HealingConditions
    {
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            NotDone = false;
        }

        public HealingConditionsBefore(Session session) : base(session) { }
        [XafDisplayName("Условия")]
        public MedConditions HealCondition { get; set; }
        [XafDisplayName("Учреждения")]
        public MedOrganizations MedOrganization { get; set; }

        [XafDisplayName("Лечение не выполнено")]
        public bool NotDone { get; set; }

        [XafDisplayName("Причины невыполнения")]
        public NotDoneReasons NotDoneReason { get; set; }
        [XafDisplayName("Иная причина")]
        [Size(255)]
        public string AnotherReason { get; set; }
    }

    public class HealingConditionsAfter : HealingConditions
    {
        public HealingConditionsAfter(Session session) : base(session) { }
        [XafDisplayName("Условия")]
        public MedConditions HealCondition { get; set; }
        [XafDisplayName("Учреждения")]
        public MedOrganizations MedOrganization { get; set; }
    }

    public class HealingConditionsWithAdditionalAfter : HealingConditions
    {
        public HealingConditionsWithAdditionalAfter(Session session) : base(session) { }

        [XafDisplayName("Условия")]
        public MedConditions HealCondition { get; set; }

        [XafDisplayName("Учреждения")]
        public MedOrganizations MedOrganization { get; set; }

        [XafDisplayName("Дополнительные консультации")]
        public AdditionalConsultations AdditionalConsultation { get; set; }
    }

    public enum MedConditions
    {
        [XafDisplayName("Амбулаторные условия")]
        AmbulConditions = 0,
        [XafDisplayName("Стационарные условия")]
        StacionConditions = 1,
        [XafDisplayName("Условия дневного стационара")]
        DayStacionConditions = 2
    }

    public enum MedOrganizations
    {
        [XafDisplayName("Муниципальная мед. организация")]
        Municipal = 0,
        [XafDisplayName("Гос. мед. организация субъекта РФ")]
        State = 1,
        [XafDisplayName("Федеральная мед. организация")]
        Federal = 2,
        [XafDisplayName("Частная мед. организация")]
        Private = 3,
        [XafDisplayName("Санаторно-курортная организация")]
        Resort = 4
    }

    public enum NotDoneReasons
    {
        [XafDisplayName("Отсутствие на момент проведения диспансеризации")]
        NoPacient = 0,
        [XafDisplayName("Отказ от мед. вмешательства")]
        PacientResigns = 1,
        [XafDisplayName("Смена места жительства")]
        Relocating = 2,
        [XafDisplayName("Выполнение не в полном объеме")]
        Partial = 3,
        [XafDisplayName("Проблемы организации мед. помощи")]
        MedOrgProblems = 4,
        [XafDisplayName("Прочие")]
        Another = 9
    }

    public enum AdditionalConsultations
    {
        [XafDisplayName("Не выполнены в соответствии с назначением")]
        NotPerformed = 0,
        [XafDisplayName("Выполнены в полном объеме")]
        Performed = 1,
        [XafDisplayName("Выполнены не в полном объеме")]
        PartialPerformed = 2

    }
}
