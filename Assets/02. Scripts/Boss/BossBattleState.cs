using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattleState : BossBaseState
{
    private Vector3 _look;
    private BossAttackState _nextAttackState;

    public BossBattleState(BossStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = _stateMachine.Boss.Data.MovementData.WalkSpeedModifier;
        base.Enter();
        
        _look = _stateMachine.Target.transform.position - _stateMachine.Boss.transform.position;
        _look.y = 0;
        StartAnimation(_stateMachine.Boss.AnimationData.BattleParamterHash);
        CoroutineManagement.Instance.StartCoroutine(GetNextSkill());
    }

    public override void Exit() 
    {
        base.Exit();
        _nextAttackState = null;
    }

    public override void Update()
    {
        base.Update();
        // Rotate
        if (Rotate(_stateMachine.Target.transform.position - _stateMachine.Boss.transform.position) == false) return;

        if (_nextAttackState == null) return;

        // 다음 공격 상태의 리치 만큼 플레이어가 가깝게 있는가
        var sqrDist = GetDistToTarget();
        if (sqrDist >= _nextAttackState._reach * _nextAttackState._reach)
        {
            _stateMachine.Boss.NavMeshAgent.SetDestination(_stateMachine.Target.transform.position);
            return;
        }
        
        // Rotate 끝나면 공격 상태
        _stateMachine.ChangeState(_nextAttackState);
    }
    IEnumerator GetNextSkill()
    {
        bool loop = true;
        while(loop)
        {
            yield return null;
            loop = _stateMachine.Skill.Count <= 0;
        }
        _nextAttackState = _stateMachine.GetUsableSkill();
    }
}
