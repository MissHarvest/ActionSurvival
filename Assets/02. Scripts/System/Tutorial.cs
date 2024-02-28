using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// 2024. 01. 29 Byun Jeongmin
public class Tutorial : MonoBehaviour
{
    [SerializeField] private List<Quest> _quests; // 아직 클리어하지 않은 퀘스트 리스트
    [SerializeField] private List<Quest> _activeQuests = new List<Quest>(); // 현재 활성화된 퀘스트 리스트
    private List<Vector3> artifactPositions = new List<Vector3>();

    private bool isSummerQuestAdded = false;
    private bool isWinterQuestAdded = false;

    public event Action OnActiveQuestsUpdated;
    private PathFinder _pathFinder;
    public Collider[] groups;

    private Player _player;

    public List<Quest> ActiveQuests
    {
        get { return _activeQuests; }
        private set { _activeQuests = value; }
    }

    private void Awake()
    {
        Initialize();
        Load();
        _player = GetComponentInParent<Player>();
        var go = Instantiate(Managers.Resource.GetCache<GameObject>("PathFinder.prefab"), transform);
        _pathFinder = go.GetComponent<PathFinder>();
        _pathFinder.gameObject.SetActive(false);

        GameManager.Instance.OnSaveCallback += Save;
    }

    private void Initialize()
    {
        _quests = new(Managers.Resource.GetCacheGroup<QuestSO>("QuestData")
            .Select(questSO => new Quest(questSO))
            .ToList());

        _activeQuests = _quests
            .Where(quest => IsPreQuestsCleared(quest) && !quest.questSO.canBeAddedWithoutPreQuests)
            .ToList();
    }

