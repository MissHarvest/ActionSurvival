using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 29 Byun Jeongmin
public class UITutorial : UIPopup
{
    enum GameObjects
    {
        Content,
    }

    private Transform _content;
    private List<Quest> _activeQuests;
    private QuestSO[] _quests;
    private List<UIQuest> _uiQuests = new List<UIQuest>();
    
    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(GameObjects));
    }

    private void Awake()
    {
        Initialize();
        _content = Get<GameObject>((int)GameObjects.Content).transform;
    }

    private void Start()
    {
        Managers.Game.Player.Tutorial.OnActiveQuestsUpdated += HandleActiveQuestsUpdated;
        _quests = Managers.Resource.GetCacheGroup<QuestSO>("QuestData");
        _activeQuests = Managers.Game.Player.Tutorial.ActiveQuests;
        CreateQuestUI();
        ShowQuest(); 
    }

    private void HandleActiveQuestsUpdated()
    {
        ShowQuest();
    }

    private void CreateQuestUI()
    {
        var questPrefab = Managers.Resource.GetCache<GameObject>("UIQuest.prefab");

        // 풀에 퀘스트 UI 미리 생성
        for (int i = 0; i < _quests.Length; i++)
        {
            var questGO = Instantiate(questPrefab, _content);
            var quest = questGO.GetComponent<UIQuest>();
            _uiQuests.Add(quest);
            questGO.SetActive(false);
        }
    }

    private void ShowQuest()
    {
        ClearQuests();

        // 활성화된 퀘스트 UI만 활성화
        for(int i = 0; i < _activeQuests.Count; ++i)
        {
            _uiQuests[i].Set(_activeQuests[i], Managers.Game.Player.Tutorial);
            _uiQuests[i].gameObject.SetActive(true);
        }

        // 모든 퀘스트 클리어 시 퀘스트 UI 닫음
        if (_activeQuests.Count == 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void ClearQuests()
    {
        // 모든 퀘스트 UI를 비활성화
        foreach (var quest in _uiQuests)
        {
            quest.gameObject.SetActive(false);
        }
    }
}
