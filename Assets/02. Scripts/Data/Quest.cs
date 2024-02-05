using System;
using UnityEngine;

// 2024. 02. 05 Byun Jeongmin

[Serializable]
public class Quest
{
    [field: SerializeField] public QuestSO questSO { get; private set; } = null;
    //[field: SerializeField] public string questName { get; private set; } = "";
    [field: SerializeField] public bool isCompleted { get; private set; } = false;

    public Quest(QuestSO questSO)
    {
        this.questSO = questSO;
        //this.questName = questSO.name;
        this.isCompleted = false;
    }

    public void CompleteQuest()
    {
        isCompleted = true;
    }
}
