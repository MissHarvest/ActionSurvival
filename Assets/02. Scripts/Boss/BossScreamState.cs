using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossScreamState : BossAttackState
{
    private bool _canScream = true;
    private float _sfxVolume = 0.7f;

    public BossScreamState(BossStateMachine stateMachine) : base(stateMachine)
    {
        _reach = 100.0f;
        cooltime = 60.0f;
        weight = 1.0f;
    }

    public override void Enter()
    {
        _stateMachine.MovementSpeedModifier = 0.0f;
        base.Enter();        
        StartAnimation(_stateMachine.Boss.AnimationData.ScreamParamterHash);
        if(_stateMachine.isBattaleState == false)
        {
            _stateMachine.isBattaleState = true;
        }
        else
        {
            // 몬스터 소환
        }
    }

    public override void Exit()
    {
        base.Exit();
        _canScream = true;
        StopAnimation(_stateMachine.Boss.AnimationData.ScreamParamterHash);
    }

    public override void Update()
    {
        float normalizedTime = GetNormalizedTime(_stateMachine.Boss.Animator, "Scream");
        if (normalizedTime >= 1.0f)
        {
            _stateMachine.ChangeState(_stateMachine.BattleState);
        }
        else if(normalizedTime > 0.2f && _canScream)
        {
            _canScream = false;
            Managers.Sound.PlayEffectSound(_stateMachine.Boss.transform.position, "Scream", _sfxVolume);
        }       
    }
}
