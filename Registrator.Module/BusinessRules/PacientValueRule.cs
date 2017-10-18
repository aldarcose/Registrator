using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Validation;
using System.Globalization;
using Registrator.Module.BusinessObjects;
using Registrator.Module.BusinessObjects.Dictionaries;
using System.Text.RegularExpressions;
namespace Registrator.Module.BusinessRules
{
    /// <summary>
    /// Проверка нового пациента на дубликаты
    /// </summary>
    [CodeRule]
    public class PacientValueRule : RuleBase<Pacient>
    {
        public PacientValueRule() : base("PacientValueRule", DefaultContexts.Save) { }
        public PacientValueRule(IRuleBaseProperties properties) : base(properties) { }

        protected override bool IsValidInternal(Pacient target, out string errorMessageTemplate)
        {
            errorMessageTemplate = "";
            var createdBy = SecuritySystem.CurrentUser as Doctor;
            var ambulanceId = "203001";
            if (createdBy.Otdelenie!=null && !createdBy.Otdelenie.Id.Equals(ambulanceId))
            {
                var objSpace = this.targetObjectSpace;

                var criterias = new List<CriteriaOperator>();

                criterias.Add(CriteriaOperator.Parse("Oid!=?", target.Oid));

                if (!string.IsNullOrEmpty(target.LastName))
                {
                    criterias.Add(CriteriaOperator.Parse("Lower(LastName)=?", target.LastName.ToLower()));
                }
                if (!string.IsNullOrEmpty(target.FirstName))
                {
                    criterias.Add(CriteriaOperator.Parse("Lower(FirstName)=?", target.FirstName.ToLower()));
                }
                if (!string.IsNullOrEmpty(target.MiddleName))
                {
                    criterias.Add(CriteriaOperator.Parse("Lower(MiddleName)=?", target.MiddleName.ToLower()));
                }
                if (target.Birthdate != null)
                {
                    criterias.Add(CriteriaOperator.Parse("Birthdate=?", target.Birthdate));
                }

                if (criterias.Count > 1)
                {
                    // Проверяем есть ли пациент с такими же ФИО и днем рождения
                    var dups = objSpace.GetObjects<Pacient>(CriteriaOperator.And(criterias));

                    if (dups.Count > 0)
                    {
                        // проверяем по СНИЛС
                        foreach (var pacient in dups)
                        {
                            if (pacient.SNILS.Equals(target.SNILS))
                            {
                                errorMessageTemplate = "Пациент с таким же данными, датой рождения и СНИЛС существует.";
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            return string.IsNullOrEmpty(errorMessageTemplate);
        }
    }

    /// <summary>
    /// Предоставляет правила валидации для документа, удостоверяющего личность пациента
    /// </summary>
    [CodeRule]
    public class PacientDocumentValueRule : RuleBase<Document>
    {
        public PacientDocumentValueRule() : base("PacientDocumentValueRule", DefaultContexts.Save) { }
        public PacientDocumentValueRule(IRuleBaseProperties properties) : base(properties) { }

        protected override bool IsValidInternal(Document target, out string errorMessageTemplate)
        {
            var createdBy = SecuritySystem.CurrentUser as Doctor;
            var ambulanceId = "203001";
            errorMessageTemplate = "";
            if (createdBy.Otdelenie != null && !createdBy.Otdelenie.Id.Equals(ambulanceId))
            {
                if (target == null)
                {
                    errorMessageTemplate = "Документ должен быть задан";
                    return false;
                }
                if (target.Type == null)
                {
                    errorMessageTemplate = "Тип документа должен быть задан";
                    return false;
                }
                if (string.IsNullOrEmpty(target.Serial) || string.IsNullOrEmpty(target.Number))
                {
                    errorMessageTemplate = "Серия и номер документа не могут быть пустыми";
                    return false;
                }


                // если введенная серия не соответствует маске серии типа
                if (!Regex.IsMatch(target.Serial, target.Type.MaskSerial))
                {
                    errorMessageTemplate = string.Format("Формат серии документа неверен");
                }

                // если введенный номер не соответствует маске серии типа
                if (!Regex.IsMatch(target.Number, target.Type.MaskNumber))
                {
                    errorMessageTemplate = string.Format("Формат номера документа неверен");
                }
            }
            return string.IsNullOrEmpty(errorMessageTemplate);
        }
    }
    
    /// <summary>
    /// Предоставляет правила валидации для полиса ОМС
    /// </summary>
    [CodeRule]
    public class PacientPolisValueRule : RuleBase<Polis>
    {
        public PacientPolisValueRule() : base("PacientPolisValueRule", DefaultContexts.Save) { }
        public PacientPolisValueRule(IRuleBaseProperties properties) : base(properties) { }

        protected override bool IsValidInternal(Polis target, out string errorMessageTemplate)
        {
            errorMessageTemplate = "";
            var createdBy = SecuritySystem.CurrentUser as Doctor;
            var ambulanceId = "203001";
            if (createdBy.Otdelenie != null && !createdBy.Otdelenie.Id.Equals(ambulanceId))
            {
                if (target == null)
                {
                    errorMessageTemplate = "Полис должен быть задан";
                    return false;
                }
                if (target.Type == null)
                {
                    errorMessageTemplate = "Тип полиса должен быть задан";
                    return false;
                }
                if (string.IsNullOrEmpty(target.Number))
                {
                    errorMessageTemplate = string.Format("Номер полиса не может быть пустым");
                    return false;
                }
                if (string.IsNullOrEmpty(target.Serial) & target.Type.Code == "1")
                {
                    errorMessageTemplate = string.Format("Серия полиса этого типа не может быть пустой");
                    return false;
                }

                if ((target.Type.Code == "3") & !string.IsNullOrEmpty(target.Serial))
                {
                    errorMessageTemplate = string.Format("Серия полиса единого образца должна быть пустой");
                    return false;
                }

                // если введенная серия не соответствует маске серии типа
                if (!Regex.IsMatch(target.Serial, target.Type.MaskSerial))
                {
                    errorMessageTemplate = string.Format("Формат серии полиса неверен");
                    return false;
                }

                // если введенный номер не соответствует маске серии типа
                if (!Regex.IsMatch(target.Number, target.Type.MaskNumber))
                {
                    errorMessageTemplate = string.Format("Формат номера полиса неверен");
                    return false;
                }
            }
            return string.IsNullOrEmpty(errorMessageTemplate);
        }
    }
}