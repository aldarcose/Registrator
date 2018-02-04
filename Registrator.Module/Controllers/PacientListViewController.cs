using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using Registrator.Module.BusinessObjects;
using Registrator.Module.BusinessObjects.Abstract;
using System;
using System.Linq;

namespace Registrator.Module.Controllers
{
    /// <summary>
    /// Котроллер фильтрует пациентов, которые записаны к текущему врачу
    /// </summary>
    public partial class PacientListViewController : ObjectViewController<ListView, Pacient>
    {
        private const string FILTERKEY = "PACIENTFILTER";

        protected override void OnActivated()
        {
            base.OnActivated();

            Doctor curDoctor = (Doctor)SecuritySystem.CurrentUser;
            bool isAdmin = curDoctor.DoctorRoles.Any(t => t.IsAdministrative);
            if (!isAdmin)
            {
                CriteriaOperator criteria = 
                    new JoinOperand(typeof(CommonCase).FullName,
                        CommonCase.Fields.DateIn >= DateTime.Today &
                        CommonCase.Fields.Doctor.Oid == curDoctor.Oid &
                        CommonCase.Fields.Pacient.Oid == new OperandProperty("^.Oid")) |
                    new JoinOperand(typeof(DoctorEvent).FullName,
                        DoctorEvent.Fields.Pacient.Oid == new OperandProperty("^.Oid") &
                        DoctorEvent.Fields.AssignedTo.Oid == curDoctor.Oid &
                        DoctorEvent.Fields.StartOn >= DateTime.Today &
                        DoctorEvent.Fields.StartOn < DateTime.Today.AddDays(1));
                
                ((ListView)View).CollectionSource.BeginUpdateCriteria();
                ((ListView)View).CollectionSource.Criteria.Clear();
                ((ListView)View).CollectionSource.Criteria[FILTERKEY] = criteria;
                ((ListView)View).CollectionSource.EndUpdateCriteria();
            }
        }
    }
}
