using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;

namespace Registrator.Module.BusinessObjects.Dictionaries.BaseDispancerization
{
    public class Rehabilitation : BaseObject
    {
        public Rehabilitation(Session session) : base(session) { }

        [XafDisplayName("Нуждается в реабилитации")]
        [ImmediatePostData(true)]
        public bool IsNeed { get; set; }

        [Appearance("DateHide", Criteria = "!IsNeed", Visibility = ViewItemVisibility.Hide, Context = "DispanserizaionCase_DetailView")]
        [XafDisplayName("Дата назначения")]
        public DateTime SetDate { get; set; }

        [Appearance("ProgressHide", Criteria = "!IsNeed", Visibility = ViewItemVisibility.Hide, Context = "DispanserizaionCase_DetailView")]
        [XafDisplayName("Степень выполнения")]
        public RehabilitationProgress Progress { get; set; }
    }

    public enum RehabilitationProgress
    {
        [XafDisplayName("Полностью")]
        Done = 0,
        [XafDisplayName("Частично")]
        Partial = 1,
        [XafDisplayName("Начато")]
        Started = 2,
        [XafDisplayName("Не выполнено")]
        NotDone =3
    }
}
