using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;

using DevExpress.Persistent.Validation;
using DevExpress.Persistent.BaseImpl;

namespace Registrator.Module.BusinessObjects
{
    [DefaultClassOptions]
    [XafDisplayName("Рецепт")]
    public class Recipe : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (http://documentation.devexpress.com/#Xaf/CustomDocument3146).
        public Recipe(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (http://documentation.devexpress.com/#Xaf/CustomDocument2834).
        }
        private string snils;
        [XafDisplayName("СНИЛС"), ToolTip("My hint message")]
        [ModelDefault("EditMask", "000-000-000 00"), Index(0), VisibleInListView(false)]
        [Persistent("DatabaseColumnName"), RuleRequiredField(DefaultContexts.Save)]
        public string Snils
        {
            get { return snils; }
            set { SetPropertyValue("PersistentProperty", ref snils, value); }
        }

        [XafDisplayName("Фамилия")]
        public string Fam
        {
            get;
            set;
        }

        [XafDisplayName("Имя")]
        public string Im
        {
            get;
            set;
        }

        [XafDisplayName("Отчество")]
        public string Ot { get; set; }

        //[Action(Caption = "My UI Action", ConfirmationMessage = "Are you sure?", ImageName = "Attention", AutoCommit = true)]
        //public void ActionMethod() {
        //    // Trigger a custom business logic for the current record in the UI (http://documentation.devexpress.com/#Xaf/CustomDocument2619).
        //    this.PersistentProperty = "Paid";
        //}
    }
}
