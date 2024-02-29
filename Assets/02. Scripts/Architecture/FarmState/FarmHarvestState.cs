using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmHarvestState : FarmBaseState
{
    public FarmHarvestState(FarmStateMachine stateMachine) : base(stateMachine)
    {

    }
    public override void Enter()
    {
        _stateMachine.Farm.ChangeObject(2);
        _stateMachine.Farm.State = 2;
        //_stateMachine.Farm.gameObject.layer = 6;
    }

    // Interact > change State (Grow)
    public override void Interact(Player player)
    {
        Debug.Log("Farm Interact");
        _stateMachine.Farm.looting.AddInventory(player.Inventory);
        _stateMachine.ChangeState(_stateMachine.EmptyState);
    }
}
