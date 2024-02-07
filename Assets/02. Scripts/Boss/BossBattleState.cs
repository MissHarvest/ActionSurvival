using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattleState : BossBaseState
{
    private Vector3 _look;
    private int _testNumber = 0;
    private BossAttackState _nextAttackState;

    public BossBattleState(BossStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = _stateMachine.Boss.Data.MovementData.WalkSpeedModifier;
        base.Enter();
        // 어떤 공격을 할 것인지 확인하는 로직
        _look = _stateMachine.Target.transform.position - _stateMachine.Boss.transform.position;
        _look.y = 0;
        StartAnimation(_stateMachine.Boss.AnimationData.BattleParamterHash);
        ChangeTestState();
    }

    public override void Exit() 
    {
        base.Exit();
        ++_testNumber;
        
    }

    public override void Update()
    {
        // Rotate
        if (Rotate(_stateMachine.Target.transform.position - _stateMachine.Boss.transform.position) == false) return;
        
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
    private void ChangeTestState()
    {
        Debug.Log($"{_testNumber}");
        if (_testNumber < 2)
            _nextAttackState = _stateMachine.RushSate;
        else if (_testNumber < 3)
            _nextAttackState = _stateMachine.ScreamState;
        else if (_testNumber < 4)
            _nextAttackState = _stateMachine.BreathState;
        else if (_testNumber < 5)
            _nextAttackState = _stateMachine.BiteState;
        else if (_testNumber < 7)
            _nextAttackState = _stateMachine.StabState;
        else
            _nextAttackState = _stateMachine.MeteorState;
    }
}
