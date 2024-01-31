using UnityEngine;
using UnityEngine.InputSystem;

// 2024. 01. 24 Byun Jeongmin
public class PlayerBuildState : PlayerBaseState
{
    private BuildingSystem _buildingSystem;
    private UIBuilding _buildingUI;

    public PlayerBuildState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        _buildingSystem = Managers.Game.Player.Building;
        _buildingSystem.OnBuildRequested += OnBuildRequested; //basestate로 옮기기
    }

    public override void Enter()
    {
        Debug.Log("[Player State Enter Building]");
        _stateMachine.MovementSpeedModifier = 0;
        base.Enter();

        _stateMachine.Player.Building.CreateArchitecture(); //CreateArchitecture 내에서 buildingsystem 작동 확인 코드?
        _buildingUI = Managers.UI.ShowPopupUI<UIBuilding>();
    }

    public override void Exit()
    {
        base.Exit();
        Managers.UI.ClosePopupUI(_buildingUI);
        _buildingSystem.OnBuildRequested -= OnBuildRequested;
    }

    private void OnBuildRequested(int index)
    {
        _stateMachine.ChangeState(_stateMachine.BuildState);
    }

    protected override void AddInputActionsCallbacks()
    {
        PlayerInput input = _stateMachine.Player.Input;
        _stateMachine.Player.ToolSystem.OnUnEquip += OnItemEquiped;

        input.PlayerActions.Interact.started += OnInteractStarted;
        input.PlayerActions.QuickSlot.started += OnQuickUseStarted; //override
        input.PlayerActions.RotateArchitectureLeft.started += OnRotateArchitectureLeftStarted;
        input.PlayerActions.RotateArchitectureRight.started += OnRotateArchitectureRightStarted;
    }

    protected override void RemoveInputActionsCallbacks()
    {
        PlayerInput input = _stateMachine.Player.Input;
        _stateMachine.Player.ToolSystem.OnUnEquip -= OnItemEquiped;

        input.PlayerActions.Interact.started -= OnInteractStarted;
        input.PlayerActions.QuickSlot.started -= OnQuickUseStarted;
        input.PlayerActions.RotateArchitectureLeft.started -= OnRotateArchitectureLeftStarted;
        input.PlayerActions.RotateArchitectureRight.started -= OnRotateArchitectureRightStarted;
    }

    private void OnItemEquiped(QuickSlot quickSlot)
    {
        // 빌드 상태 나가기
        Debug.Log("Exit Build");

        _stateMachine.Player.Building.CancelBuilding();
        if (quickSlot.itemSlot.itemData is ToolItemData tooldata)
        {
            if (tooldata.isTwoHandedTool)
            {
                _stateMachine.ChangeState(_stateMachine.TwoHandedToolIdleState);
            }
            else if (tooldata.isTwinTool)
            {
                _stateMachine.ChangeState(_stateMachine.TwinToolIdleState);
            }
            else
            {
                Debug.Log("안녕");
                _stateMachine.ChangeState(_stateMachine.IdleState);
            }
        }
        else
        {
            Debug.Log("안녕하세요");
            _stateMachine.ChangeState(_stateMachine.IdleState);
        }
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
