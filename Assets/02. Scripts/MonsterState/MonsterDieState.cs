using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 15 Park Jun Uk
public class MonsterDieState : MonsterBaseState
{
    public MonsterDieState(MonsterStateMachine monsterStateMachine) : base(monsterStateMachine)
    {

    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0.0f;
        base.Enter();
        _stateMachine.Monster.Animator.SetTrigger(_stateMachine.Monster.AnimationData.DieParameterHash);
    }

    public override void Exit() 
    {
        base.Exit();
        
    }

    public override void Update()
    {
        float normalizedTime = GetNormalizedTime(_stateMachine.Monster.Animator, "Die");
        if (normalizedTime >= 1.0f)
        {
            _stateMachine.Monster.gameObject.SetActive(false);
            if (_stateMachine.Monster.Habitat == null)
            {
                Object.Destroy(_stateMachine.Monster.gameObject);
            }
        }
    }
}
