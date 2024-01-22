using OpenCover.Framework.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IAttack
{
    // lgs 24.01.19
    private int _damage; // SO의 데미지를 받아오자

    private void Awake()
    {
        gameObject.GetComponentInChildren<BoxCollider>().enabled = false;
        Managers.Game.Player.ToolSystem.OnEquip += DamageOfTheEquippedWeapon;
    }

    private void OnTriggerEnter(Collider other)
    {
        Attack(other.GetComponent<IHit>());
    }

    public void Attack(IHit target)
    {
        if (target == null)
        {
            return;
        }
        target.Hit(this, _damage);
    }

    public void DamageOfTheEquippedWeapon(QuickSlot quickSlot)
    {
        ItemData weapon = quickSlot.itemSlot.itemData;
        ToolItemData toolItemDate = (ToolItemData)weapon;

        _damage += toolItemDate.damage;
    }
}
