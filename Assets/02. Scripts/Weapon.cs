//using OpenCover.Framework.Model; lgs 24.01.29
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IAttack
{
    // lgs 24.01.19
    [SerializeField] private int _damage; // SO의 데미지를 받아오자
    private QuickSlot _linkedSlot;

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
        if (target == null) return;
        target.Hit(this, _damage);
        Managers.Game.Player.Inventory.UseToolItemByIndex(_linkedSlot.targetIndex, 1);
    }

    public void DamageOfTheEquippedWeapon(QuickSlot quickSlot)
    {
        ItemData weapon = quickSlot.itemSlot.itemData;
        ToolItemData toolItemDate = weapon is ToolItemData ? (ToolItemData)weapon: null;
        if (toolItemDate == null) return;
        _damage = toolItemDate.damage;
    }

    public void Link(QuickSlot slot)
    {
        _linkedSlot = slot;
        if(_linkedSlot.itemSlot.itemData is ToolItemData tool)
        {
            _damage = tool.damage;
        }
    }
}
