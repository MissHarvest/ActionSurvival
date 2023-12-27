using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

public class PlayerInteractState : PlayerBaseState
{
    protected GameObject target;
    public PlayerInteractState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0;
        base.Enter();
        
        ToolItemData tool = _stateMachine.Player.equippedTool;
        
        var targets = Physics.OverlapSphere(_stateMachine.Player.transform.position, tool.range, tool.targetLayers);
        if (targets.Length != 0 && (targets[0].CompareTag(tool.targetTagName) || targets[0].CompareTag("Gather")))
        {
            target = targets[0].gameObject;
            Debug.Log($"target Name : {target.name}");
            StartAnimation(_stateMachine.Player.AnimationData.InteractParameterHash);
        }
        else
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
        }    
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_stateMachine.Player.AnimationData.InteractParameterHash);
    }

    public override void Update()
    {
        // exit 조건 설정
        float normalizedTime = GetNormalizedTime(_stateMachine.Player.Animator, "Interact");
        // Debug.Log("nor" + normalizedTime);
        if(normalizedTime >= 1f)
        {
            if(target != null)
                target.GetComponent<IInteractable>()?.Interact(_stateMachine.Player);
            _stateMachine.ChangeState(_stateMachine.IdleState);
        }
    }
}
