using System;
using System.Collections.Generic;
using UnityEngine;

public class ToolSystem : MonoBehaviour
{
    [field: SerializeField] public ItemSlot ItemInUse { get; private set; }
    public Transform handPosition;
    public Transform leftHandPosition;

    public QuickSlot[] Equipments = new QuickSlot[(int)ItemParts.Max];
    private QuickSlot EmptyHand = new QuickSlot();

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

        for (int i = 0; i < Equipments.Length; ++i)
        {
            Equipments[i] = new QuickSlot();
        }

        var emptyHandData = Managers.Resource.GetCache<ItemData>("EmptyHandItemData.data");
        EmptyHand.Set(-1, new(emptyHandData));


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


        Equip(EmptyHand);
    }

    private void Start()
    {
        Managers.Game.Player.QuickSlot.OnUnRegisted += OnItemUnregisted;
    }

    public void Equip(QuickSlot slot)
    {
        int part = GetPart(slot);
        if (part == -1) return;

        UnEquip(part);

        Equipments[part].Set(slot.targetIndex, slot.itemSlot);
        Equipments[part].itemSlot.SetEquip(true);

        if (part == (int)ItemParts.Hand)
        {
            Debug.Log($"[Equip]{slot.itemSlot.itemData.displayName}");
            //EquipTool(Equipments[part].itemSlot);
            EquipTool(slot);
        }

        if (slot.itemSlot != EmptyHand.itemSlot)
        {
            OnEquip?.Invoke(Equipments[part]);
        }
    }

    private void EquipTool(QuickSlot slot)
    {
        ItemInUse.Set(slot.itemSlot);

        // ToolItemData의 isArchitecture가 true면 나뭇잎을 손에 들게
        if (ItemInUse.itemData is ToolItemData toolItem && toolItem.isArchitecture)
        {
            _tools["Handable_Base"].SetActive(true);
        }
        else
        {
            var toolName = GetToolName(ItemInUse);
            _tools[toolName].SetActive(true);
            _tools[toolName].GetComponent<Weapon>()?.Link(slot);
        }

        if (ItemInUse.itemData.name.Contains("Twin")) // TwinTool의 왼 손 도구를 활성화한다.
        {
            var twinToolName = GetTwinToolLeftHandName(ItemInUse);
            if (twinToolName.Contains("Handable_L_") == true)
            {
                _twinTools[twinToolName].SetActive(true);
                _twinTools[twinToolName].GetComponent<Weapon>()?.Link(slot);
            }
        }
    }

    public void UnEquip(int part)
    {
        if (Equipments[part].itemSlot.itemData == null) return;

        Equipments[part].itemSlot.SetEquip(false);

        var toolName = GetToolName(Equipments[part].itemSlot);
        Debug.Log($"[UnEquip ToolName] {toolName}");
        if (ItemInUse.itemData is ToolItemData toolItem && toolItem.isArchitecture)
        {
            _tools["Handable_Base"].SetActive(false);
        }
        else if (part == (int)ItemParts.Body)
        {

        }
        else
        {
            _tools[toolName].SetActive(false);
        }

        if (Equipments[part].itemSlot.itemData.name.Contains("Twin"))
        {
            var twinToolName = GetTwinToolLeftHandName(Equipments[part].itemSlot);
            if (twinToolName.Contains("Handable_L_") == true)
            {
                _twinTools[twinToolName].SetActive(false);
            }
        }

        if (-1 != Equipments[part].targetIndex)
        {
            OnUnEquip?.Invoke(Equipments[part]);
        }
        Equipments[part].Clear();
    }

    public string GetToolName(ItemSlot itemSlot)
    {
        return "Handable_" + itemSlot.itemData.name.Replace("ItemData", "");
    }

    public string GetTwinToolLeftHandName(ItemSlot itemSlot) //lgs 24.01.23 TwinTool의 왼 손 도구의 이름을 재정의한다.
    {
        return "Handable_L_" + itemSlot.itemData.name.Replace("ItemData", "");
    }

    public void UnEquip(QuickSlot slot)
    {
        int part = GetPart(slot);
        if (part == -1) return;

        if (Equipments[part].itemSlot.itemData == slot.itemSlot.itemData)
        {
            UnEquip(part);
        }
    }

    private int GetPart(QuickSlot slot)
    {
        var itemData = slot.itemSlot.itemData as EquipItemData;
        if (itemData == null) return -1;
        return (int)itemData.part;
    }

    public void ClearHand()
    {
        Equip(EmptyHand);
    }

    private void OnItemUnregisted(QuickSlot slot)
    {
        UnEquip(slot);
        ClearHand();
    }
}
