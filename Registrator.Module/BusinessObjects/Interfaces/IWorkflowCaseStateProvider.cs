using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.StateMachine;

namespace Registrator.Module.BusinessObjects.Interfaces
{
    interface IWorkflowCaseStateProvider : IStateMachineProvider, IWorkflowCaseStatus
    {
    }
}
