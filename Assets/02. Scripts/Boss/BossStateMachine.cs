using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;

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
    public float DetectionDist { get; private set; }
    public float DetectionDistModifier { get; set; } = 1.0f;
    public GameObject Target { get; set; } = null;
    public Queue<BossAttackState> Skill { get; private set; } = new();
    public BossStateMachine(BossMonster boss)
    {
        this.Boss = boss;
        MovementSpeed = boss.Data.MovementData.BaseSpeed;
        DetectionDist = boss.Data.AttackData.DetectionDistance;
        
        IdleState = new BossIdleState(this);
        SleepState = new BossSleepState(this);
        BattleState = new BossBattleState(this);
        ScreamState = new BossScreamState(this);
        RushSate = new BossRushState(this);
        BreathState = new BossBreathState(this);
        BiteState = new BossBiteState(this);
        StabState = new BossStabbingState(this);
        MeteorState = new BossMeteorState(this);

        Skill.Enqueue(MeteorState);
        Skill.Enqueue(BreathState);
        Skill.Enqueue(RushSate);
        //Skill.Enqueue(BiteState);
    }
}
