using System;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.ReportsV2;
using Registrator.Module.BusinessObjects;

namespace Registrator.Module.Reports
{
    [DomainComponent]
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/#Xaf/CustomDocument3594.
    public class VisitReportParametersObject : ReportParametersObjectBase
    {
        public VisitReportParametersObject(IObjectSpaceCreator provider) : base(provider) { }

        public VisitCase Visit { get; set; }
        protected override IObjectSpace CreateObjectSpace()
        {
            return objectSpaceCreator.CreateObjectSpace(typeof(object));
        }
        public override CriteriaOperator GetCriteria()
        {
            CriteriaOperator criteria = new BinaryOperator("Oid", Visit.Oid);
            return criteria;
        }
        public override SortProperty[] GetSorting()
        {
            SortProperty[] sorting = { new SortProperty("DateIn", SortingDirection.Descending) };
            return sorting;
        }

    }
}
