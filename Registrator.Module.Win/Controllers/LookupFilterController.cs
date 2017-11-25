using System;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Model.NodeGenerators;

namespace Registrator.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class LookupFilterController : ViewController
    {
        public LookupFilterController()
        {
            InitializeComponent();
            RegisterActions(components);
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.

            var filterController = Frame.GetController<FilterController>();
            if (filterController !=null)
            {
                filterController.CustomBuildCriteria += filterController_CustomBuildCriteria;
            }
        }

        void filterController_CustomBuildCriteria(object sender, CustomBuildCriteriaEventArgs e)
        {
            var filter = sender as FilterController;
            if (string.IsNullOrEmpty(e.SearchText))
            {
                e.Criteria = CriteriaOperator.Parse("1=0");
                e.Handled = true;
                return;
            }
            string normalizedText = e.SearchText.ToUpper();
            
            if (View.Id =="MKB10_LookupListView")
            {
                var criteria = CriteriaOperator.Or(
                    CriteriaOperator.Parse("Iif(MKB is null, false, Contains(Upper(MKB),?))", normalizedText),
                    CriteriaOperator.Parse("Contains(Upper(NAME), ?)", normalizedText)
                );
                e.Criteria = criteria;
                e.Handled = true;
                return;
            }
            if (View.Id == "Kladr_LookupListView")
            {
                var criteria = CriteriaOperator.Or(
                    CriteriaOperator.Parse("Contains(Upper(Name), ?)", normalizedText)
                );
                e.Criteria = criteria;
                e.Handled = true;
                return;
            }
            if (View.Id == "TerritorialUsluga_LookupListView")
            {
                var criteria = CriteriaOperator.Or(
                    CriteriaOperator.Parse("Contains(Upper(Code),?)", normalizedText),
                    CriteriaOperator.Parse("Contains(Upper(Name), ?)", normalizedText)
                );
                e.Criteria = criteria;
                e.Handled = true;
                return;
            }
            if (View.Id == "Doctor_LookupListView" || View.Id == "Pacient_LookupListView")
            {
                var criteria = CriteriaOperator.Or(
                    CriteriaOperator.Parse("Contains(Upper(FirstName),?)", normalizedText),
                    CriteriaOperator.Parse("Contains(Upper(LastName), ?)", normalizedText),
                    CriteriaOperator.Parse("Contains(Upper(MiddleName), ?)", normalizedText)
                );
                e.Criteria = criteria;
                e.Handled = true;
                return;
            }

            if (filter != null)
            {
                var list = new List<CriteriaOperator>();
                foreach (var column in filter.View.Model.Columns)
                {
                    list.Add(CriteriaOperator.Parse(string.Format("Contains(Upper({0}), '{1}')", column.Id.ToString(), normalizedText)));
                }
                e.Criteria = CriteriaOperator.Or(list);
                e.Handled = true;
            }
        }

        protected override void OnDeactivated()
        {
            var filterController = Frame.GetController<FilterController>();
            if (filterController != null)
            {
                filterController.CustomBuildCriteria -= filterController_CustomBuildCriteria;
            }

            base.OnDeactivated();
        }
    }
}
