using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackState : BossBaseState
{
    public float _reach;
    // 데미지
    public float cooltime;
    public float weight;
    public bool usable = true;

    public BossAttackState(BossStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        _stateMachine.NextAttackState = null;
        base.Enter();
        usable = false;
    }

    public override void Exit()
    {
        base.Exit();        
        CoroutineManagement.Instance.StartCoroutine(CoolTime());
        StopAnimation(_stateMachine.Boss.AnimationData.BattleParamterHash);
    }

    public virtual void Cancel()
    {

    }

    IEnumerator CoolTime()
    {
        yield return new WaitForSeconds(cooltime);
        usable = true;
    }
}
