using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackState : BossBaseState
{
    public float _reach;
    // 데미지
    // 쿨타임
    
    public BossAttackState(BossStateMachine stateMachine) : base(stateMachine)
    {

    }
}
