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
        GameManager.Instance.Player.Tutorial.OnActiveQuestsUpdated += HandleActiveQuestsUpdated;
        _activeQuests = GameManager.Instance.Player.Tutorial.ActiveQuests;
        CreateOrUpdateQuestUI();
        ShowQuest();
    }

    private void HandleActiveQuestsUpdated()
    {
        CreateOrUpdateQuestUI();
        ShowQuest();
    }

    private void CreateOrUpdateQuestUI()
    {
        int requiredQuestCount = _activeQuests.Count;

        // 현재 프리팹 개수와 필요한 프리팹 개수를 비교
        if (_uiQuests.Count < requiredQuestCount)
        {
            // 부족하면 추가 생성
            int questsToCreate = requiredQuestCount - _uiQuests.Count;

            for (int i = 0; i < questsToCreate; i++)
            {
                var questPrefab = Managers.Resource.GetCache<GameObject>("UIQuest.prefab");
                var questGO = Instantiate(questPrefab, _content);
                var uiQuest = questGO.GetComponent<UIQuest>();
                _uiQuests.Add(uiQuest);
                questGO.SetActive(false);
            }
        }
        else if (_uiQuests.Count > requiredQuestCount)
        {
            // 불필요한 프리팹은 비활성화
            for (int i = requiredQuestCount; i < _uiQuests.Count; i++)
            {
                _uiQuests[i].gameObject.SetActive(false);
            }
        }

        // 프리팹 개수만큼 Set
        for (int i = 0; i < requiredQuestCount; i++)
        {
            _uiQuests[i].Set(_activeQuests[i], GameManager.Instance.Player.Tutorial);
            _uiQuests[i].gameObject.SetActive(true);
        }
    }

    private void ShowQuest()
    {
        // 활성화된 퀘스트 UI만 활성화
        for (int i = 0; i < _activeQuests.Count; ++i)
        {
            _uiQuests[i].Set(_activeQuests[i], GameManager.Instance.Player.Tutorial);
            _uiQuests[i].gameObject.SetActive(true);
        }

        // 모든 퀘스트 클리어 시 퀘스트 UI 닫음
        if (_activeQuests.Count == 0)
        {
            gameObject.SetActive(false);
        }
    }
}
