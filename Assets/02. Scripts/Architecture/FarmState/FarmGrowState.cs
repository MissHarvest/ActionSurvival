using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmGrowState : FarmBaseState
{
    protected DayCycle _manager;

    public FarmGrowState(FarmStateMachine stateMachine) : base(stateMachine)
    {
        _manager = Managers.Game.DayCycle;
    }

    public override void Enter()
    {
        _manager.OnTimeUpdated += TimeLapse;
        _stateMachine.Farm.ChangeObject(1);
        _stateMachine.Farm.State = 1;
        _stateMachine.Farm.RemainingTime = _stateMachine.Farm.RemainingTime <= 0 ? _stateMachine.Farm.MaxTime : _stateMachine.Farm.RemainingTime;
    }

    public override void Exit()
    {
        if (_manager != null)
            _manager.OnTimeUpdated -= TimeLapse;
    }

    public override void TimeLapse()
    {
        _stateMachine.Farm.RemainingTime--;
        if (_stateMachine.Farm.RemainingTime <= 0)
        {
            _stateMachine.ChangeState(_stateMachine.HarvestState);
        }
    }
}