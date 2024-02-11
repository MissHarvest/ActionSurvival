using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 2024. 01. 29 Byun Jeongmin
public class Tutorial : MonoBehaviour
{
    [SerializeField] private List<Quest> _quests; // 아직 클리어하지 않은 퀘스트 리스트
    [SerializeField] private List<Quest> _activeQuests = new List<Quest>(); // 현재 활성화된 퀘스트 리스트

    public event Action OnActiveQuestsUpdated;

    public List<Quest> ActiveQuests
    {
        get { return _activeQuests; }
        private set { _activeQuests = value; }
    }

    private void Awake()
    {
        Initialize();
        Load();

        Managers.Game.OnSaveCallback += Save;
    }

    private void Initialize()
    {
        _quests = Managers.Resource.GetCacheGroup<QuestSO>("QuestData")
            .Select(questSO => new Quest(questSO))
            .OrderBy(quest => quest.questSO.questID) // questID를 기준으로 오름차순 정렬
            .ToList();

        _activeQuests = _quests
            .Where(quest => IsPreQuestsCleared(quest))
            .ToList();
    }



    private void Start()
    {
        BindInventoryEvents();
    }

    // preQuests(선행퀘)가 비어 있거나, 모든 preQuests가 클리어된 경우에만 true
    private bool IsPreQuestsCleared(Quest quest)
    {
        if (quest.questSO.preQuests == null || quest.questSO.preQuests.Count == 0)
        {
            return true;
        }
        
        foreach (var preQuest in quest.questSO.preQuests)
        {
            if (_quests.Any(q => q.questSO == preQuest))
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

    //인벤토리가 업데이트되면 클리어 조건 확인
    private void OnInventoryUpdated(int index, ItemSlot itemSlot)
    {
        _activeQuests
            .Where(activeQuest => IsEnoughRequirements(activeQuest, itemSlot))
            .ToList()
            .ForEach(activeQuest => {
                ConfirmQuestCompletion(activeQuest);
                _activeQuests.Remove(activeQuest);
                _quests.Remove(activeQuest);
            });

        _quests
            .Where(quest => !_activeQuests.Contains(quest) && IsPreQuestsCleared(quest))
            .ToList()
            .ForEach(quest => _activeQuests.Add(quest));

        OnActiveQuestsUpdated?.Invoke();
    }


    private bool IsEnoughRequirements(Quest quest, ItemSlot itemSlot)
    {
        foreach (var requiredItem in quest.questSO.requiredItems)
        {
            if (itemSlot.itemData != null && itemSlot.itemData.name == requiredItem.item.name)
            {
                //Debug.Log($"{requiredItem.item.displayName} 획득");
                return true;
            }
        }
        return false;
    }

    private void ConfirmQuestCompletion(Quest quest)
    {
        quest.CompleteQuest();
        // Debug.Log($"{quest.QuestSO.questName} 퀘스트 클리어!!!!!!!!!!!!");
    }

    public virtual void Load()
    {
        if(SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, "Tutorial"))
        {
            foreach(var quest in _quests)
            {
                if(quest.questName != string.Empty)
                    quest.LoadData();
            }

            foreach(var quest in _activeQuests)
            {
                if (quest.questName != string.Empty)
                    quest.LoadData();
            }
        }
    }

    protected virtual void Save()
    {
        var json = JsonUtility.ToJson(this);
        SaveGame.CreateJsonFile("Tutorial", json, SaveGame.SaveType.Runtime);
    }
}
