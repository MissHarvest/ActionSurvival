// 작성 날짜 : 2024. 01. 12
// 작성자 : Park Jun Uk

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterBaseState : IState
{
    protected MonsterStateMachine _stateMachine;

    public MonsterBaseState(MonsterStateMachine monsterStateMachine)
    {
        _stateMachine = monsterStateMachine;
    }

    public virtual void Enter()
    {
        _stateMachine.Monster.NavMeshAgent.speed = _stateMachine.MovementSpeed * _stateMachine.MovementSpeedModifier;
    }

    public virtual void Exit()
    {
        
    }

    public virtual void Update()
    {
        if(TryDetectPlayer())
        {
            _stateMachine.ChangeState(_stateMachine.ChaseState);
        }
    }

    public virtual void HandleInput()
    {

    }

    public virtual void PhysicsUpdate()
    {

    }

    protected float GetDistanceBySqr(Vector3 target)
    {
        var offset = _stateMachine.Monster.transform.position - target;
        var sqrLength = offset.sqrMagnitude;
        return sqrLength;
    }

    protected bool TryDetectPlayer()
    {
        var sqrLength = GetDistanceBySqr(Managers.Game.Player.transform.position);
        var dist = _stateMachine.DetectionDist * _stateMachine.DetectionDistModifier;
        return sqrLength < dist * dist;
    }

    protected void StartAnimation(int animationHash)
    {
        _stateMachine.Monster.Animator.SetBool(animationHash, true);
    }

    protected void StopAnimation(int animationHash)
    {
        _stateMachine.Monster.Animator.SetBool(animationHash, false);
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
