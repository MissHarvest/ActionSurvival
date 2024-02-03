using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDieState : PlayerBaseState
{
    public PlayerDieState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0.0f;
        _stateMachine.Player.Animator.SetTrigger("Die");
        _stateMachine.Player.GetComponent<CharacterController>().enabled = false;
    }

    public override void Update()
    {
        
    }
}
