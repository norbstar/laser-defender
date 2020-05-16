using UnityEngine;

public class AnimatorStateMonitor : StateMachineBehaviour
{
    public delegate void OnEnterState(AnimatorStateInfo stateInfo);
    public delegate void OnUpdateState(AnimatorStateInfo stateInfo);
    public delegate void OnExitState(AnimatorStateInfo stateInfo);

    public class Delegates
    {
        public OnEnterState OnEnterStateDelegate { get; set; }
        public OnUpdateState OnUpdateStateDelegate { get; set; }
        public OnExitState OnExitStateDelegate { get; set; }
    }

    private Delegates delegates;

    public void RegisterDelegates(Delegates delegates)
    {
        this.delegates = delegates;
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log($"OnStateEnter");

        delegates?.OnEnterStateDelegate?.Invoke(stateInfo);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log($"OnStateUpdate");

        delegates?.OnUpdateStateDelegate?.Invoke(stateInfo);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log($"OnStateExit");

        delegates?.OnExitStateDelegate?.Invoke(stateInfo);
    }
}