using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBuildState : PlayerBaseState
{
    public PlayerBuildState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        
    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0;
        base.Enter();
        _stateMachine.Player.Building.OnCreateBluePrintArchitecture();
        Managers.UI.ShowPopupUI<UIBuilding>();
    }

    public override void Exit()
    {
        base.Exit();
        var ui = Managers.UI.GetPopupUI<UIBuilding>();
        Managers.UI.ClosePopupUI(ui);
    }

    protected override void AddInputActionsCallbacks()
    {
        PlayerInput input = _stateMachine.Player.Input;
        base.AddInputActionsCallbacks();
        _stateMachine.Player.ToolSystem.OnUnEquip += OnItemEquiped;

        input.PlayerActions.Interact.started += OnInteractStarted;
        //input.PlayerActions.Interact.canceled += OnInteractCanceled;
        //input.PlayerActions.InstallArchitecture.started += OnInstallArchitectureStarted;
        input.PlayerActions.RotateArchitectureLeft.started += OnRotateArchitectureLeftStarted;
        input.PlayerActions.RotateArchitectureRight.started += OnRotateArchitectureRightStarted;
    }

    protected override void RemoveInputActionsCallbacks()
    {
        PlayerInput input = _stateMachine.Player.Input;
        base.RemoveInputActionsCallbacks();
        _stateMachine.Player.ToolSystem.OnUnEquip -= OnItemEquiped;

        input.PlayerActions.Interact.started -= OnInteractStarted;
        //input.PlayerActions.Interact.canceled -= OnInteractCanceled;
        //input.PlayerActions.InstallArchitecture.started -= OnInstallArchitectureStarted;
        input.PlayerActions.RotateArchitectureLeft.started -= OnRotateArchitectureLeftStarted;
        input.PlayerActions.RotateArchitectureRight.started -= OnRotateArchitectureRightStarted;
    }

    private void OnItemEquiped(QuickSlot quickSlot)
    {
        // 빌드 상태 나가기
        Debug.Log("Exit Build");
        _stateMachine.Player.Building.OnCancelBuildMode();
        if (quickSlot.itemSlot.itemData is ToolItemData tooldata)
        {
            if(tooldata.isTwoHandedTool)
            {
                _stateMachine.ChangeState(_stateMachine.TwoHandedToolIdleState);
            }
            else if(tooldata.isTwinTool)
            {
                _stateMachine.ChangeState(_stateMachine.TwinToolIdleState);
            }
            else 
            {
                _stateMachine.ChangeState(_stateMachine.IdleState);
            }
        }        
    }

    protected override void OnInteractStarted(InputAction.CallbackContext context) //E키 눌렀을 때
    {
        // 건축 상태 시 상호작용 재 입력 시

        base.OnInteractStarted(context);

        //_stateMachine.Player.Building.OnCreateBluePrintArchitecture();
        _stateMachine.Player.Building.OnInstallArchitecture();
        Debug.Log("건축 모드 on");
    }

    //protected override void OnInteractCanceled(InputAction.CallbackContext context)
    //{

    //    base.OnInteractCanceled(context);
    //    //_stateMachine.Player.Building.OnCancelBuildMode();
    //    Debug.Log("건축 모드 off");
    //}

    public override void Update()
    {
        
    }

    //private void OnInstallArchitectureStarted(InputAction.CallbackContext context)
    //{
    //    //_stateMachine.Player.Building.OnInstallArchitecture();
    //}

    private void OnRotateArchitectureLeftStarted(InputAction.CallbackContext context)
    {
        _stateMachine.Player.Building.OnRotateArchitectureLeft();
    }

    private void OnRotateArchitectureRightStarted(InputAction.CallbackContext context)
    {
        _stateMachine.Player.Building.OnRotateArchitectureRight();
    }


    // _stateMachine.MovementInput 활용!
    // basestate의 ReadMovementInput()
}
