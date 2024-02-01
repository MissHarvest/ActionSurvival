using System;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 29 Byun Jeongmin
public class Tutorial : MonoBehaviour
{
    private List<QuestSO> _quests; // 모든 퀘스트 리스트
    private List<QuestSO> _activeQuests = new List<QuestSO>(); // 현재 활성화된 퀘스트 리스트
    [SerializeField] private List<string> _questCompletionStatus = new List<string>(); //완료한 퀘스트 리스트

    public event Action OnActiveQuestsUpdated;

    public List<QuestSO> Quests
    {
        get { return _quests; }
        private set { _quests = value; }
    }

    public List<QuestSO> ActiveQuests
    {
        get { return _activeQuests; }
        private set { _activeQuests = value; }
    }


    private void Awake()
    {
        Initialize();
    }

    private void Initialize() //튜토리얼 저장 관련해서 초기화 부분은 바뀔수도??
    {
        // QuestSO 초기화
        _quests = new List<QuestSO>(Managers.Resource.GetCacheGroup<QuestSO>("QuestData"));
        _activeQuests.Clear();

        foreach (var quest in _quests)
        {
            quest.InitializeQuest();
            _questCompletionStatus.Clear();
            if (IsPreQuestsCleared(quest))
            {
                _activeQuests.Add(quest);
            }
        }
    }

    private void OnEnable()
    {
        BindInventoryEvents();
    }

    private void OnDisable()
    {
        CancelBindInventoryEvents();
    }

    // preQuests(선행퀘)가 비어 있거나, 모든 preQuests가 클리어된 경우에만 true
    private bool IsPreQuestsCleared(QuestSO quest)
    {
        if (quest.preQuests == null || quest.preQuests.Count == 0)
        {
            return true;
        }
        foreach (var preQuest in quest.preQuests)
        {
            if (!_questCompletionStatus.Contains(preQuest.questName))
            {
                return false;
            }
        }
        return true;
    }

    private void BindInventoryEvents()
    {
        Managers.Game.Player.Inventory.OnUpdated += OnInventoryUpdated;
    }

    private void CancelBindInventoryEvents()
    {
        Managers.Game.Player.Inventory.OnUpdated -= OnInventoryUpdated;
    }


    //인벤토리가 업데이트되면 클리어 조건 확인
    private void OnInventoryUpdated(int index, ItemSlot itemSlot)
    {
        List<QuestSO> questsToRemove = new List<QuestSO>();

        foreach (var activeQuest in _activeQuests)
        {
            if (IsEnoughRequirements(activeQuest, itemSlot))
            {
                ConfirmQuestCompletion(activeQuest);
                questsToRemove.Add(activeQuest);
            }
        }

        //클리어된 퀘스트는 activeQuests, quests에서 제외
        foreach (var questToRemove in questsToRemove)
        {
            _activeQuests.Remove(questToRemove);
            _quests.Remove(questToRemove);
        }

        // 선행 퀘가 모두 클리어된 경우 activeQuests에 추가
        foreach (var quest in _quests)
        {
            if (!_activeQuests.Contains(quest) && IsPreQuestsCleared(quest))
            {
                _activeQuests.Add(quest);
            }
        }
        OnActiveQuestsUpdated?.Invoke();
    }

    private bool IsEnoughRequirements(QuestSO quest, ItemSlot itemSlot)
    {
        foreach (var requiredItem in quest.requiredItems)
        {
            if (itemSlot.itemData != null && itemSlot.itemData.name == requiredItem.item.name)
            {
                //Debug.Log($"{requiredItem.item.displayName} 획득");
                return true;
            }
        }
        return false;
    }

    private void ConfirmQuestCompletion(QuestSO quest)
    {
        _questCompletionStatus.Add(quest.questName);
        quest.CompleteQuest();
        //Debug.Log($"{quest.questName} 퀘스트 클리어!!!!!!!!!!!!");
    }
}
