using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : UIPopup
{
    enum Gameobjects
    {
        Contents,
        Exit,
    }
        
    private Transform _contents;
    private List<UIItemSlot> _uiItemSlots = new List<UIItemSlot>();

    private void Awake()
    {
        Bind<GameObject>(typeof(Gameobjects));
        _contents = Get<GameObject>((int)Gameobjects.Contents).transform;
        Get<GameObject>((int)Gameobjects.Exit).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });
        
        // Create Slot
        var inventory = GameObject.Find("Player").GetComponentInChildren<InventorySystem>();
        inventory.OnUpdated += OnItemSlotUpdated;
        CreateItemSlots(InventorySystem.maxCapacity);//inventory 를 넘겨주는게 나을듯
    }

    private void CreateItemSlots(int capacity)
    {
        for(int i = 0; i < capacity; ++i)
        {
            var itemSlotUI = Managers.Resource.Instantiate("UIItemSlot", Literals.PATH_UI, _contents);
            var uiItemSlot = itemSlotUI.GetComponent<UIItemSlot>();
            uiItemSlot?.SetIndex(i);
            _uiItemSlots.Add(uiItemSlot);
        }
    }

    private Transform FindItemSlotRoot(Transform root)
    {
        for(int i = 0; i < root.childCount; ++i)
        {
            if(root.GetChild(i).childCount != 0)
            {
                return FindItemSlotRoot(root.GetChild(i));
            }
            else
            {
                if(root.GetChild(i).name == "Contents")
                {
                    return root.GetChild(i);
                }
            }
        }
        return null;
    }

    private void OnItemSlotUpdated(int index, ItemSlot itemSlot)
    {
        _uiItemSlots[index].Set(itemSlot);
    }
}
