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

    public bool IsBuilt(ArchitectureItemData architectureItemData) // 건축 퀘스트일 경우
    {
        if (questSO.type == QuestSO.QuestType.Using)
        {
            if (architectureItemData.name == questSO.requiredItems[0].item.name)
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
