using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AttackInfo
{
    public IHit target;
    public float damage;

    public AttackInfo(IHit target, float damage)
    {
        this.target = target;
        this.damage = damage;
    }
}

public interface IAttack
{
    void Attack(AttackInfo damage);    
}
