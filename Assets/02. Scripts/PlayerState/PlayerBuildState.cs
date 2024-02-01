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
        Debug.Log("[Player State Enter Building]");
        _stateMachine.MovementSpeedModifier = 0;
        base.Enter();

        _stateMachine.Player.Building.CreateArchitecture();
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

        input.PlayerActions.Interact.started += OnInteractStarted;
        input.PlayerActions.QuickSlot.started += OnQuickUseStarted;
        input.PlayerActions.RotateArchitectureLeft.started += OnRotateArchitectureLeftStarted;
        input.PlayerActions.RotateArchitectureRight.started += OnRotateArchitectureRightStarted;
    }

    protected override void RemoveInputActionsCallbacks()
    {
        PlayerInput input = _stateMachine.Player.Input;

        input.PlayerActions.Interact.started -= OnInteractStarted;
        input.PlayerActions.QuickSlot.started -= OnQuickUseStarted;
        input.PlayerActions.RotateArchitectureLeft.started -= OnRotateArchitectureLeftStarted;
        input.PlayerActions.RotateArchitectureRight.started -= OnRotateArchitectureRightStarted;
    }

    protected override void OnInteractStarted(InputAction.CallbackContext context) //E키 눌렀을 때
    {
        if(_stateMachine.Player.Building.BuildArchitecture())
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
        }
    }

    protected override void OnQuickUseStarted(InputAction.CallbackContext context)
    {
        _stateMachine.Player.Building.CancelBuilding();
        _stateMachine.ChangeState(_stateMachine.IdleState);
    }

    // override 안하면 건축 시 조이스틱을 움직이면 플레이어 방향이 돌아간다
    public override void Update()
    {
        Managers.Game.Player.Building.SetObjPositionWithJoystick(_stateMachine.MovementInput);
    }

    public void OnRotateArchitectureLeftStarted(InputAction.CallbackContext context)
    {
        _stateMachine.Player.Building.HandleRotateArchitectureLeft();
    }

    public void OnRotateArchitectureRightStarted(InputAction.CallbackContext context)
    {
        _stateMachine.Player.Building.HandleRotateArchitectureRight();
    }
}
