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
    private List<UIQuest> _uiQuestPool = new List<UIQuest>();

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

    private void OnEnable()
    {

        _quests = Managers.Resource.GetCacheGroup<QuestSO>("QuestData");
        _activeQuests = Managers.Game.Player.Tutorial.ActiveQuests;
        CreateQuestUIPool();
        ShowQuest(); // 게임 시작 시 퀘스트 표시
    }

    private void HandleActiveQuestsUpdated()
    {
        ShowQuest();
    }

    private void CreateQuestUIPool() //생성 시 순서가 questID랑 같아야 할듯?
    {
        var questPrefab = Managers.Resource.GetCache<GameObject>("UIQuest.prefab");

        // 풀에 퀘스트 UI 미리 생성
        for (int i = 0; i < _quests.Length; i++)
        {
            var questGO = Instantiate(questPrefab, _content);
            var quest = questGO.GetComponent<UIQuest>();
            quest.Set(i);
            _uiQuestPool.Add(quest);
            questGO.SetActive(false);
        }
    }

    private void ShowQuest()
    {
        ClearQuests();

        // 활성화된 퀘스트 UI만 활성화
        for (int i = 0; i < _activeQuests.Count; i++)
        {
            Quest activeQuest = _activeQuests[i];

            for (int j = 0; j < _uiQuestPool.Count; j++)
            {
                if (activeQuest.questSO.questID == j)
                {
                    UIQuest matchingUIQuest = _uiQuestPool[j];
                    matchingUIQuest.gameObject.SetActive(true);
                }

            }
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
        foreach (var quest in _uiQuestPool)
        {
            quest.gameObject.SetActive(false);
        }
    }
}
