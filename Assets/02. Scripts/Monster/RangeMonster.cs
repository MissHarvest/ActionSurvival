using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//2024. 01. --  Park Jun Uk
public class RangeMonster : Monster
{
    private GameObject _projectileWeapon;
    public Transform fireTransform;
    public LayerMask targetLayers;

    protected override void Awake()
    {
        base.Awake();
        var monsterName = name.Replace("(Clone)", "");
        _projectileWeapon = Managers.Resource.GetCache<GameObject>($"{monsterName}Projectile.prefab");
    }

    public override void TryAttack()
    {
        if (Target == null) return;

        var go = Instantiate(_projectileWeapon, fireTransform.position, Quaternion.identity);

        var monsterweapon = go.GetComponent<MonsterWeapon>();
        monsterweapon.ActivateWeapon();
        monsterweapon.Owner = this;

        var projectile = go.GetComponent<Projectile>();

        projectile.Fire(Target.transform.position, Data.AttackData.AttackalbeDistance * 2.0f);
    }
}
