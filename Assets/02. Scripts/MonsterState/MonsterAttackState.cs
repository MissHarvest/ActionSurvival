using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackState : MonsterBaseState
{
    public MonsterAttackState(MonsterStateMachine monsterStateMachine) : base(monsterStateMachine)
    {

    }

    public override void Enter()
    {
        Debug.Log("Monster State Changed to [ Attack ]");
        _stateMachine.MovementSpeedModifier = 0.0f;
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        // base.Update();

        var sqrLength = GetDistanceBySqr(Managers.Game.Player.transform.position);
        var reach = _stateMachine.Monster.Data.AttackData.AttackalbeDistance;

        if (sqrLength >= reach * reach)
        {
            _stateMachine.ChangeState(_stateMachine.ChaseState);
        }
    }
}
