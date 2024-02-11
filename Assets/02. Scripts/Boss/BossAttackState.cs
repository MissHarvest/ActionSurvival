using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackState : BossBaseState
{
    public float _reach;
    // 데미지
    public float cooltime;
    public float weight;

    public BossAttackState(BossStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        _stateMachine.NextAttackState = null;
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();        
        CoroutineManagement.Instance.StartCoroutine(CoolTime());
        StopAnimation(_stateMachine.Boss.AnimationData.BattleParamterHash);
    }

    IEnumerator CoolTime()
    {
        yield return new WaitForSeconds(cooltime);
        _stateMachine.Skill.AddItem(this, weight);
    }
}
