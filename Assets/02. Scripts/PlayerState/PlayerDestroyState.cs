using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDestroyState : PlayerBaseState
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
        StartAnimation(_stateMachine.Player.AnimationData.InteractParameterHash); //interact하자마자 파괴 해쉬
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_stateMachine.Player.AnimationData.InteractParameterHash);
    }

    //public override void Update()
    //{
    //    // exit 조건 설정
    //    float normalizedTime = GetNormalizedTime(_stateMachine.Player.Animator, "Interact");

    //    if (normalizedTime >= 3f)
    //    {
    //        if (target != null)
    //        {
    //            target.GetComponent<IInteractable>()?.Interact(_stateMachine.Player);
    //            if (target.CompareTag("Gather") == false)
    //            {
                    
    //            }
    //        }
    //        _stateMachine.ChangeState(_stateMachine.IdleState);
    //    }
    //}

    protected override void AddInputActionsCallbacks()
    {
        PlayerInput input = _stateMachine.Player.Input;
        base.AddInputActionsCallbacks();
        _stateMachine.Player.ToolSystem.OnUnEquip += OnItemEquiped;
        input.PlayerActions.Interact.started += OnInteractStarted;
    }

    protected override void RemoveInputActionsCallbacks()
    {
        PlayerInput input = _stateMachine.Player.Input;
        base.RemoveInputActionsCallbacks();
        _stateMachine.Player.ToolSystem.OnUnEquip -= OnItemEquiped;
        input.PlayerActions.Interact.started -= OnInteractStarted;
    }

    //제작대 경우 고려해서 따로 만들기(제작대를 interact하면 파괴, 제작이 동시에 되는 문제)
    protected override void OnInteractStarted(InputAction.CallbackContext context) //E키 눌렀을 때
    {
        // 파괴 상태 시 상호작용 재 입력 시

        //base.OnInteractStarted(context); //이러면 빌드 모드 중 인벤토리가 열림

        // 망치 들고 상호작용하면 아이템 파괴
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
        //_stateMachine.Player.Animator.SetBool(targetTag, true);
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
