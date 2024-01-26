using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDestroyState : PlayerGroundedState
{
    protected GameObject target;
    protected string targetTag;

    public PlayerDestroyState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = _groundData.RunSpeedModifier;
        base.Enter();
        Debug.Log("ㅁㄴㅇㄻㄴㄹㅇㅁㄴㄹ");
        StartAnimation(_stateMachine.Player.AnimationData.IdleParameterHash); //interact하자마자 파괴 해쉬
    }

    public override void Exit()
    {
        base.Exit();
        if (target != null)
        {
            _stateMachine.Player.Animator.SetBool(targetTag, false);
            target = null;
        }
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

        if (target != null)
        {
            target.GetComponent<IInteractable>()?.Interact(_stateMachine.Player);
            GameObject.Destroy(target);
        }

        //// exit 조건 설정
        //float normalizedTime = GetNormalizedTime(_stateMachine.Player.Animator, "Interact");

        //if (normalizedTime >= 3f)
        //{
        //    if (target != null)
        //    {
        //        target.GetComponent<IInteractable>()?.Interact(_stateMachine.Player);
        //        GameObject.Destroy(target);
        //    }
        //    //_stateMachine.ChangeState(_stateMachine.IdleState);
        //}
    }

    protected override void AddInputActionsCallbacks()
    {
        PlayerInput input = _stateMachine.Player.Input;
        base.AddInputActionsCallbacks();
        _stateMachine.Player.ToolSystem.OnUnEquip += OnItemEquiped;
        //input.PlayerActions.Interact.started += OnInteractStarted;
        input.PlayerActions.Interact.started += OnDestroyStarted;
    }

    protected override void RemoveInputActionsCallbacks()
    {
        PlayerInput input = _stateMachine.Player.Input;
        base.RemoveInputActionsCallbacks();
        _stateMachine.Player.ToolSystem.OnUnEquip -= OnItemEquiped;
        //input.PlayerActions.Interact.started -= OnInteractStarted;
        input.PlayerActions.Interact.started -= OnDestroyStarted;
    }

    public void OnDestroyStarted(InputAction.CallbackContext context)
    {
        Debug.Log("파괴 모드 on");

        ToolItemData hammer = _stateMachine.Player.EquippedItem.itemData as ToolItemData;

        var targets = Physics.OverlapSphere(_stateMachine.Player.transform.position, hammer.range, hammer.targetLayers);
        if (targets.Length == 0)
        {
            return;
        }

        target = targets[0].gameObject;
        targetTag = target.tag;
        Debug.Log($"target Name : {targetTag}");
        RotateOfTarget();
        _stateMachine.Player.Animator.SetBool(targetTag, true);

        return;
    }


    private void OnItemEquiped(QuickSlot quickSlot)
    {
        // 파괴 상태 나가기
        Debug.Log("Exit Destroy");
        _stateMachine.Player.Building.OnCancelBuildMode();
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
                _stateMachine.ChangeState(_stateMachine.IdleState);
            }
        }
    }

    private void RotateOfTarget()
    {
        var look = target.transform.position - _stateMachine.Player.transform.position;
        look.y = 0;

        var targetRotation = Quaternion.LookRotation(look);
        _stateMachine.Player.transform.rotation = targetRotation;
    }
}
