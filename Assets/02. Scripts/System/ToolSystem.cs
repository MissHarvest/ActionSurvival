using System;
using System.Collections.Generic;
using UnityEngine;

public class ToolSystem : MonoBehaviour
{
    public Transform handPosition;
    public Transform leftHandPosition;
    private Player _player;

    public QuickSlot EquippedTool = new QuickSlot();
    private ItemData _emptyhand;

    private Dictionary<string, GameObject> _tools = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> _twinTools = new Dictionary<string, GameObject>();

    public event Action<QuickSlot> OnEquip;
    public event Action<QuickSlot> OnUnEquip;

    private void Awake()
    {
        if (handPosition == null)
        {
            this.enabled = false;
            return;
        }
        _player = GetComponentInParent<Player>();
        _emptyhand = Managers.Resource.GetCache<ItemData>("EmptyHandItemData.data");

        var tools = Managers.Resource.GetCacheGroup<GameObject>("Handable_");
        foreach (var tool in tools)
        {
            var go = UnityEngine.Object.Instantiate(tool, handPosition);
            go.SetActive(false);
            _tools.TryAdd(tool.name, go);
        }

        var twinTools = Managers.Resource.GetCacheGroup<GameObject>("Handable_L_"); //lgs 24.01.23 TwinTool의 왼 손 도구를 새로운 컬렉션에 저장한다.
        foreach (var tool in twinTools)
        {
            var go = UnityEngine.Object.Instantiate(tool, leftHandPosition);
            go.SetActive(false);
            _twinTools.TryAdd(tool.name, go);
        }

        ClearHand();
        EquippedTool.itemSlot.Set(_emptyhand);
        Load();

        GameManager.Instance.OnSaveCallback += Save;
    }

    private void Start()
    {
        _player.QuickSlot.OnClickEmptySlot += OnItemUnregisted;
        _player.QuickSlot.OnUnRegisted += OnQuickSlotUnRegisted;
        _player.Inventory.OnUpdated += OnInventoryUpdated;
    }

    private void OnInventoryUpdated(int arg1, ItemSlot arg2)
    {
        if (arg2.itemData != null) return;
        if (EquippedTool.targetIndex != arg1) return;

        UnEquip();
    }

    private void OnQuickSlotUnRegisted(QuickSlot obj)
    {
        if(obj.targetIndex == EquippedTool.targetIndex &&
            obj.itemSlot.itemData == EquippedTool.itemSlot.itemData)
        {
            UnEquip();
        }
    }

    public void Equip(int index, ItemSlot itemSlot)
    {
        UnEquip();

        itemSlot.SetEquip(true);
        EquippedTool.Set(index, itemSlot);
        OnEquip?.Invoke(EquippedTool);
        SetToolActivate(true);
    }

    private void ClearHand()
    {
        EquippedTool.Clear();
        EquippedTool.itemSlot.Set(_emptyhand);
    }

    public void UnEquip()
    {
        if (EquippedTool.itemSlot.itemData == null) return;
        if (EquippedTool.itemSlot.itemData == _emptyhand) return;
        
        EquippedTool.itemSlot.SetEquip(false);        
        OnUnEquip?.Invoke(EquippedTool);
        SetToolActivate(false);
        ClearHand();
    }

    private void SetToolActivate(bool value)
    {
        if (EquippedTool.itemSlot.itemData is ArchitectureItemData)
        {
            _tools["Handable_Base"].SetActive(value);
        }
        else
        {
            var toolName = GetToolName(EquippedTool.itemSlot);
            _tools[toolName].SetActive(value);
            _tools[toolName].GetComponent<Weapon>()?.Link(EquippedTool);//EquippedTool
        }

        if (EquippedTool.itemSlot.itemData.name.Contains("Twin")) // TwinTool의 왼 손 도구를 활성화한다.
        {
            var twinToolName = GetTwinToolLeftHandName(EquippedTool.itemSlot);
            if (twinToolName.Contains("Handable_L_"))
            {
                _twinTools[twinToolName].SetActive(value);
                _twinTools[twinToolName].GetComponent<Weapon>()?.Link(EquippedTool);
            }
        }
    }

    public string GetToolName(ItemSlot itemSlot)
    {
        return "Handable_" + itemSlot.itemData.name.Replace("ItemData", "");
    }

    public string GetTwinToolLeftHandName(ItemSlot itemSlot) //lgs 24.01.23 TwinTool의 왼 손 도구의 이름을 재정의한다.
    {
        return "Handable_L_" + itemSlot.itemData.name.Replace("ItemData", "");
    }

    private void OnItemUnregisted()
    {
        if (_player.StateMachine.IsAttackState)
            return;

        UnEquip();
    }

    private void Load()
    {
        if (SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, "ToolSystem"))
        {
            for (int i = 0; i < 2; i++)
            {
                EquippedTool.itemSlot.LoadData();
            }
        }
    }

    private void Save()
    {
        var json = JsonUtility.ToJson(this);
        SaveGame.CreateJsonFile("ToolSystem", json, SaveGame.SaveType.Runtime);
    }
}
