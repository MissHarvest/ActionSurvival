using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemUsageHelper : MonoBehaviour
{
    private Player _player;
    private Dictionary<Type, Action<int, ItemSlot>> _helper = new();

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _helper.TryAdd(typeof(FoodItemData), EatFood);
        _helper.TryAdd(typeof(ArchitectureItemData), Build);
        _helper.TryAdd(typeof(ToolItemData), EquipTool);
        _helper.TryAdd(typeof(WeaponItemData), EquipWeapon);
        _helper.TryAdd(typeof(EquipItemData), EquipArmor);
    }

    public void Use(int index)
    {
        var itemSlot = _player.Inventory.Get(index);
        var itemData = itemSlot.itemData;
        if (itemData == null) return;

        _helper[itemData.GetType()].Invoke(index, itemSlot);
    }

    private void EatFood(int index, ItemSlot itemSlot)
    {
        var foodItem = itemSlot.itemData as FoodItemData;
        if (foodItem == null) return;
        var conditionHandler = _player.ConditionHandler;

        foreach (var playerCondition in foodItem.conditionModifier)
        {
            switch (playerCondition.Condition)
            {
                case Conditions.HP:
                    conditionHandler.HP.Add(playerCondition.value);
                    break;

                case Conditions.Hunger:
                    conditionHandler.Hunger.Add(playerCondition.value);
                    break;
            }
        }

        _player.Inventory.TryConsumeQuantity(index, 1);
    }

    private void EquipTool(int index, ItemSlot itemSlot)
    {
        if (_player.StateMachine.IsAttackState)
            return;

        _player.ToolSystem.Equip(index, itemSlot);
    }

    private void EquipWeapon(int index, ItemSlot itemSlot)
    {
        if (_player.StateMachine.IsAttackState)
            return;

        _player.ToolSystem.Equip(index, itemSlot);
    }

    private void EquipArmor(int index, ItemSlot itemSlot)
    {
        if (_player.StateMachine.IsAttackState)
            return;

        _player.ArmorSystem.Equip(index, itemSlot);
    }

    private void Build(int index, ItemSlot itemSlot)
    {
        if (_player.StateMachine.IsAttackState)
            return;

        _player.Building.CreateArchitecture(index, itemSlot);
    }
}