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
    public class PacientReportParametersObject : ReportParametersObjectBase
    {
        public PacientReportParametersObject(IObjectSpaceCreator provider) : base(provider) { }
        
        public Pacient Pacient { get; set; }

        protected override IObjectSpace CreateObjectSpace()
        {
            return objectSpaceCreator.CreateObjectSpace(typeof(Pacient));
        }
        public override CriteriaOperator GetCriteria()
        {
            CriteriaOperator criteria = new BinaryOperator("Oid", Pacient.Oid);
            return criteria;
        }
        public override SortProperty[] GetSorting()
        {
            SortProperty[] sorting = {  };
            return sorting;
        }

    }
}
