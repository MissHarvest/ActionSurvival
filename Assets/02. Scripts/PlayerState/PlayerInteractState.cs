using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

public class PlayerInteractState : PlayerBaseState
{
    public PlayerInteractState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0;
        base.Enter();
        StartAnimation (_stateMachine.Player.AnimationData.InteractParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_stateMachine.Player.AnimationData.InteractParameterHash);
    }

    public override void Update()
    {
        // exit 조건 설정
        float normalizedTime = GetNormalizedTime(_stateMachine.Player.Animator, "Interact");
        if(normalizedTime >= 1f)
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
        }
    }
}
