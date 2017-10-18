using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule.Notifications;
using DevExpress.Office.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    [DefaultClassOptions]
    [XafDisplayName("Справочник Обязательные услуги для диспансеризации")]
    public class DispsServiceList : BaseObject
    {
        // возраст в годах начиная с которого месяц не учитывается
        private const int NoMonthAge = 3;
        public DispsServiceList(Session session) : base(session) { }

        [XafDisplayName("Тип диспансеризации")]
        [ImmediatePostData(true)]
        public DispType Type { get; set; }

        [XafDisplayName("Возраст, для которого указываются услуги")]
        [Appearance("AgePacient", Criteria = "[Type]!=1 AND [Type]!=3 AND [Type]!=5 AND [Type]!=6 AND [Type]!=8", Visibility = ViewItemVisibility.Hide, Context = "DetailView")]
        [ImmediatePostData(true)]
        public int Age { get; set; }

        [Appearance("NewBornPacient", Criteria = "[Type]!=3 AND [Type]!=5 OR [Age]>3", Visibility = ViewItemVisibility.Hide, Context = "DetailView")]

        [XafDisplayName("Месяц")]
        public int Month { get; set; }

        [XafDisplayName("Пол")]
        public Gender? Gender { get; set; }

        [Appearance("ChildPacient", Criteria = "[Type]!=4 AND [Type]!=5", Visibility = ViewItemVisibility.Hide, Context = "DetailView")]
        [XafDisplayName("Образовательное учреждение")]
        public EducationInstituteType EducationInstitute { get; set; }

        [Association("DispServiceList-ServiceWithInfo"), DevExpress.Xpo.Aggregated]
        //[DataSourceCriteriaProperty("GenderCriteria")]
        [XafDisplayName("Услуги, которые должны быть оказаны в диспансеризации")]
        public XPCollection<ServiceWithInfo> Services
        {
            get { return GetCollection<ServiceWithInfo>("Services"); }
        }
        private CriteriaOperator GenderCriteria
        {
            get
            {
                if (Gender != null)
                    CriteriaOperator.Parse("IsNull(ForGender) Or ForGender=?", Gender.Value);
                return CriteriaOperator.Parse("True");
            }
        }


        // возвращает ТРУ если пациент удовлетворяет условиям, заданных для типа диспансеризации
        public bool CheckPacient(Pacient pacient, DateTime? checkDateFrom = null)
        {

            // проверяем пол
            if (Gender.HasValue)
            {
                if (Gender.Value != pacient.Gender)
                    return false;
            }

            DateTime now = DateTime.Now;

            if (checkDateFrom.HasValue)
                 now = checkDateFrom.Value;
            
            DateTime bd = pacient.Birthdate.Value;
            // смотрим по году исполнения
            int age = pacient.GetAge(now);
            if (age >= NoMonthAge)
            {
                if (now.Month < bd.Month)
                    age ++;
                if (now.Month == bd.Month)
                    if (now.Day < bd.Day)
                        age++;
            }


            bool checkMonth = age < NoMonthAge;
            int month = 0;
            if (checkMonth)
            {
                // от 0 до 1 - 1 месячный, от 1 до 2 - 2 месячный
                month = pacient.GetMonthWithNoAge(now);
            }

            switch (Type)
            {
                case DispType.ProfOsmotrAdult:
                    break;
                case DispType.DOGVN1:
                    // проверяем возраст
                    if (age != Age)
                        return false;
                    break;
                case DispType.DOGVN2:
                    break;
                case DispType.ProfOsmotrChild:
                    // проверяем возраст
                    if (age != Age)
                        return false;
                    else
                    {
                        // проверяем месяц (проверка по месяцам актуальна когда возраст меньше трех)
                        if (checkMonth)
                        {
                            if (Age == 0 && month != Month)
                                return false;
                            if (Age == 1)
                            {
                                // по месяцам 0, 3, 6, 9:
                                // например, для пациента возраста 1 год и 2 месяца будет применяться шаблон 1 год и 0 месяц
                                // 1 год и 8 месяц - 1 год и 6 месяцев
                                if (month < Month || Month + 3 < month)
                                    return false;
                            }

                            if (Age == 2)
                            {
                                // по месяцам 0, 6
                                if (month < Month || Month + 6 < month)
                                    return false;
                            }
                        }
                    }
                    break;
                case DispType.PreProfOsmotrChild:
                    break;
                case DispType.PeriodProfOsmotrChild:
                    // проверяем возраст
                    if (age != Age)
                        return false;
                    else
                    {
                        // проверяем месяц
                        if (checkMonth)
                            if (month != Month)
                                return false;
                    }
                    break;
                case DispType.DispStacionarChildOrphan1:
                    // проверяем возраст
                    if (age != Age)
                        return false;
                    break;
                case DispType.DispStacionarChildOrphan12:
                    break;
                case DispType.DispChildOrphan1:
                    // проверяем возраст
                    if (age != Age)
                        return false;
                    break;
                case DispType.DispChildOrphan12:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return true;
        }
    }
}
