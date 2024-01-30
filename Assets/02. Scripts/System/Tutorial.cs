using System;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 29 Byun Jeongmin
public class Tutorial : MonoBehaviour
{
    private List<string> questCompletionStatus = new List<string>(); //진행중인 퀘스트만 UI에 떠야하니까 따로 처리 필요
    private List<QuestSO> quests;
    private UITutorial _tutorialUI; //childUI

    public void Initialize() //튜토리얼 저장 관련해서 초기화 부분은 바뀔수도??
    {
        // QuestSO 초기화
        quests = new List<QuestSO>(Managers.Resource.GetCacheGroup<QuestSO>("QuestData"));
        foreach (var quest in quests)
        {
            quest.InitializeQuest();
            //questCompletionStatus[quest.questName] = false; //add로 추가
            questCompletionStatus.Clear();
        }
    }

    private void Awake()
    {
        BindInventoryEvents();
    }

    private void OnEnable()
    {
        BindInventoryEvents();
    }

    private void OnDisable()
    {
        CancelBindInventoryEvents();
    }

    private void SetQuestsToUI()
    {
        //_mainSceneUI = Managers.UI.ShowSceneUI<UIMainScene>(); // UIMainScene 내에서 UITutorial로 처리
        //Managers.UI.TryGetSceneUI(out mainSceneUI);
        //_mainSceneUI.SetQuests(quests); // UITutoriral에서 Manager.game.player.tutorial로 접근(이벤트)
    }

    private void BindInventoryEvents()
    {
        Managers.Game.Player.Inventory.OnUpdated += OnInventoryUpdated;
    }

    private void CancelBindInventoryEvents()
    {
        Managers.Game.Player.Inventory.OnUpdated -= OnInventoryUpdated;
    }


    //인벤토리가 업데이트되면 클리어 조건 확인(사과, getapplequestdata 일반화하기)
    private void OnInventoryUpdated(int index, ItemSlot itemSlot)
    {
        foreach (var activeQuest in GetActiveQuests())
        {
            Debug.Log("퀘스트명"+activeQuest.name);
            if (IsEnoughRequirements(activeQuest, itemSlot))
            {
                CheckQuestCompletion(activeQuest);
            }
        }
    }

    //활성화된 퀘스트 리스트에 퀘스트 추가
    private List<QuestSO> GetActiveQuests()
    {
        List<QuestSO> activeQuests = new List<QuestSO>();
        

        foreach (var quest in quests)
        {
            if (!questCompletionStatus.Exists(questName => questName == quest.questName))
            {
                activeQuests.Add(quest);
            }
        }
        return activeQuests;
    }

    private bool IsEnoughRequirements(QuestSO quest, ItemSlot itemSlot)
    {
        foreach (var requiredItem in quest.requiredItems)
        {
            if (itemSlot.itemData != null && itemSlot.itemData.name == requiredItem.item.name)
            {
                requiredItem.OnItemAcquired();
                Debug.Log($"{requiredItem.item.displayName} 1개 획득");

                if (requiredItem.currentQuantity >= requiredItem.quantity)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void QuestCompleted(string questName)
    {
        Debug.Log($"{questName} 퀘스트 클리어");
    }

    private void CheckQuestCompletion(QuestSO quest)
    {
        questCompletionStatus.Add(quest.questName);
        quest.CompleteQuest();
        QuestCompleted(quest.questName);
    }
}
