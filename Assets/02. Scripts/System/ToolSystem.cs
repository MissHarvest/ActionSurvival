using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class ToolSystem : MonoBehaviour
{
    public ItemSlot ItemInUse { get; private set; }
    public Transform handPosition;
    public GameObject ItemObject { get; private set; }

    public QuickSlot[] Equipments = new QuickSlot[(int)ItemParts.Max];
    private QuickSlot EmptyHand = new QuickSlot();

    private Dictionary<string, GameObject> _tools = new Dictionary<string, GameObject>();

    public event Action<QuickSlot> OnEquip;
    public event Action<QuickSlot> OnUnEquip;

    private void Awake()
    {
        if (handPosition == null)
        {
            Debug.LogWarning("handPosition is not allocated. Do Find [Hand R] / ToolSystem");
            this.enabled = false;
            return;
        }

        for(int i = 0; i < Equipments.Length; ++i)
        {
            Equipments[i] = new QuickSlot();
        }

        var emptyHandData = Managers.Resource.GetCache<ItemData>("EmptyHandItemData.data");
        EmptyHand.Set(-1, new(emptyHandData));

        var tools = Managers.Resource.GetCacheGroup<GameObject>("Handable_");
        foreach(var tool in tools)
        {
            var go = UnityEngine.Object.Instantiate(tool, handPosition);
            go.SetActive(false);
            _tools.TryAdd(tool.name, go);
        }

        Equip(EmptyHand);
    }

    private void Start()
    {
        Managers.Game.Player.QuickSlot.OnUnRegisted += OnItemUnregisted;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // 현재 손에 들고 있는 도구의 이름 가져오기
            string toolName = GetToolName(ItemInUse);

            // _tools에서 해당 도구를 가져와서 타입을 확인
            if (_tools.TryGetValue(toolName, out GameObject toolObject))
            {
                if (toolObject.GetComponent<BonFire>() != null)
                {
                    BonFire bonFireScript = toolObject.GetComponent<BonFire>();
                    bonFireScript.DropFireFromPlayerHand(ItemInUse, _tools);
                }
            }
        }
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
            Debug.Log($"{slot.itemSlot.itemData.displayName}");
            EquipTool(slot.itemSlot);
        }

        if (slot.itemSlot != EmptyHand.itemSlot)
        {
            Debug.Log($"Tool | R[{Equipments[part].itemSlot.registed}] E[{Equipments[part].itemSlot.equipped}]");
            OnEquip?.Invoke(Equipments[part]);
        }
    }

    private void EquipTool(ItemSlot itemSlot)
    {
        ItemInUse = itemSlot;
        var toolName = GetToolName(itemSlot);
        _tools[toolName].SetActive(true);
        Debug.Log($"{toolName}");
        ItemObject = _tools[toolName];
    }

    public void UnEquip(int part)
    {
        if (Equipments[part].itemSlot.itemData == null) return;

        Equipments[part].itemSlot.SetEquip(false);
        var toolName = GetToolName(Equipments[part].itemSlot);
        _tools[toolName].SetActive(false);

        if(-1 != Equipments[part].targetIndex)
        {
            OnUnEquip?.Invoke(Equipments[part]);
        }
        Equipments[part].Clear();
    }

    public string GetToolName(ItemSlot itemSlot)
    {
        return "Handable_" + itemSlot.itemData.name.Replace("ItemData", "");
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
