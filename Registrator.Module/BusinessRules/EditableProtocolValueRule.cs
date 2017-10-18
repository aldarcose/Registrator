using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Persistent.Validation;
using Registrator.Module.BusinessObjects.Dictionaries;

namespace Registrator.Module.BusinessRules
{
    [CodeRule]
    public class EditableProtocolValueRule : RuleBase<EditableProtocol>
    {
        public EditableProtocolValueRule() : base("EditableProtocolValueRule", DefaultContexts.Save) { }
        public EditableProtocolValueRule(IRuleBaseProperties properties) : base(properties) { }

        protected override bool IsValidInternal(EditableProtocol target, out string errorMessageTemplate)
        {
            errorMessageTemplate = "";
            if (target.Records.Where(record => record.Type.Type != TypeEnum.Complex && record.Type.IsRequired).Any(record => string.IsNullOrEmpty(record.Value)))
            {
                errorMessageTemplate = "Не заполнены все обязательные поля.";
                return false;
            }

            return string.IsNullOrEmpty(errorMessageTemplate);
        }
    }
}
