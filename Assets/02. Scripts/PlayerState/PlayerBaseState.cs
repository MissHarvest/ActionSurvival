using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBaseState : IState
{
    protected PlayerStateMachine _stateMachine;
    protected readonly PlayerGroundData _groundData;

    private BuildingSystem _buildingSystem;

    public PlayerBaseState(PlayerStateMachine playerStateMachine)
    {
        _stateMachine = playerStateMachine;
        _groundData = _stateMachine.Player.Data.GroundedData;

        _buildingSystem = Managers.Game.Player.Building;
    }

    public virtual void Enter()
    {
        AddInputActionsCallbacks();
        _buildingSystem.OnBuildRequested += OnBuildRequested;
    }

    public virtual void Exit()
    {
        RemoveInputActionsCallbacks();
        _buildingSystem.OnBuildRequested -= OnBuildRequested;
    }

    public virtual void Update()
    {
        Move();
    }

    public virtual void HandleInput()
    {
        ReadMovementInput();
    }

    public virtual void PhysicsUpdate()
    {

    }

    private void ReadMovementInput()
    {
        _stateMachine.MovementInput = _stateMachine.Player.Input.PlayerActions.Move.ReadValue<Vector2>().normalized;
    }

    private void Move()
    {
        Vector3 movementDirection = GetMovementDirection();
        Debug.Log($"[Move Direction] {movementDirection}"); // 0
        Rotate(movementDirection);

        Move(movementDirection);
    }

    private Vector3 GetMovementDirection()
    {
        Vector3 forward = _stateMachine.MainCameraTransform.forward;
        Vector3 right = _stateMachine.MainCameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        return forward * _stateMachine.MovementInput.y + right * _stateMachine.MovementInput.x;
    }

    private void Move(Vector3 movementDirection)
    {
        RaycastHit hit;
        Physics.Raycast(_stateMachine.Player.transform.position, Vector3.down, out hit, 10.0f, 1 | 1 << 12);

        movementDirection = Vector3.ProjectOnPlane(movementDirection, hit.normal).normalized;

        Debug.DrawRay(_stateMachine.Player.transform.position, movementDirection, Color.red);
        Debug.DrawRay(hit.point, hit.normal, Color.black);

        float movementSpeed = GetMovementSpeed();
        _stateMachine.Player.Controller.Move(
            ((movementDirection * movementSpeed) +
            _stateMachine.Player.ForceReceiver.Movement)
            * Time.deltaTime
            );
    }

    protected virtual void Rotate(Vector3 movementDirection)
    {
        if (movementDirection != Vector3.zero)
        {
            Transform playerTransform = _stateMachine.Player.transform;
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, _stateMachine.RotationDamping * Time.deltaTime);
        }
    }

    private float GetMovementSpeed()
    {
        float movementSpeed = _stateMachine.MovementSpeed * _stateMachine.MovementSpeedModifier;
        return movementSpeed;
    }

    protected void StartAnimation(int animationHash)
    {
        _stateMachine.Player.Animator.SetBool(animationHash, true);
    }

    protected void StopAnimation(int animationHash)
    {
        _stateMachine.Player.Animator.SetBool(animationHash, false);
    }

    protected virtual void AddInputActionsCallbacks()
    {
        PlayerInput input = _stateMachine.Player.Input;
        input.PlayerActions.Move.canceled += OnMovementCanceled;

        input.PlayerActions.Interact.started += OnInteractStarted;
        input.PlayerActions.Interact.canceled += OnInteractCanceled;
        input.PlayerActions.QuickSlot.started += OnQuickUseStarted;
        input.PlayerActions.Inventory.started += OnInventoryShowAndHide;

        input.PlayerActions.Esc.started += PauseGame;
    }

    protected virtual void RemoveInputActionsCallbacks()
    {
        PlayerInput input = _stateMachine.Player.Input;
        input.PlayerActions.Move.canceled -= OnMovementCanceled;

        input.PlayerActions.Interact.started -= OnInteractStarted;
        input.PlayerActions.Interact.canceled -= OnInteractCanceled;
        input.PlayerActions.QuickSlot.started -= OnQuickUseStarted;
        input.PlayerActions.Inventory.started -= OnInventoryShowAndHide;
        input.PlayerActions.Esc.started -= PauseGame;
    }

    protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
    {

    }

    protected virtual void OnInteractStarted(InputAction.CallbackContext context)
    {
        if (_stateMachine.Player.EquippedItem == null) return;
        if (_stateMachine.IsFalling) return;
        Debug.Log("Player Interact");

        var tool = _stateMachine.Player.EquippedItem.itemData as ToolItemData;

        if (tool.isArchitecture)
        {
            _stateMachine.ChangeState(_stateMachine.BuildState);
        }
        else if (tool.isWeapon)
        {
            _stateMachine.IsAttacking = true;
            _stateMachine.ChangeState(_stateMachine.ComboAttackState);
        }
        else if (tool.displayName == "망치")
        {
            _stateMachine.ChangeState(_stateMachine.DestroyState);
        }
        else
        {
            _stateMachine.ChangeState(_stateMachine.InteractState);
        }
    }

    protected virtual void OnInteractCanceled(InputAction.CallbackContext context)
    {
        _stateMachine.IsAttacking = false;
    }

    protected virtual void OnQuickUseStarted(InputAction.CallbackContext context)
    {
        _stateMachine.Player.QuickSlot.OnQuickUseInput((int)context.ReadValue<float>() - 1);
    }

    private void OnBuildRequested(int index)
    {
        //Debug.Log("빌드 모드 진입: " + index);
        _stateMachine.ChangeState(_stateMachine.BuildState);
    }

    protected void ForceMove()
    {
        _stateMachine.Player.Controller.Move(_stateMachine.Player.ForceReceiver.Movement * Time.deltaTime);
    }

    private void OnInventoryShowAndHide(InputAction.CallbackContext context)
    {
        // UICooking 등의 팝업이 활성화된 경우 모든 팝업을 닫은 후 Inventory 팝업 열기
        Managers.UI.CloseAllPopupUI();

        var ui = Managers.UI.GetPopupUI<UIInventory>();
        
        if (ui.gameObject.activeSelf)
        {
            Managers.UI.ClosePopupUI(ui);
        }
        else
        {
            Managers.UI.ShowPopupUI<UIInventory>();
        }
    }

    private void PauseGame(InputAction.CallbackContext context)
    {
        Managers.UI.ShowPopupUI<UIPauseGame>();
    }

    protected float GetNormalizedTime(Animator animator, string tag)
    {
        AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(0);

        if (animator.IsInTransition(0) && nextInfo.IsTag(tag))
        {
            return nextInfo.normalizedTime;
        }
        else if (!animator.IsInTransition(0) && currentInfo.IsTag(tag))
        {
            return currentInfo.normalizedTime;
        }
        else
        {
            return 0f;
        }
    }
}
