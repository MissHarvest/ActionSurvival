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
        _buildingSystem = _stateMachine.Player.Building;
    }

    public virtual void Enter()
    {
        AddInputActionsCallbacks();
    }

    public virtual void Exit()
    {
        RemoveInputActionsCallbacks();
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
        if (Physics.Raycast(_stateMachine.Player.transform.position, Vector3.down, out hit, 0.1f, 1 | 1 << 12 | 1 << 15))
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
        input.PlayerActions.Recipe.started += OnRecipeShowAndHide;

        input.PlayerActions.Esc.started += PauseGame;
        _buildingSystem.OnBuildRequested += OnBuildRequested;

        _stateMachine.Player.ToolSystem.OnEquip += OnChangedEquipTool;
        _stateMachine.Player.ToolSystem.OnUnEquip += OnChangedEquipTool;
    }

    protected virtual void RemoveInputActionsCallbacks()
    {
        PlayerInput input = _stateMachine.Player.Input;
        input.PlayerActions.Move.canceled -= OnMovementCanceled;

        input.PlayerActions.Interact.started -= OnInteractStarted;
        input.PlayerActions.Interact.canceled -= OnInteractCanceled;
        input.PlayerActions.QuickSlot.started -= OnQuickUseStarted;
        input.PlayerActions.Inventory.started -= OnInventoryShowAndHide;
        input.PlayerActions.Recipe.started -= OnRecipeShowAndHide;

        input.PlayerActions.Esc.started -= PauseGame;
        _buildingSystem.OnBuildRequested -= OnBuildRequested;

        _stateMachine.Player.ToolSystem.OnEquip -= OnChangedEquipTool;
        _stateMachine.Player.ToolSystem.OnUnEquip -= OnChangedEquipTool;
    }

    protected virtual void OnChangedEquipTool(QuickSlot quickSlot)
    {
        WeaponItemData hand = quickSlot.itemSlot.itemData as WeaponItemData;

        if (hand != null)
        {
            _stateMachine.Player.Animator.SetBool(_stateMachine.Player.AnimationData.EquipTwoHandedToolParameterHash, hand.isTwoHandedTool && quickSlot.itemSlot.equipped);
            _stateMachine.Player.Animator.SetBool(_stateMachine.Player.AnimationData.EquipTwinToolParameterHash, hand.isTwinTool && quickSlot.itemSlot.equipped);
            _stateMachine.Player.Animator.SetFloat(_stateMachine.Player.AnimationData.BlendEquipDefaultToolParameterHash, hand.isTwoHandedTool && quickSlot.itemSlot.equipped || hand.isTwinTool && quickSlot.itemSlot.equipped ? 0f : 1f);
            _stateMachine.Player.Animator.SetFloat(_stateMachine.Player.AnimationData.BlendEquipTwoHandedToolParameterHash, hand.isTwoHandedTool && quickSlot.itemSlot.equipped ? 1f : 0f);
            _stateMachine.Player.Animator.SetFloat(_stateMachine.Player.AnimationData.BlendEquipTwinToolParameterHash, hand.isTwinTool && quickSlot.itemSlot.equipped ? 1f : 0f);
        }
        else
        {
            _stateMachine.Player.Animator.SetBool(_stateMachine.Player.AnimationData.EquipTwoHandedToolParameterHash, false);
            _stateMachine.Player.Animator.SetBool(_stateMachine.Player.AnimationData.EquipTwinToolParameterHash, false);
            _stateMachine.Player.Animator.SetFloat(_stateMachine.Player.AnimationData.BlendEquipDefaultToolParameterHash, 1f);
            _stateMachine.Player.Animator.SetFloat(_stateMachine.Player.AnimationData.BlendEquipTwoHandedToolParameterHash, 0f);
            _stateMachine.Player.Animator.SetFloat(_stateMachine.Player.AnimationData.BlendEquipTwinToolParameterHash, 0f);
        }
    }

    protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
    {

    }

    protected virtual void OnInteractStarted(InputAction.CallbackContext context)
    {
        if (_stateMachine.IsFalling) return;
        _stateMachine.InteractSystem.TryInteractSequence();
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

    private void OnRecipeShowAndHide(InputAction.CallbackContext context)
    {
        var ui = Managers.UI.GetPopupUI<UIRecipe>();

        if (ui.gameObject.activeSelf)
        {
            Managers.UI.ClosePopupUI(ui);
        }
        else
        {
            Managers.UI.ShowPopupUI<UIRecipe>();
        }
    }

    private void PauseGame(InputAction.CallbackContext context)
    {
        var count = Managers.UI.GetActivatedPopupCount();
        if(count > 0)
        {
            Managers.UI.ClosePopupUI();
        }
        else
        {
            Managers.UI.ShowPopupUI<UIPauseGame>(pause: true);
        }
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