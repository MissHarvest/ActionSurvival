using System;
using Unity.VisualScripting;
using UnityEngine;

// 2024. 02. 05 Byun Jeongmin
[Serializable]
public class Quest
{
    public QuestSO questSO { get; private set; } = null;
    [field: SerializeField] public string questName { get; private set; } = string.Empty;
    [field: SerializeField] public bool isCompleted { get; private set; } = false;

    public Quest(QuestSO questSO)
    {
        this.questSO = questSO;
        this.questName = questSO.name;
        this.isCompleted = false;
    }

    public bool IsEnoughRequirements(int index, ItemSlot itemSlot)
    {
        if (questSO.type == QuestSO.QuestType.Using) // 장착 퀘스트일 경우
        {
            return itemSlot.itemData == questSO.requiredItems[0].item && itemSlot.equipped;
            //if (itemSlot.itemData == questSO.requiredItems[0].item)
            //if (Managers.Game.Player.Inventory.Get(index).itemData is ToolItemData)
            //{
            //    for (int i = 0; i < Managers.Game.Player.QuickSlot.slots.Length; i++)
            //    {
            //        if (index == Managers.Game.Player.QuickSlot._slots[i].targetIndex)
            //            if (Managers.Game.Player.QuickSlot._slots[i].itemSlot.itemData.name == questSO.requiredItems[0].item.name)
            //                return true;
            //    }
            //}
        }
        else // 제작 퀘스트일 경우
        {
            foreach (var requiredItem in questSO.requiredItems)
            {
                if (itemSlot.itemData != null && itemSlot.itemData.name == requiredItem.item.name)
                {
                    //Debug.Log($"{requiredItem.item.displayName} 획득");
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsBuilt(int index) // 건축 퀘스트일 경우
    {
        if (questSO.type == QuestSO.QuestType.Using)
        {
            if (Managers.Game.Player.Inventory.Get(index).itemData is ArchitectureItemData)
            {
                if (Managers.Game.Player.Inventory.Get(index).itemData.name == questSO.requiredItems[0].item.name)
                    return true;
            }
        }
        return false;
    }

    public void CompleteQuest()
    {
        isCompleted = true;
        Debug.Log($"{questName}퀘스트 클리어");
    }

    public void LoadData()
    {
        var path = $"{questName}.data";
        questSO = Managers.Resource.GetCache<QuestSO>(path);
    }
}