    private void Start()
    {
        BindEvents();
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

    private void BindEvents()
    {
        _player.Inventory.OnUpdated += OnInventoryUpdated;
        _player.Building.OnBuildCompleted += OnBuildUpdated;
        GameManager.DayCycle.OnEveningCame += OnAddSeasonQuestUpdated;
    }

    private void OnInventoryUpdated(int index, ItemSlot itemSlot)
    {
        Func<Quest, bool> isEnoughRequirementsFunc = (activeQuest) => activeQuest.IsEnoughRequirements(index, itemSlot);
        OnInventoryOrBuildUpdated(isEnoughRequirementsFunc);
    }

    private void OnBuildUpdated(int index)
    {
        Func<Quest, bool> isBuiltFunc = (activeQuest) => activeQuest.IsBuilt(index);
        OnInventoryOrBuildUpdated(isEnoughRequirementsFunc: isBuiltFunc);
    }

    //인벤토리나 건축이 업데이트되면 클리어 조건 확인
    private void OnInventoryOrBuildUpdated(Func<Quest, bool> isEnoughRequirementsFunc)
    {
        List<Quest> questsToRemove = new List<Quest>();

        _activeQuests.RemoveAll(activeQuest =>
        {
            if (isEnoughRequirementsFunc(activeQuest))
            {
                activeQuest.CompleteQuest();
                RemoveActiveQuest(activeQuest);
                return true;
            }
            return false;
        });

        foreach (var quest in _quests)
        {
            if (!IsQuestInActiveQuests(quest) && IsPreQuestsCleared(quest) && !quest.questSO.canBeAddedWithoutPreQuests)
            {
                _activeQuests.Add(quest);
            }
        }
        OnActiveQuestsUpdated?.Invoke();
    }

    private void RemoveActiveQuest(Quest activeQuest)
    {
        foreach (var quest in _quests)
        {
            if (activeQuest.questName == quest.questName)
            {
                _quests.Remove(quest);
                break;
            }
        }
    }

    private bool IsQuestInActiveQuests(Quest quest)
    {
        foreach (var activeQuest in _activeQuests)
        {
            if (activeQuest.questName == quest.questName)
                return true;
        }
        return false;
    }

    public void OnAddSeasonQuestUpdated()
    {
        if (GameManager.Season.IsFireIslandActive && !isSummerQuestAdded)
        {
            AddSeasonQuest("DestroyFireArtifactQuestData");
            isSummerQuestAdded = true;
        }
        else if (GameManager.Season.IsIceIslandActive && !isWinterQuestAdded)
        {
            AddSeasonQuest("DestroyIceArtifactQuestData");
            isWinterQuestAdded = true;
        }
    }

    private void AddSeasonQuest(string questName)
    {
        Quest seasonQuest = _quests.First(quest => quest.questSO.name == questName);
        _activeQuests.Add(seasonQuest);
        PathFinding(seasonQuest);
        OnActiveQuestsUpdated?.Invoke();
    }
    #endregion

    #region Guide
    public void PathFinding(Quest quest)
    {
        var targetLayer = quest.questSO.targetLayer;
        var targetName = quest.questSO.targetName;

        if (targetName == "Artifact")
        {
            artifactPositions.Clear();

            foreach (var artifact in GameManager.ArtifactCreator.Artifacts)
            {
                artifactPositions.Add(artifact.transform.position);
            }

            if (artifactPositions.Count == 0)
            {
                var ui = Managers.UI.ShowPopupUI<UIWarning>();
                ui.SetWarning("퀘스트 진행에 필요한 Artifact가 주변에 존재하지 않습니다.");
                return;
            }

            artifactPositions = artifactPositions.OrderBy(pos => (transform.position - pos).sqrMagnitude).ToList();
            groups = artifactPositions
                .Select(pos => Physics.OverlapSphere(pos, 5.0f, targetLayer, QueryTriggerInteraction.Collide))
                .SelectMany(x => x)
                .ToArray();

            _pathFinder.gameObject.SetActive(true);
            _pathFinder.SetDestination(artifactPositions[0]);
        }
        else
        {
            var allHits = Physics.SphereCastAll(transform.position, 50.0f, Vector3.up, 0, targetLayer, QueryTriggerInteraction.Collide);
            var validHits = allHits.Where(hit =>
            {
                if (targetLayer == LayerMask.GetMask("Resources"))
                {
                    var targetTransform = hit.transform.parent;
                    if (targetTransform != null)
                        return targetTransform.name.Contains(targetName);
                }
                else
                    return hit.transform.name.Contains(targetName);

                return false;
            }).ToArray();

            if (validHits.Length > 0)
            {
                validHits = validHits.OrderBy(x => (transform.position - x.collider.gameObject.transform.position).sqrMagnitude).ToArray();
                groups = validHits.Select(x => x.collider).ToArray();

                _pathFinder.gameObject.SetActive(true);
                _pathFinder.SetDestination(validHits[0].collider.gameObject.transform.position);
            }
            else
            {
                var ui = Managers.UI.ShowPopupUI<UIWarning>();
                var findObjectName = quest.questSO.requiredItems[0].item.displayName;
                ui.SetWarning($"퀘스트 진행에 필요한 {findObjectName}가 주변에 존재하지 않습니다.");
            }
        }
    }

    public void StartInvnetoryGuide(ItemData itemData/* Craft/Inventory , target Item info */)
    {
        StartCoroutine(GuideInventroy(itemData));
    }

    IEnumerator GuideInventroy(ItemData itemData)
    {
        int index = _player.Inventory.GetIndexOfItem(itemData);
        if (index == -1)
        {
            var warning = Managers.UI.ShowPopupUI<UIWarning>();
            warning.SetWarning($"{itemData.displayName} 을(를) 먼저 제작하세요.");
            yield break;
        }

        Managers.UI.TryGetSceneUI<UIMainScene>(out UIMainScene sceneUI);
        if (sceneUI == null) yield break;

        var menuUI = sceneUI.UIMenu.GetComponent<UIMenu>();
        menuUI.HighLightInventoryButton();

        var guideUI = Managers.UI.GetPopupUI<UITutorialArrow>();
        if (guideUI == null) yield break;

        yield return new WaitWhile(() => guideUI.gameObject.activeSelf);

        var invenUI = Managers.UI.GetPopupUI<UIInventory>();
        yield return new WaitWhile(() => !invenUI.gameObject.activeSelf);

        invenUI.HighLightItemSlot(index);
        yield return new WaitWhile(() => guideUI.gameObject.activeSelf);

        yield return new WaitForSeconds(0.2f);
        if (itemData is ToolItemData == false) yield break;
        var toolData = (ToolItemData)itemData;

        UIItemUsageHelper.Functions type = UIItemUsageHelper.Functions.Destroy;
        switch (itemData)
        {
            case ArchitectureItemData _:
                type = UIItemUsageHelper.Functions.Build;
                break;

            case ToolItemData _:
                type = UIItemUsageHelper.Functions.Regist;
                break;

            case EquipItemData _:
                type = UIItemUsageHelper.Functions.Equip;
                break;

            case FoodItemData _:
                type = UIItemUsageHelper.Functions.Use;
                break;
        }

        if (!(type == UIItemUsageHelper.Functions.Destroy))
        {
            invenUI.HighLightHelper(type);
        }
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
        if (SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, "Tutorial"))
        {
            foreach (var quest in _quests)
            {
                if (quest.questName != string.Empty)
                    quest.LoadData();
            }

            foreach (var quest in _activeQuests)
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
