using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackState : BossBaseState
{
    public float _reach;
    // 데미지
    public float cooltime;
    
    public BossAttackState(BossStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Exit()
    {
        base.Exit();
        CoroutineManagement.Instance.StartCoroutine(CoolTime());
    }

    IEnumerator CoolTime()
    {
        yield return new WaitForSeconds(cooltime);
        _stateMachine.Skill.Enqueue(this);
    }
}
