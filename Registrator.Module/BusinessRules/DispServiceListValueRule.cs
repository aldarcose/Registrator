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
    public class DispServiceListValueRule : RuleBase<DispsServiceList>
    {
        public DispServiceListValueRule() : base("DispServiceListValueRule", DefaultContexts.Save) { }
        public DispServiceListValueRule(IRuleBaseProperties properties) : base(properties) { }
        protected override bool IsValidInternal(DispsServiceList target, out string errorMessageTemplate)
        {
            errorMessageTemplate = "";

            if (target.Age < 0)
            {
                errorMessageTemplate =
                        "Неверно указан возраст: должно быть больше 0";
                return false;
            }

            if (target.Type == DispType.DOGVN1 || target.Type == DispType.DOGVN2)
            {
                if (target.Age > 20 && target.Age < 100)
                {
                    if (target.Age%3 != 0)
                        errorMessageTemplate = "Возраст для типа ДОГВН указан неверно: должен быть кратен трем";
                }
                else
                {
                    errorMessageTemplate = "Возраст для типа ДОГВН указан неверно: должен быть в пределах [21 - 99]";
                }
            }
            if (target.Type == DispType.ProfOsmotrAdult)
            {
                if (target.Age < 18)
                {
                    errorMessageTemplate =
                        "Возраст для типа Профосмотр взрослого населения указан неверно: должен быть больше или равно 18";
                }
            }

            if ((target.Type != DispType.DOGVN1 & target.Type != DispType.DOGVN2) & target.Type != DispType.ProfOsmotrAdult)
            {
                if (target.Age > 17)
                {
                    errorMessageTemplate =
                        "Возраст диспансеризаций, осмотров для несовершеннолетних указан неверно";
                }

                if (target.Type != DispType.PreProfOsmotrChild)
                    if (target.EducationInstitute == EducationInstituteType.HighSchoolAfter15 || target.EducationInstitute == EducationInstituteType.HighSchoolTill15)
                        errorMessageTemplate =
                            "Выбранный тип образовательного учреждения определен только для типа Предварительный профосмотр для несовершеннолетних";}

            if (target.Services.Count == 0)
            {
                errorMessageTemplate =
                        "Список услуг не может быть пустым";
            }
            else
            {
                if (target.Gender.HasValue)
                {
                    if (target.Services.Any(t=>t.Service.ForGender.HasValue && t.Service.ForGender.Value != target.Gender.Value))
                        errorMessageTemplate =
                            "Пол, для которого указывается услуга, не совпадает с полом в типе диспансеризации.";
                }

                if (target.Services.Count(t => t.IsMain) != 1)
                {
                    if (target.Services.Any(t => t.Service.ForGender.HasValue && t.Service.ForGender.Value != target.Gender.Value))
                        errorMessageTemplate =
                            "Не указана основная услуга.";
                    if (target.Services.Count(t => t.IsMain) > 1)
                    {
                        errorMessageTemplate =
                            "Указаны несколько основных услуг. Должна быть только одна основная услуга.";
                    }
                }
            }
            

            return string.IsNullOrEmpty(errorMessageTemplate);
        }
    }
}
