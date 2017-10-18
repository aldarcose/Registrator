using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Registrator.Module.BusinessObjects.Abstract;
using Registrator.Module.BusinessObjects.Dictionaries.BaseDispancerization;
using System.ComponentModel;

namespace Registrator.Module.BusinessObjects.Dictionaries.BaseMedical
{
    /// <summary>
    /// Диагнозы обследования ДЕТЕЙ при диспансеризации
    /// </summary>
    public abstract class MKBWithDispInfo : BaseObject
    {
        protected MKBWithDispInfo(Session session) : base(session) { }
        [XafDisplayName("МКБ")]
        public MKB10 MKB10 { get; set; }

        [XafDisplayName("Диспанс. наблюдение")]
        public DispObsers DispObser { get; set; }
    }

    public enum DispObsers
    {
        [XafDisplayName("Установлено ранее")]
        SettedEarlier = 0,
        [XafDisplayName("Установлено впервые")]
        SetFirstTime = 1,
        [XafDisplayName("Не установлено")]
        NotSet = 2
    }

    /// <summary>
    /// До обследования
    /// </summary>
    [Appearance("Before_RehabVisible", Context = "DetailView", Criteria = "!RehabEnabled", TargetItems = "Rehab", AppearanceItemType = "LayoutItem", Visibility = ViewItemVisibility.Hide)]
    [Appearance("Before_HealVisible", Context = "DetailView", Criteria = "!HealingEnabled", TargetItems = "Heal", AppearanceItemType = "LayoutItem", Visibility = ViewItemVisibility.Hide)]
    public class MKBWithDispInfoBefore : MKBWithDispInfo
    {
        public MKBWithDispInfoBefore(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            DispObser = DispObsers.NotSet;

            Healing = new HealingConditionsBefore(Session);
            Rehabilitation = new HealingConditionsBefore(Session);
        }

        [XafDisplayName("Лечение назначено")]
        [ImmediatePostData]
        public bool HealingEnabled { get; set; }

        [XafDisplayName("Лечение")]
        [DevExpress.Xpo.Aggregated]
        public HealingConditionsBefore Healing { get; set; }

        [XafDisplayName("Реабилитация/СКЛ назначено")]
        [ImmediatePostData]
        public bool RehabEnabled { get; set; }

        [XafDisplayName("Реабилитация/СКЛ")]
        [DevExpress.Xpo.Aggregated]
        
        public HealingConditionsBefore Rehabilitation { get; set; }
        [XafDisplayName("ВМП")]
        public HighTechRecommends HighTechRecommend { get; set; }

        [Association("DispService-MKBWithDispInfoBefore")]
        [Browsable(false)]
        public DispService Service { get; set; }
    }

    public enum HighTechRecommends
    {
        [XafDisplayName("Не рекомендована")]
        NotRecommended = 0,
        [XafDisplayName("Рекомендована и не оказана")]
        RecommendedAndNotPerformed = 1,
        [XafDisplayName("Рекомендована и оказана")]
        RecommendedAndPerformed = 2
    }

    /// <summary>
    /// После обследования
    /// </summary>
    [Appearance("After_RehabVisible", Context = "DetailView", Criteria = "!RehabEnabled", TargetItems = "Rehabilition", AppearanceItemType = "LayoutItem", Visibility = ViewItemVisibility.Hide)]
    [Appearance("After_HealVisible", Context = "DetailView", Criteria = "!HealingEnabled", TargetItems = "Healing", AppearanceItemType = "LayoutItem", Visibility = ViewItemVisibility.Hide)]
    [Appearance("After_ConsulVisible", Context = "DetailView", Criteria = "!AddConsultEnabled", TargetItems = "Consultation", AppearanceItemType = "LayoutItem", Visibility = ViewItemVisibility.Hide)]
    public class MKBWithDispInfoAfter : MKBWithDispInfo
    {
        public MKBWithDispInfoAfter(Session session) : base(session) { }
        public override void AfterConstruction()
        {
            base.AfterConstruction();

            DispObser = DispObsers.NotSet;

            Healing = new HealingConditionsAfter(Session);
            Rehabilitation = new HealingConditionsAfter(Session);
            AdditionalConsult = new HealingConditionsWithAdditionalAfter(Session);
        }

        [XafDisplayName("Выявлен впервые")]
        public bool IsNew { get; set; }

        [XafDisplayName("Рекомендуется ВМП")]
        public bool RecommendsHighTech { get; set; }
        [XafDisplayName("Рекомендуется СМП")]
        public bool RecommendsAmbulance { get; set; }

        [XafDisplayName("Рекомендуется СКЛ")]
        public bool RecommendsResort { get; set; }

        [XafDisplayName("Лечение назначено")]
        [ImmediatePostData]
        public bool HealingEnabled { get; set; }

        [XafDisplayName("Лечение")]
        [DevExpress.Xpo.Aggregated]
        [Appearance("HealVisible", Context = "DetailView", Criteria = "!HealingEnabled", Visibility = ViewItemVisibility.Hide)]
        public HealingConditionsAfter Healing { get; set; }

        [XafDisplayName("Реабилитация/СКЛ назначено")]
        [ImmediatePostData]
        public bool RehabEnabled { get; set; }

        [XafDisplayName("Реабилитация/СКЛ")]
        [DevExpress.Xpo.Aggregated]
        [Appearance("RehabVisible", Context = "DetailView", Criteria = "!RehabEnabled", Visibility = ViewItemVisibility.Hide)]
        public HealingConditionsAfter Rehabilitation { get; set; }

        [XafDisplayName("Доп. консультации назначены")]
        [ImmediatePostData]
        public bool AddConsultEnabled { get; set; }

        [XafDisplayName("Дополнительные консультации")]
        [DevExpress.Xpo.Aggregated]
        [Appearance("AddConsVisible", Context = "DetailView", Criteria = "!AddConsultEnabled", Visibility = ViewItemVisibility.Hide)]
        public HealingConditionsWithAdditionalAfter AdditionalConsult { get; set; }
        
        /// <summary>
        /// Рекомендации
        /// </summary>
        [Size(1000)]
        [XafDisplayName("Рекомендации")]
        public string Recommendation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XafDisplayName("Шаблоны рекомендаций")]
        public IList<RecomendTemplate> RecomendTemplates
        {
            get { return CurrentDoctor.RecomendTemplates; }
        }

        /// <summary>
        /// Текущий доктор
        /// </summary>
        [Browsable(false)]
        [NonPersistent]
        public Doctor CurrentDoctor
        {
            get
            {
                return SecuritySystem.CurrentUser as Doctor;
            }
        }

        [Association("DispService-MKBWithDispInfoAfter")]
        [Browsable(false)]
        public DispService Service { get; set; }
    }
}
