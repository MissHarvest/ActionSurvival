using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMonster : Monster
{
    protected MeleeAttackArea AttackArea { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        AttackArea = GetComponentInChildren<MeleeAttackArea>();
    }

    public override void TryAttack()
    {
        var players = Physics.OverlapBox(AttackArea.Center,
            AttackArea.BoxSize,
            transform.rotation,
            1 << 9,
            QueryTriggerInteraction.Collide);
        
        if(players.Length > 0)
        {
            players[0].GetComponent<IHit>()?.Hit(this, Data.AttackData.Atk);
        }
    }
}
