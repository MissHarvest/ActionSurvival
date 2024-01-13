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
        //EmptyHand.Set(-1, new ItemSlot((ItemData)Resources.Load<ScriptableObject>("SO/EmptyHandItemData")));

        //var tools = Managers.Resource.GetPrefabs(Literals.PATH_HANDABLE);
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

    public void Equip(QuickSlot slot)
    {
        int part = GetPart(slot);
        if (part == -1) return;        

        UnEquip(part);

        Equipments[part].Set(slot.targetIndex, slot.itemSlot);
        Equipments[part].itemSlot.SetEquip(true);
        if (part == (int)ItemParts.Hand)
        {
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

        // Managers.Game.Player.Animator.SetBool(Managers.Game.Player.AnimationData.EquipTwoHandedToolIdleParameterHash, true);

        _tools[toolName].GetComponent<ItemObjectData>()?.OnEquipTwoHandedTool(); // lgs
        ItemObject = _tools[toolName];
        // 게임오브젝트 리스트를 만들었고, 딕셔너리 내부의 게임오브젝트 내부의 겟컴포넌트를 가져와서 ? 앞이 null이 아니면 함수를 호출한다.
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
            _tools[toolName].GetComponent<ItemObjectData>()?.OnUnEquipTwoHandedTool(); // lgs
        }
        Equipments[part].Clear();
    }

    private string GetToolName(ItemSlot itemSlot)
    {
        return "Handable_" + itemSlot.itemData.name.Replace("ItemData", "");
        //return itemSlot.itemData.name.Replace("ItemData", "");
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
