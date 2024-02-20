using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractState : PlayerBaseState
{
    protected GameObject target;
    protected string targetTag;
    protected int _repeatCount;
    private float _defaultRange = 0.5f;
    private LayerMask _defaultLayer = 64;//2112;

    public PlayerInteractState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0;
        base.Enter();
        StartAnimation(_stateMachine.Player.AnimationData.InteractParameterHash);

        var tool = _stateMachine.Player.EquippedItem.itemSlot.itemData as ToolItemData;

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
            _repeatCount = targetTag == "Gather" ? 1 : 3;
            
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

        if (normalizedTime >= 1.0f * _repeatCount)
        {
            if (target != null)
            {
                target.GetComponent<IInteractable>()?.Interact(_stateMachine.Player);
                if (!target.CompareTag("Gather"))
                {
                    int curIndex = _stateMachine.Player.ToolSystem.EquippedTool.targetIndex;
                    _stateMachine.Player.Inventory.TrySubtractDurability(curIndex, 1.0f);
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
