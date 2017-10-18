using DevExpress.ExpressApp;
using DevExpress.ExpressApp.StateMachine;
using DevExpress.ExpressApp.StateMachine.NonPersistent;
using Registrator.Module.BusinessObjects.Interfaces;

namespace Registrator.Module.BusinessObjects.StateMachines
{
    public class WorkflowServiceStatusStateMaching : StateMachine<IWorkflowServiceStatus>
    {
        private IState _startState;
        public WorkflowServiceStatusStateMaching(IObjectSpace objectSpace)
            : base(objectSpace)
        {
            // состояния
            _startState = new State(this, WorkflowServiceStatus.Created);
            IState closedState = new State(this, WorkflowServiceStatus.Closed);
            IState inProgressState = new State(this, WorkflowServiceStatus.InProgress);
            IState rejectedState = new State(this, WorkflowServiceStatus.Rejected);
            IState refusedState = new State(this, WorkflowServiceStatus.Refused);
            IState waitingForPayState = new State(this, WorkflowServiceStatus.WaitingForPayment);
            IState paidState = new State(this, WorkflowServiceStatus.Paid);
            IState partlyPaidState = new State(this, WorkflowServiceStatus.PartlyPaid);
            IState noPayState = new State(this, WorkflowServiceStatus.PaymentRejected);

            // transitions
            _startState.Transitions.Add(new Transition(inProgressState));
            _startState.Transitions.Add(new Transition(closedState));

            inProgressState.Transitions.Add(new Transition(closedState));
            inProgressState.Transitions.Add(new Transition(rejectedState));
            inProgressState.Transitions.Add(new Transition(refusedState));

            closedState.Transitions.Add(new Transition(waitingForPayState));

            waitingForPayState.Transitions.Add(new Transition(paidState));
            waitingForPayState.Transitions.Add(new Transition(partlyPaidState));
            waitingForPayState.Transitions.Add(new Transition(noPayState));

            // adding states
            States.Add(_startState);
            States.Add(inProgressState);
            States.Add(rejectedState);
            States.Add(refusedState);
            States.Add(closedState);
            States.Add(waitingForPayState);
            States.Add(paidState);
            States.Add(partlyPaidState);
            States.Add(noPayState);

            // appearance
            var waitInsAppearance = new StateAppearance(_startState);
            waitInsAppearance.TargetItems = "Status";
            waitInsAppearance.FontColor = System.Drawing.Color.DarkOrange;
        }

        public override string Name
        {
            get { return "Current Status"; }
        }

        public override string StatePropertyName
        {
            get { return "WorkflowServiceStatus"; }
        }


        public override IState StartState
        {
            get { return _startState; }
        }
    }
}
