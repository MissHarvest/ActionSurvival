using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 29 Byun Jeongmin
public class UITutorial : UIPopup
{
    enum GameObjects
    {
        Character,
        Contents,
        Content,
        Exit,
    }

    private Transform _character;
    private Transform _contents;
    private Transform _content;
    private List<UIQuest> _uiQuestList = new List<UIQuest>();
    //private List<Quest> _quests = new List<Quest>();

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(GameObjects));
        Get<GameObject>((int)GameObjects.Exit).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });
    }

    private void Awake()
    {
        Initialize();
        _character = Get<GameObject>((int)GameObjects.Character).transform;
        _contents = Get<GameObject>((int)GameObjects.Contents).transform;
        _content = Get<GameObject>((int)GameObjects.Content).transform;

        // 비활성화 상태에서 시작
        gameObject.SetActive(false);
    }

    //public void SetQuests(List<Quest> _quests)
    //{
    //    _quests = _quests;
    //}

    //public void AddQuest(Quest quest)
    //{
    //    _quests.Add(quest);
    //}

    private void OnEnable()
    {
        //ShowQuest(GetQuestList());
        ShowQuest();
    }

    private void ShowQuest()
    {
        ClearSlots();
        //Debug.Log("퀘스트 개수 : " + _quests.Count);

        //if (_quests == null)
        //    return;
        
        // i < QuestList.Count
        for (int i = 0; i < 9; i++)
        {
            var questPrefab = Managers.Resource.GetCache<GameObject>("UIQuest.prefab");
            var questGO = Instantiate(questPrefab, _content); //instantiate 말고 오브젝트 풀링?비슷하게 쓰면 좋음
            var questSlot = questGO.GetComponent<UIQuest>();

            // 퀘스트 클릭 시 판넬 띄우기?
            questGO.BindEvent((x) =>
            {
                if (questGO.activeSelf)
                {
                    
                    gameObject.SetActive(false);
                }
            });

            _uiQuestList.Add(questSlot);
        }
    }

    private void ClearSlots()
    {
        foreach (var slot in _uiQuestList)
        {
            Destroy(slot.gameObject);
        }
        _uiQuestList.Clear();
    }
}
