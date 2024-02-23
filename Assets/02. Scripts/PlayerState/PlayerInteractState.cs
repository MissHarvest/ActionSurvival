using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractState : PlayerBaseState
{
    protected IInteractable _interactable;
    protected IDestructible _destructible;
    protected float _progressTime;
    protected string _targetTag;
    protected Vector3 _targetPos;

    public PlayerInteractState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }

    public override void Enter()
    {
        if (_interactable == null && _destructible == null)
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
            return;
        }

        _stateMachine.MovementSpeedModifier = 0;
        base.Enter();
        StartAnimation(_stateMachine.Player.AnimationData.InteractParameterHash);

        _stateMachine.Player.Animator.SetBool(_targetTag, true);
        RotateOfTarget();

        _progressTime = _interactable?.GetInteractTime() ?? _destructible?.GetDestructTime() ?? 0f;

        //var tool = _stateMachine.Player.EquippedItem.itemSlot.itemData as ToolItemData;

        //var targets = Physics.OverlapSphere(_stateMachine.Player.transform.position, tool.range, tool.targetLayers, QueryTriggerInteraction.Collide);
        //if (targets.Length == 0)
        //{
        //    _stateMachine.ChangeState(_stateMachine.MakeState);
        //    return;
        //}

        //if (targets[0].CompareTag(tool.targetTagName) || targets[0].CompareTag("Gather"))
        //{
        //    _target = targets[0].gameObject;            
        //    targetTag = _target.tag;
        //    _repeatCount = targetTag == "Gather" ? 1 : 3;

        //    RotateOfTarget();
        //    _stateMachine.Player.Animator.SetBool(targetTag, true);
        //    return;
        //}
    }

    public override void Exit()
    {
        base.Exit();
        //if(_target != null)
        //{
        //    _stateMachine.Player.Animator.SetBool(targetTag, false);
        //    _target = null;
        //}
        _stateMachine.Player.Animator.SetBool(_targetTag, false);
        _interactable = null;
        _destructible = null;
        _targetTag = string.Empty;

        StopAnimation(_stateMachine.Player.AnimationData.InteractParameterHash);
    }

    public override void Update()
    {
        // exit 조건 설정
        float normalizedTime = GetNormalizedTime(_stateMachine.Player.Animator, "Interact");

        if (normalizedTime >= _progressTime)
        {
            _interactable?.Interact(_stateMachine.Player);
            _destructible?.Destruct(_stateMachine.Player);
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
        var look = _targetPos - _stateMachine.Player.transform.position;
        look.y = 0;

        var targetRotation = Quaternion.LookRotation(look);
        _stateMachine.Player.transform.rotation = targetRotation;
    }

    public void SetTarget(IInteractable target, string tag, Vector3 position)
    {
        _interactable = target;
        _targetPos = position;
        _targetTag = tag;
    }

    public void SetTarget(IDestructible target, string tag, Vector3 position)
    {
        _destructible = target;
        _targetPos = position;
        _targetTag = tag;
    }
}