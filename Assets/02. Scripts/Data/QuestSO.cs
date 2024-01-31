using System.Collections.Generic;
using UnityEngine;
using static RecipeSO;

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

    public void InitializeQuest()
    {
        foreach (var requiredItem in requiredItems)
        {
            //requiredItem.currentQuantity = 0; //SO가 이렇게 직접적으로 변경되는 건 안 좋은 것 같다 ,,
            isCompleted = false;
        }
    }

    [System.Serializable]
    public class RequiredItem
    {
        public ItemData item;
        //public int quantity;
        //public int currentQuantity = 0; // 현재 획득한 수량

        //public void OnItemAcquired()
        //{
        //    currentQuantity++; // 인벤토리 onupdated마다 호출됨, 아이템 여러 개 획득 시 여러 번 호출됨
        //}
    }


    public void CompleteQuest()
    {
        isCompleted = true;
    }
}
