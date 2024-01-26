using UnityEngine;

// 2024. 01. 25 Byun Jeongmin
public class PlayerDestroyState : PlayerGroundedState
{
    protected GameObject target;
    protected string targetTag;

    public PlayerDestroyState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        //Debug.Log("파괴 모드 on");
        _stateMachine.MovementSpeedModifier = 0;
        base.Enter();
        StartAnimation(_stateMachine.Player.AnimationData.InteractParameterHash);

        ToolItemData hammer = _stateMachine.Player.EquippedItem.itemData as ToolItemData;

        var targets = Physics.OverlapSphere(_stateMachine.Player.transform.position, hammer.range, hammer.targetLayers);
        if (targets.Length == 0)
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
            return;
        }

        target = targets[0].gameObject;
        targetTag = target.tag;
        Debug.Log($"target Name : {targetTag}");
        RotateOfTarget();
        _stateMachine.Player.Animator.SetBool(targetTag, true);
        return;
    }

    public override void Exit()
    {
        base.Exit();
        if (target != null)
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
                BuildableObject buildableObject = target.GetComponent<BuildableObject>();
                if (buildableObject != null)
                {
                    buildableObject.DestroyObject();
                }
                else
                {
                    GameObject.Destroy(target);
                }
            }

            _stateMachine.ChangeState(_stateMachine.IdleState);
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
