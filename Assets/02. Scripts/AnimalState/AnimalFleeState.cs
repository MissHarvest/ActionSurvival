using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFleeState : AnimalBaseState
{
    public AnimalFleeState(AnimalStateMachine stateMachine) : base(stateMachine)
    {

    }
    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = _stateMachine.Animal.Data.MovementData.FleeSpeedModifier;
        base.Enter();
        StartAnimation(_stateMachine.Animal.AnimationData.RunParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_stateMachine.Animal.AnimationData.RunParameterHash);
    }

    private void Flee()
    {
        // 플레이어 위치 찾기

        // 나의 위치 - 플레이어 위치 = 도망갈 방향

        // 도망 거리

        // Set Destination

        // 
    }
}
