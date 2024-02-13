using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class BossStateMachine : StateMachine
{
    public BossMonster Boss { get; }
    public bool isBattaleState = false;
    public float MovementSpeed { get; private set; }
    public float MovementSpeedModifier { get; set; } = 1.0f;
    public BossIdleState IdleState { get; }
    public BossBattleState BattleState { get; }
    public BossScreamState ScreamState { get; }
    public BossRushState RushSate { get; }
    public BossSleepState SleepState { get; }
    public BossBreathState BreathState { get; }
    public BossBiteState BiteState { get; }
    public BossStabbingState StabState { get; }
    public BossMeteorState MeteorState { get; }
    public BossInvincibilityState InvincibilityState { get; }
    public BossDieState DieState { get; }

    public float DetectionDist { get; private set; }
    public float DetectionDistModifier { get; set; } = 1.0f;
    public GameObject Target { get; set; } = null;
    
    public WeightedRandomPicker<BossAttackState> Skill = new();

    private int _phase = 1;

    public BossAttackState NextAttackState { get; set; } = null;

    public BossStateMachine(BossMonster boss)
    {
        this.Boss = boss;

        boss.HP.OnDecreased += AddMeteorPattern;

        MovementSpeed = boss.Data.MovementData.BaseSpeed;
        DetectionDist = boss.Data.AttackData.DetectionDistance;

        InvincibilityState = new BossInvincibilityState(this);
        IdleState = new BossIdleState(this);
        SleepState = new BossSleepState(this);
        BattleState = new BossBattleState(this);
        ScreamState = new BossScreamState(this);
        RushSate = new BossRushState(this);
        BreathState = new BossBreathState(this);// 2 phase
        BiteState = new BossBiteState(this);
        StabState = new BossStabbingState(this); // 2phase
        MeteorState = new BossMeteorState(this);
        DieState = new BossDieState(this);

        Skill.AddItem(RushSate, RushSate.weight);
        Skill.AddItem(BiteState, BiteState.weight);

        CoroutineManagement.Instance.StartCoroutine(GetNextSkill());
    }

    private void AddMeteorPattern(float hpPercent)
    {
        if(hpPercent <= 0.7f && _phase == 1)
        {
            Skill.AddItem(MeteorState, MeteorState.weight);
            Skill.AddItem(StabState, StabState.weight);
            Skill.AddItem(BreathState, BreathState.weight);
            ++_phase;
        }
        else if(hpPercent <= 0.3f && _phase == 2)
        {
            Skill.AddItem(MeteorState, MeteorState.weight);
            ++_phase;
        }
    }

    public BossAttackState GetUsableSkill()
    {
        var v = Target.transform.position - Boss.transform.position;
        v.y = 0;
        var sqrDist = v.sqrMagnitude;        
        var skill = Skill.AddWeightAndPick((x) =>
        {
            return (x._reach * x._reach) > sqrDist;
        }, 30);

        return skill;
    }

    IEnumerator GetNextSkill()
    {
        while (true)
        {
            yield return null;

            if (Target == null) continue;

            if (NextAttackState != null) continue;

            if (Skill.Count <= 0) continue;

            NextAttackState = GetUsableSkill();
        }
    }
}
