using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 29 Byun Jeongmin
[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest/New Quest")]
public class QuestSO : ScriptableObject
{
    public string questName;
    public string questUIName;
    public int questID; //UI 띄울 때 사용하고, 0부터 시작하는 고유한 숫자여야 함

    public bool isCompleted;

    public List<QuestSO> preQuests; // 선행 퀘스트 리스트

    public List<RequiredItem> requiredItems = new List<RequiredItem>();

    [System.Serializable]
    public class RequiredItem
    {
        public ItemData item;
    }


    public void CompleteQuest()
    {
        isCompleted = true;
    }
}
