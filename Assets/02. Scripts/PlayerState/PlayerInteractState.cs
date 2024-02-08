using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractState : PlayerBaseState
{
    protected GameObject target;
    protected string targetTag;

    public PlayerInteractState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0;
        base.Enter();
        StartAnimation(_stateMachine.Player.AnimationData.InteractParameterHash);

        ToolItemData tool = _stateMachine.Player.EquippedItem.itemData as ToolItemData;

        var targets = Physics.OverlapSphere(_stateMachine.Player.transform.position, tool.range, tool.targetLayers, QueryTriggerInteraction.Collide);
        if (targets.Length == 0)
        {
            _stateMachine.ChangeState(_stateMachine.MakeState);
            return;
        }

        if (targets[0].CompareTag(tool.targetTagName) || targets[0].CompareTag("Gather"))
        {
            target = targets[0].gameObject;
            targetTag = target.tag;
            Debug.Log($"target Name : {targetTag}");
            RotateOfTarget();
            _stateMachine.Player.Animator.SetBool(targetTag, true);
            return;
        }
        _stateMachine.ChangeState(_stateMachine.IdleState);
    }

    public override void Exit()
    {
        base.Exit();
        if(target != null)
        {
            _stateMachine.Player.Animator.SetBool(targetTag, false);
            target = null;
        }

        StopAnimation(_stateMachine.Player.AnimationData.InteractParameterHash);
    }

    public override void Update()
    {
        // exit 조건 설정
        float normalizedTime = GetNormalizedTime(_stateMachine.Player.Animator, "Interact");

        if (normalizedTime >= 3f)
        {
            if (target != null)
            {
                target.GetComponent<IInteractable>()?.Interact(_stateMachine.Player);
                if(target.CompareTag("Gather") ==  false )
                {
                    int curIndex = Managers.Game.Player.QuickSlot.IndexInUse;
                    int inventoryIndex = Managers.Game.Player.QuickSlot.slots[curIndex].targetIndex;
                    Managers.Game.Player.Inventory.UseToolItemByIndex(inventoryIndex, 1f);
                }                
            }
            _stateMachine.ChangeState(_stateMachine.IdleState);
        }
    }

    protected override void OnInteractStarted(InputAction.CallbackContext context)
    {

    }

    protected override void OnQuickUseStarted(InputAction.CallbackContext context)
    {

    }

    private void RotateOfTarget()
    {
        var look = target.transform.position - _stateMachine.Player.transform.position;
        look.y = 0;

        var targetRotation = Quaternion.LookRotation(look);
        _stateMachine.Player.transform.rotation = targetRotation;
    }
}
