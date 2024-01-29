using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmGrowState : FarmBaseState
{
    public FarmGrowState(FarmStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        _stateMachine.Farm.ChangeObject(1);
        _stateMachine.Farm.State = 1;
        _stateMachine.Farm.RemainingTime = _stateMachine.Farm.RemainingTime <= 0.0f ? _stateMachine.Farm.MaxTime : _stateMachine.Farm.RemainingTime;
    }


    public override void Update()
    {
        _stateMachine.Farm.RemainingTime -= Time.deltaTime;
        if(_stateMachine.Farm.RemainingTime <= 0)
        {
            _stateMachine.ChangeState(_stateMachine.HarvestState);
        }
    }
}
