using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerBaseState : IState
{
    protected PlayerStateMachine _stateMachine;
    protected readonly PlayerGroundData _groundData;
    
    public PlayerBaseState(PlayerStateMachine playerStateMachine)
    {
        _stateMachine  = playerStateMachine;
        _groundData = _stateMachine.Player.Data.GroundedData;
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
        _stateMachine.MovementInput = _stateMachine.Player.Input.PlayerActions.Move.ReadValue<Vector2>();
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
        //Vector3 forward = _stateMachine.Player.transform.forward;
        Vector3 right = _stateMachine.MainCameraTransform.right;
        //Vector3 right = _stateMachine.Player.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        return forward * _stateMachine.MovementInput.y + right * _stateMachine.MovementInput.x;
    }

    private void Move(Vector3 movementDirection)
    {
        float movementSpeed = GetMovementSpeed();
        _stateMachine.Player.Controller.Move(
            ((movementDirection * movementSpeed) +
            _stateMachine.Player.ForceReceiver.Movement)
            * Time.deltaTime
            );
    }
    
    private void Rotate(Vector3 movementDirection)
    {
        if(movementDirection != Vector3.zero)
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
        input.PlayerActions.Run.started += OnRunStarted;
        input.PlayerActions.Run.canceled += OnRunCanceled;
        //input.PlayerActions.Jump.started += OnJumpStarted;
        input.PlayerActions.Attack.performed += OnAttackPerformed;
        input.PlayerActions.Attack.canceled += OnAttackCanceled;
        
        input.PlayerActions.Interact.started += OnInteractStarted;
    }

    protected virtual void RemoveInputActionsCallbacks()
    {
        PlayerInput input = _stateMachine.Player.Input;
        input.PlayerActions.Move.canceled -= OnMovementCanceled;
        input.PlayerActions.Run.started -= OnRunStarted;
        input.PlayerActions.Run.canceled -= OnRunCanceled;
        //input.PlayerActions.Jump.started -= OnJumpStarted;
        input.PlayerActions.Attack.performed -= OnAttackPerformed;
        input.PlayerActions.Attack.canceled -= OnAttackCanceled;

        input.PlayerActions.Interact.started -= OnInteractStarted;
    }

    protected virtual void OnRunStarted(InputAction.CallbackContext context)
    {
       
    }

    protected virtual void OnRunCanceled(InputAction.CallbackContext context)
    {

    }

    protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
    {
        
    }

    protected virtual void OnJumpStarted(InputAction.CallbackContext context)
    {

    }

    protected virtual void OnInteractStarted(InputAction.CallbackContext context)
    {
        if (_stateMachine.Player.EquippedItem == null) return;

        var tool = _stateMachine.Player.EquippedItem.itemData as ToolItemData;
        if (tool.isWeapon)
        {
            // _stateMachine.ChangeState(_stateMachine.AttackState);
        }
        else
        {
            _stateMachine.ChangeState(_stateMachine.InteractState);
        }        
        Debug.Log("Player Interact");
    }

    protected void ForceMove()
    {
        _stateMachine.Player.Controller.Move(_stateMachine.Player.ForceReceiver.Movement * Time.deltaTime);
    }

    protected void OnAttackPerformed(InputAction.CallbackContext context)
    {
        _stateMachine.IsAttacking = true;
    }

    protected void OnAttackCanceled(InputAction.CallbackContext context)
    {
        _stateMachine.IsAttacking = false;
    }

    protected float GetNormalizedTime(Animator animator, string tag)
    {
        AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(0);

        if(animator.IsInTransition(0) && nextInfo.IsTag(tag))
        {
            return nextInfo.normalizedTime;
        }
        else if(!animator.IsInTransition(0) && currentInfo.IsTag(tag))
        {
            return currentInfo.normalizedTime;
        }
        else
        {
            return 0f;
        }
    }
}
