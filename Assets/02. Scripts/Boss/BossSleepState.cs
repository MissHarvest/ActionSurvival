using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSleepState : BossBaseState
{
    public BossSleepState(BossStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(_stateMachine.Boss.AnimationData.SleepParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_stateMachine.Boss.AnimationData.SleepParameterHash);
    }

    public override void Update()
    {
        var array = Physics.OverlapSphere(_stateMachine.Boss.transform.position, 30, 1 << 9);
        if (array.Length == 0) return;

        if (_stateMachine.Target != null) return;
        _stateMachine.Target = array[0].gameObject;
        Debug.Log("Target Name " + _stateMachine.Target.name);
        _stateMachine.ChangeState(_stateMachine.ScreamState);
    }
}
