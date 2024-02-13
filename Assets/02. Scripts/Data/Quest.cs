using System;
using UnityEngine;

// 2024. 02. 05 Byun Jeongmin

[Serializable]
public class Quest
{
    public QuestSO questSO { get; private set; } = null;
    [field: SerializeField] public string questName { get; private set; } = string.Empty;
    [field: SerializeField] public bool isCompleted { get; private set; } = false;

    private Action onQuestCompletedCallback;

    public Quest(QuestSO questSO)
    {
        this.questSO = questSO;
        this.questName = questSO.name;
        this.isCompleted = false;
    }

    public bool IsEnoughRequirements(ItemSlot itemSlot) // quest Type이 using일 경우 바로 return false, requireItem 0개
    {
        if (questSO.type == QuestSO.QuestType.Using)
            return false;

        foreach (var requiredItem in questSO.requiredItems)
        {
            if (itemSlot.itemData != null && itemSlot.itemData.name == requiredItem.item.name)
            {
                //Debug.Log($"{requiredItem.item.displayName} 획득");
                return true;
            }
        }
        return false;
    }

    public void SetQuestCompletedCallback(Action callback)
    {
        onQuestCompletedCallback = callback;
    }

    public void CompleteQuest()
    {
        isCompleted = true;

        onQuestCompletedCallback?.Invoke();
    }

    public void LoadData()
    {
        var path = $"{questName}.data";
        questSO = Managers.Resource.GetCache<QuestSO>(path);
    }
}
