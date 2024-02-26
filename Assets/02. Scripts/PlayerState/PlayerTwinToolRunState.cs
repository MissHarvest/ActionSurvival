//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class PlayerTwinToolRunState : PlayerGroundedState
//{
//    public PlayerTwinToolRunState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
//    {

//    }
//    public override void Enter()
//    {
//        _stateMachine.MovementSpeedModifier = _groundData.RunSpeedModifier;

//        base.Enter();
//        StartAnimation(_stateMachine.Player.AnimationData.EquipTwinToolRunParameterHash);
//    }

//    public override void Exit()
//    {
//        base.Exit();
//        StopAnimation(_stateMachine.Player.AnimationData.EquipTwinToolRunParameterHash);
//    }

//    protected override void OnMovementCanceled(InputAction.CallbackContext context)
//    {
//        base.OnMovementCanceled(context);
//        _stateMachine.ChangeState(_stateMachine.TwinToolIdleState);
//    }
//}
