using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
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
        Managers.Game.Player.Building.OnBuildCompleted += OnBuildUpdated;
    }

    private void OnInventoryUpdated(int index, ItemSlot itemSlot)
    {
        OnInventoryOrBuildUpdated(index, itemSlot, isBuildEvent: false);
    }

    private void OnBuildUpdated(int index)
    {
        OnInventoryOrBuildUpdated(index, itemSlot: null, isBuildEvent: true);
    }

    //인벤토리나 건축이 업데이트되면 클리어 조건 확인
    private void OnInventoryOrBuildUpdated(int index, ItemSlot itemSlot, bool isBuildEvent)
    {
        Func<Quest, bool> isEnoughRequirementsFunc = isBuildEvent ?
            (activeQuest => activeQuest.IsBuilt(index)) :
            (activeQuest => activeQuest.IsEnoughRequirements(index, itemSlot));

        _activeQuests
            .Where(isEnoughRequirementsFunc)
            .ToList()
            .ForEach(activeQuest =>
            {
                activeQuest.CompleteQuest();
                _activeQuests.Remove(activeQuest);
                _quests.Remove(activeQuest);
            });

        _quests
            .Where(quest => !_activeQuests.Contains(quest) && IsPreQuestsCleared(quest))
            .ToList()
            .ForEach(quest => _activeQuests.Add(quest));

        OnActiveQuestsUpdated?.Invoke();
    }
    #endregion

    #region Guide
    public void PathFinding(Quest quest)
    {
        var targetLayer = quest.questSO.targetLayer;
        var targetName = quest.questSO.targetName;

        var targets = Physics.SphereCastAll(transform.position, 50.0f, Vector3.up, 0, targetLayer);
        if (targetLayer == LayerMask.GetMask("Architecture"))
            targets = targets.Where(x => x.transform.name.Contains(targetName)).ToArray();
        else
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
            var findObjectName = quest.questSO.requiredItems[0].item.displayName;
            ui.SetWarning($"퀘스트 진행에 필요한 오브젝트가 주변에 존재하지 않습니다.");
        }
    }

    public void StartInvnetoryGuide(ItemData  itemData/* Craft/Inventory , target Item info */)
    {
        StartCoroutine(GuideInventroy(itemData));
    }

    IEnumerator GuideInventroy(ItemData itemData)
    {
        Managers.Game.Player.Inventory.FindItem(itemData, out int index);
        if (index == -1)
        {
            var warning =Managers.UI.ShowPopupUI<UIWarning>();
            warning.SetWarning($"{itemData.displayName} 을(를) 먼저 제작하세요.");
            yield break;
        }

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
