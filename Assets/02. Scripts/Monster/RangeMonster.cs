using System.Collections;
using System.Collections.Generic;
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
        // 공격 사거리 탐지
        var targets = Physics.OverlapSphere(transform.position, Data.AttackData.AttackalbeDistance * 2.0f, targetLayers);

        if (targets.Length != 0)
        {
            var go = Instantiate(_projectileWeapon, fireTransform.position, Quaternion.identity);

            var monsterweapon = go.GetComponent<MonsterWeapon>();
            monsterweapon.ActivateWeapon();
            monsterweapon.Owner = this;

            var projectile = go.GetComponent<Projectile>();
            projectile.Fire(targets[0].transform.position, Data.AttackData.AttackalbeDistance * 2.0f);
        }
    }
}
