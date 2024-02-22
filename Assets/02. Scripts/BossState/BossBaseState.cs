using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
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
        //Debug.Log($"[Enter State] {GetType().ToString()}");
        _stateMachine.Boss.NavMeshAgent.speed = _stateMachine.MovementSpeed * _stateMachine.MovementSpeedModifier;
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
        // 범위 밖으로 나가면 원위치 //
        var v1 = _stateMachine.Boss.RespawnPoint - _stateMachine.Boss.transform.position;
        v1.y = 0;
        var distPointToBoss = v1.sqrMagnitude;
        var detect = _stateMachine.DetectionDist * _stateMachine.DetectionDistModifier;

        var v2 = _stateMachine.Boss.RespawnPoint - _stateMachine.Target.transform.position;
        v2.y = 0;
        var distPointToPlayer = v2.sqrMagnitude;

        if (distPointToBoss > detect * detect && distPointToPlayer > detect * detect)
        {
            _stateMachine.ChangeState(_stateMachine.InvincibilityState);
            return;
        }
    }

    protected void StartAnimation(int animationHash)
    {
        _stateMachine.Boss.Animator.SetBool(animationHash, true);
    }

    protected void StopAnimation(int animationHash)
    {
        _stateMachine.Boss.Animator.SetBool(animationHash, false);
    }

    protected void RotateToTarget()
    {
        if (CheckTargetInViewArea()) return;

        Transform transform = _stateMachine.Boss.transform;
        var look = _stateMachine.Target.transform.position - transform.position;
        look.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(look);
        int dir = Vector3.Cross(_stateMachine.Boss.transform.forward, look).y > 0 ? 1 : -1;

        transform.Rotate(new Vector3(0, Time.deltaTime * 90 * dir, 0));
        if (CheckTargetInViewArea())
        {
            transform.rotation = targetRotation;
        }
    }

    protected float GetDistToTarget()
    {
        var distance = _stateMachine.Target.transform.position - _stateMachine.Boss.transform.position;
        distance.y = 0;
        return distance.sqrMagnitude;
    }

    protected float GetAngleWithTarget()
    {
        var dir = _stateMachine.Target.transform.position - _stateMachine.Boss.transform.position;
        dir.y = 0;
        var targetRotation = Quaternion.LookRotation(dir);
        return Mathf.Abs(Quaternion.Angle(_stateMachine.Boss.transform.rotation, targetRotation));
    }

    protected bool CheckTargetInViewArea()
    {
        return GetAngleWithTarget() < 5.0f;
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

    protected IEnumerator Stay(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action.Invoke();
    }
}
