using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmEmptyState : FarmBaseState
{
    public FarmEmptyState(FarmStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        _stateMachine.Farm.ChangeObject(0);
        _stateMachine.Farm.State = 0;
        _stateMachine.Farm.gameObject.layer = 11;
    }

    // Interact > change State (Grow)
    public override void Interact(Player player)
    {
        _stateMachine.ChangeState(_stateMachine.GrowState);
    }
}
