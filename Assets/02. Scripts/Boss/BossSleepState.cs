using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSleepState : BossBaseState
{
    private UIBossHP _bossUI= null;

    public BossSleepState(BossStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        if(_bossUI != null)
        {
            _bossUI.gameObject.SetActive(false);            
        }
        _stateMachine.Target = null;
        StartAnimation(_stateMachine.Boss.AnimationData.SleepParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        _stateMachine.Boss.gameObject.layer = 7;
        _stateMachine.DetectionDistModifier = _stateMachine.Boss.Data.AttackData.ChaseDectionDistModifier;
        _stateMachine.Boss.HP.regenRate = 0.0f;
        _stateMachine.InitPattern();
        Managers.UI.TryGetSceneUI<UIMainScene>(out UIMainScene scene);
        _bossUI = scene.UIBoss.GetComponent<UIBossHP>();
        _bossUI.SetBoss(_stateMachine.Boss);
        StopAnimation(_stateMachine.Boss.AnimationData.SleepParameterHash);
    }

    public override void Update()
    {        
        var array = Physics.OverlapSphere(_stateMachine.Boss.transform.position, 30, 1 << 9);
        if (array.Length == 0) return;

        if (_stateMachine.Target != null) return;
        _stateMachine.Target = array[0].gameObject;
        
        _stateMachine.ChangeState(_stateMachine.ScreamState);
    }
}
