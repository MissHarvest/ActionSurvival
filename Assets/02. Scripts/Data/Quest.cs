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
        if (questSO.type == QuestSO.QuestType.Using)
        {
            switch (questSO.requiredItems[0].item is ToolItemData toolItem && toolItem.isArchitecture)
            {
                case true: // 건축(buildingsystem) 퀘스트일 경우, 퀵슬롯 등록해서 건축할 경우
                    Debug.Log("등록하기 후 건축");
                    if (Managers.Game.Player.Inventory.slots[index].itemData.name == null)
                        return true;
                    break;
                case false: // 장착(toolsystem) 퀘스트일 경우
                    Debug.Log("장착 퀘스트애오");
                    for (int i = 0; i < Managers.Game.Player.QuickSlot.slots.Length; i++)
                    {
                        if (index == Managers.Game.Player.QuickSlot.slots[i].targetIndex)
                            if (Managers.Game.Player.QuickSlot.slots[i].itemSlot.itemData.name == questSO.requiredItems[0].item.name)
                                return true;
                    }
                    break;
            }
        }
        else
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

    public bool IsBuilt(int index) // 해당 인덱스에 아이템이 없으면 건축하느라 아이템 소비된 것?
    {
        if (questSO.type == QuestSO.QuestType.Using && questSO.requiredItems[0].item is ToolItemData toolItem && toolItem.isArchitecture)
        {
            Debug.Log("건축하기 눌러서 건축");
            if (Managers.Game.Player.Inventory.slots[index].itemData == null)
                return true;
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
