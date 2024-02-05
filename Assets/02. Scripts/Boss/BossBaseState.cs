using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBaseState : IState
{
    protected BossStateMachine _stateMachine;

    public BossBaseState(BossStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public virtual void Enter()
    {
        
    }

    public virtual void Exit()
    {
        
    }

    public virtual void HandleInput()
    {
        
    }

    public virtual void PhysicsUpdate()
    {
        
    }

    public virtual void Update()
    {
        
    }

    protected void StartAnimation(int animationHash)
    {
        _stateMachine.Boss.Animator.SetBool(animationHash, true);
    }

    protected void StopAnimation(int animationHash)
    {
        _stateMachine.Boss.Animator.SetBool(animationHash, false);
    }

    protected float GetNormalizedTime(Animator animator, string tag)
    {
        AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(0);

        if (animator.IsInTransition(0) && nextInfo.IsTag(tag))
        {
            return nextInfo.normalizedTime;
        }
        else if (!animator.IsInTransition(0) && currentInfo.IsTag(tag))
        {
            return currentInfo.normalizedTime;
        }
        else
        {
            return 0f;
        }
    }
}
