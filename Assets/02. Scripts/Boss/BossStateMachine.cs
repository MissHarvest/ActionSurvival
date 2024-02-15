using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    
    public WeightedRandomPicker<BossAttackState> SkillPicker = new();
    public List<BossAttackState> Skills = new();

    public int Phase { get; private set; } = 1;

    public BossAttackState NextAttackState { get; set; } = null;
    public Transform ObjectPoolContainer { get; private set; }
    public BossStateMachine(BossMonster boss)
    {
        this.Boss = boss;

        ObjectPoolContainer = new GameObject("@Pool Container").transform;

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

        CoroutineManagement.Instance.StartCoroutine(GetNextSkill());
    }

    private void AddMeteorPattern(float hpPercent)
    {
        if(hpPercent <= 0.7f && Phase == 1)
        {
            Skills.Add(MeteorState);
            Skills.Add(StabState);
            Skills.Add(BreathState);
            ++Phase;
        }
        else if(hpPercent <= 0.3f && Phase == 2)
        {
            Skills.Add(MeteorState);
            ++Phase;
        }
    }

    public void InitPattern()
    {
        for(int i = 0; i < Skills.Count; ++i)
        {
            Skills[i].Cancel();
        }
        Skills.Clear();

        Skills.Add(RushSate);
        Skills.Add(BiteState);
        Skills.Add(ScreamState);

        Phase = 1;
    }

    public BossAttackState GetUsableSkill()
    {
        var v = Target.transform.position - Boss.transform.position;
        v.y = 0;
        var sqrDist = v.sqrMagnitude;
        var list = Skills.Where(x => x.usable).ToList();

        foreach(var skill in list)
        {
            var add = (skill._reach * skill._reach) > sqrDist ? 30 : 0;
            SkillPicker.AddItem(skill, skill.weight + add);
        }

        if (SkillPicker.Count <= 0) return null;

        var selected = SkillPicker.Pick();
        SkillPicker.Clear();
        return selected;
    }

    IEnumerator GetNextSkill()
    {
        while (true)
        {
            yield return null;

            if (Target == null) continue;

            if (NextAttackState != null) continue;

            NextAttackState = GetUsableSkill();
        }
    }
}
