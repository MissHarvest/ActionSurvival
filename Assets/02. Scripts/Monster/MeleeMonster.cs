using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMonster : Monster
{
    [SerializeField] protected MonsterWeapon _monsterWeapon;

    protected override void Awake()
    {
        base.Awake();
        if(_monsterWeapon)
        {
            _monsterWeapon.Owner = this;
        }
    }

    public override void TryAttack()
    {
        _monsterWeapon.ActivateWeapon(Data.AttackData.TimeColliderInactivated);
    }

    public override void OffAttack()
    {
        _monsterWeapon.InactivateWeapon();
    }
}
