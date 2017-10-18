using DevExpress.ExpressApp;
using DevExpress.ExpressApp.StateMachine;
using DevExpress.ExpressApp.StateMachine.NonPersistent;
using Registrator.Module.BusinessObjects.Interfaces;

namespace Registrator.Module.BusinessObjects.StateMachines
{
    public class WorkflowCaseStatusStateMachine : StateMachine<IWorkflowCaseStatus>
    {
        private IState _startState;
        public WorkflowCaseStatusStateMachine(IObjectSpace objectSpace)
            : base(objectSpace)
        {
            // состояния
            _startState = new State(this, WorkflowCaseStatus.Created);
            IState closedState = new State(this, WorkflowCaseStatus.Closed);
            IState inProgressState = new State(this, WorkflowCaseStatus.InProgress);
            IState rejectedState = new State(this, WorkflowCaseStatus.Rejected);
            IState refusedState = new State(this, WorkflowCaseStatus.Refused);
            IState waitingForPayState = new State(this, WorkflowCaseStatus.WaitingForPayment);
            IState paidState = new State(this, WorkflowCaseStatus.Paid);
            IState partlyPaidState = new State(this, WorkflowCaseStatus.PartlyPaid);
            IState noPayState = new State(this, WorkflowCaseStatus.PaymentRejected);

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
            get { return "WorkflowCaseStatus"; }}


        public override IState StartState
        {
            get { return _startState; }
        }
    }
}
