using UnityEngine;
using UnityEngine.InputSystem;

// 2024. 01. 24 Byun Jeongmin
public class PlayerBuildState : PlayerBaseState
{
    private UIBuilding _buildingUI;

    public PlayerBuildState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        
    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0;
        base.Enter();
        _stateMachine.Player.Building.OnCreateBluePrintArchitecture();
        _stateMachine.Player.Building.OnInstallArchitecture();
        _buildingUI = Managers.UI.ShowPopupUI<UIBuilding>();
    }

    public override void Exit()
    {
        base.Exit();
        Managers.UI.ClosePopupUI(_buildingUI);
    }

    protected override void AddInputActionsCallbacks()
    {
        PlayerInput input = _stateMachine.Player.Input;
        _stateMachine.Player.ToolSystem.OnUnEquip += OnItemEquiped;

        input.PlayerActions.Interact.started += OnInteractStarted;
        input.PlayerActions.RotateArchitectureLeft.started += OnRotateArchitectureLeftStarted;
        input.PlayerActions.RotateArchitectureRight.started += OnRotateArchitectureRightStarted;
    }

    protected override void RemoveInputActionsCallbacks()
    {
        PlayerInput input = _stateMachine.Player.Input;
        _stateMachine.Player.ToolSystem.OnUnEquip -= OnItemEquiped;

        input.PlayerActions.Interact.started -= OnInteractStarted;
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
        _stateMachine.Player.Building.OnInstallArchitecture();
        //Debug.Log("건축 모드 on");
    }

    // override 안하면 건축 시 조이스틱을 움직이면 플레이어 방향이 돌아간다
    public override void Update()
    {
        ////SetObjPositionWithJoystick 가져다가 쓰기 
        //if (Managers.Game.Player.Building.IsHold)
        //{
        //    Managers.Game.Player.Building.SetObjPosition(_stateMachine.MovementInput);
        //    //Managers.Game.Player.Building.SetObjPositionWithJoystick(_stateMachine.MovementInput);
        //}
    }

    public void OnRotateArchitectureLeftStarted(InputAction.CallbackContext context)
    {
        _stateMachine.Player.Building.OnRotateArchitectureLeft();
    }

    public void OnRotateArchitectureRightStarted(InputAction.CallbackContext context)
    {
        _stateMachine.Player.Building.OnRotateArchitectureRight();
    }

    // _stateMachine.MovementInput 활용!
    // basestate의 ReadMovementInput()
}
