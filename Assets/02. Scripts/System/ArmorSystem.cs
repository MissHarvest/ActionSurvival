using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Lee gyuseong 24.02.07

public class ArmorSystem : MonoBehaviour
{
    ////이게 아니고 ArmorSlot를 관리하는 ArmorSystem이 있고 거기서 장착 해제시 Inventory의 ItemSlot.ItemData의 데이터를 참조해서 ArmorSlot에 전달한다.
    ////그래야 Inventory의 ItemSlot.ItemData == null 체크가 가능할 듯 하다.
    ////
    //private int _defense;

    //public QuickSlot[] _linkedSlots;

    //private void Awake()
    //{
    //    _linkedSlots = new QuickSlot[2];

    //    Managers.Game.Player.ToolSystem.OnEquip += DefenseOfTheEquippedArmor;
    //    Managers.Game.Player.ToolSystem.OnUnEquip += DefenseOfTheUnEquippedArmor;
    //    Managers.Game.Player.OnHit += Duration;
    //}

    //public void DefenseOfTheEquippedArmor(QuickSlot quickSlot)
    //{
    //    ItemData armor = quickSlot.itemSlot.itemData;
    //    EquipItemData toolItemDate = (EquipItemData)armor;
    //    _defense += toolItemDate.defense;
    //    Managers.Game.Player.playerDefense = _defense;

    //    if ((int)toolItemDate.part == 0 || (int)toolItemDate.part == 1)
    //    {
    //        _linkedSlots[(int)toolItemDate.part] = quickSlot;
    //    }
    //}

    //public void DefenseOfTheUnEquippedArmor(QuickSlot quickSlot)
    //{
    //    ItemData armor = quickSlot.itemSlot.itemData;
    //    EquipItemData toolItemDate = (EquipItemData)armor;
    //    _defense -= toolItemDate.defense;
    //    Managers.Game.Player.playerDefense = _defense;

    //    if ((int)toolItemDate.part == 0 || (int)toolItemDate.part == 1)
    //    {
    //        _linkedSlots[(int)toolItemDate.part] = null;
    //    }
    //}

    //public void Duration() // Player Hit에서 호출
    //{
    //    foreach (var items in _linkedSlots)
    //    {
    //        if (items != null)
    //        {
    //            Managers.Game.Player.Inventory.UseToolItemByIndex(items.targetIndex, 1);
    //        }
    //    }
    //}
}
