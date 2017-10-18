using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Validation;
using Registrator.Module.BusinessObjects;

namespace Registrator.Module.BusinessRules
{
    [CodeRule]
    public class DSValueRule : RuleBase<DnevnoyStacionar>
    {
        public DSValueRule() : base("DSValueRule", DefaultContexts.Save) { }
        public DSValueRule(IRuleBaseProperties properties) : base(properties) { }
        protected override bool IsValidInternal(DnevnoyStacionar target, out string errorMessageTemplate)
        {
            errorMessageTemplate = "";

            var objSpace = this.targetObjectSpace;

            if (target.Pacient == null)
            {
                errorMessageTemplate = "Нет связанного с дневным стационаром пациента!";
                return false;
            }

            // Проверяем есть ли дневной стационар для этого пациента
            var dups = objSpace.GetObjects<DnevnoyStacionar>(CriteriaOperator.Parse("Oid!=? AND Pacient=?", target.Oid, target.Pacient));
            foreach (var dnevnoyStacionar in dups)
            {
                if (dnevnoyStacionar.IsDeleted)
                    continue;
                if (dnevnoyStacionar.DataVypiski == DateTime.MinValue)
                {
                    errorMessageTemplate = string.Format("Пациент стационара не выписан в другом стационаре (от {0})!", dnevnoyStacionar.DataPriema.Date);
                    return false;
                }
            }

            return string.IsNullOrEmpty(errorMessageTemplate);
        }
    }
}
