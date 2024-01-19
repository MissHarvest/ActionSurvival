using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IAttack
{
    // lgs 24.01.19
    private int damage; // SO의 데미지를 받아오자
    //private float knockback;


    private void Awake()
    {
        gameObject.GetComponentInChildren<BoxCollider>().enabled = false;
        Managers.Game.Player.ToolSystem.OnEquip += DamageOfTheEquippedWeapon;
        //knockback = Managers.Game.Player.Data.AttackData.AttackInfoDatas[0].Force;

    }

    private void OnTriggerEnter(Collider other)
    {
        //Vector3 playerPosition = Managers.Game.Player.transform.position;
        Attack(other.GetComponent<IHit>());

        //if (other.TryGetComponent(out ForceReceiver forceReceiver))
        //{
        //    Vector3 direction = (other.transform.position - playerPosition).normalized;
        //    forceReceiver.AddForce(direction * knockback);
        //    Debug.Log(direction);
        //}
    }

    public void Attack(IHit target)
    {
        if (target == null)
        {
            return;
        }
        target.Hit(this, damage);
    }

    public void DamageOfTheEquippedWeapon(QuickSlot quickSlot)
    {
        ItemData weapon = quickSlot.itemSlot.itemData;
        ToolItemData toolItemDate = (ToolItemData)weapon;

        damage += toolItemDate.damage;
    }
}
