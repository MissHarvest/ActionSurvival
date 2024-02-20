using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// lgs 24.01.19

public class Weapon : MonoBehaviour, IAttack
{
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
        Managers.Game.Player.Inventory.TrySubtractDurability(_linkedSlot.targetIndex, 1);
    }

    public void DamageOfTheEquippedWeapon(QuickSlot quickSlot)
    {
        ItemData itemData = quickSlot.itemSlot.itemData;
        if(itemData is WeaponItemData weaponItem)
        {
            _damage = weaponItem.damage;
        }
    }

    public void Link(QuickSlot slot)
    {
        _linkedSlot = slot;
        if(_linkedSlot.itemSlot.itemData is WeaponItemData weaponItem)
        {
            _damage = weaponItem.damage;
        }
    }
}
