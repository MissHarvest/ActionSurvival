using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(PlayerStateMachine playerStateMachine) :base(playerStateMachine)
    {

    }
    private ItemObjectData itemObjectData = new ItemObjectData();


    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0f;
        base.Enter();
        StartAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);

        var Two = _stateMachine.Player.ToolSystem.ItemObject.GetComponent<ItemObjectData>();
        Debug.Log(Two.name);

        if (Two.onEquipTwoHandedTool == true)
        {
            StartAnimation(_stateMachine.Player.AnimationData.EquipTwoHandedToolIdleParameterHash);
            Debug.Log("두 손 애니메이션 시작");
        }
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);
    }

    public override void Update()
    {
        base.Update();

        if (_stateMachine.MovementInput != Vector2.zero)
        {
            OnMove();
            return;
        }        
    }
}
