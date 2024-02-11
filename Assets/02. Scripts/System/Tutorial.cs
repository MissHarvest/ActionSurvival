using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UIBase;

// 2024. 01. 29 Byun Jeongmin
public class Tutorial : MonoBehaviour
{
    [SerializeField] public List<Quest> _quests; // 아직 클리어하지 않은 퀘스트 리스트
    [SerializeField] private List<Quest> _activeQuests = new List<Quest>(); // 현재 활성화된 퀘스트 리스트

    public event Action OnActiveQuestsUpdated;
    private PathFinder _pathFinder;
    public Collider[] groups;

    public List<Quest> ActiveQuests
    {
        get { return _activeQuests; }
        private set { _activeQuests = value; }
    }

    private void Awake()
    {
        Initialize();
        Load();

        var go = Instantiate(Managers.Resource.GetCache<GameObject>("PathFinder.prefab"), transform);
        _pathFinder = go.GetComponent<PathFinder>();
        _pathFinder.gameObject.SetActive(false);

        Managers.Game.OnSaveCallback += Save;
    }

    private void Initialize()
    {
        _quests = Managers.Resource.GetCacheGroup<QuestSO>("QuestData")
            .Select(questSO => new Quest(questSO))
            .ToList();

        _activeQuests = _quests
            .Where(quest => IsPreQuestsCleared(quest))
            .ToList();
    }


    private void Start()
    {
        BindInventoryEvents();
    }

    #region Quest Logic
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
                return true;
            }
        }
        return false;
    }

    private void ConfirmQuestCompletion(Quest quest)
    {
        quest.CompleteQuest();
    }
    #endregion

    #region Guide
    public void PathFinding(LayerMask targetLayer, string targetName)
    {
        var targets = Physics.SphereCastAll(transform.position, 50.0f, Vector3.up, 0, targetLayer);
        targets = targets.Where(x => x.transform.parent.name.Contains(targetName)).ToArray();
        
        if (targets.Length > 0)
        {
            targets = targets.OrderBy(x => (transform.position - x.collider.gameObject.transform.position).sqrMagnitude).ToArray();
            groups = targets.Select(x => x.collider).ToArray();

            _pathFinder.gameObject.SetActive(true);
            _pathFinder.SetDestination(targets[0].collider.gameObject.transform.position);
        }
        else
        {
            var ui = Managers.UI.ShowPopupUI<UIWarning>();
            ui.SetWarning($"주변에 {targetName}가 존재하지 않습니다.");
        }
    }

    public void StartInvnetoryGuide(ItemData  itemData/* Craft/Inventory , target Item info */)
    {
        StartCoroutine(GuideInventroy(itemData));
    }

    IEnumerator GuideInventroy(ItemData itemData)
    {
        Managers.Game.Player.Inventory.FindItem(itemData, out int index);
        if (index == -1) yield break;

        Managers.UI.TryGetSceneUI<UIMainScene>(out UIMainScene sceneUI);
        if (sceneUI == null) yield break;

        var menuUI = sceneUI.UIMenu.GetComponent<UIMenu>();
        menuUI.HighLightInventoryButton();

        var guideUI = Managers.UI.GetPopupUI<UITutorialArrow>();
        if(guideUI == null) yield break;

        yield return new WaitWhile(() => guideUI.gameObject.activeSelf);

        var invenUI = Managers.UI.GetPopupUI<UIInventory>();
        yield return new WaitWhile(() => !invenUI.gameObject.activeSelf);

        invenUI.HighLightItemSlot(index);
        yield return new WaitWhile(() => guideUI.gameObject.activeSelf);

        yield return new WaitForSeconds(0.2f);
        if (itemData is ToolItemData == false) yield break;
        var toolData = (ToolItemData)itemData;

        var type = toolData.isArchitecture ? UIItemUsageHelper.Functions.Build : UIItemUsageHelper.Functions.Regist;
        invenUI.HighLightHelper(type);
    }

    public void GuideCraft()
    {
        Managers.UI.TryGetSceneUI<UIMainScene>(out UIMainScene sceneUI);
        if (sceneUI == null) return;

        var menuUI = sceneUI.UIMenu.GetComponent<UIMenu>();
        menuUI.HighLightCraftButton();
    }
    #endregion

    #region Save & Load
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
    #endregion
}
