using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;

public class BossStateMachine : StateMachine
{
    public BossMonster Boss { get; }

    public BossIdleState IdleState { get; }
    public float DetectionDist { get; private set; }
    public float DetectionDistModifier { get; set; } = 1.0f;
    public BossStateMachine(BossMonster boss)
    {
        this.Boss = boss;
        //MovementSpeed = monster.Data.MovementData.BaseSpeed;
        DetectionDist = boss.Data.AttackData.DetectionDistance;
        
        IdleState = new BossIdleState(this);
    }
}
