using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMakeState : PlayerBaseState
{
    protected GameObject target;
    public PlayerMakeState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        var targets = Physics.OverlapSphere(_stateMachine.Player.transform.position, 1.0f, 2048);
        if (targets.Length == 0)
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
            return;
        }
        target = targets[0].gameObject;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        target.GetComponent<IInteractable>()?.Interact(_stateMachine.Player);
        _stateMachine.ChangeState(_stateMachine.IdleState);
    }
}
