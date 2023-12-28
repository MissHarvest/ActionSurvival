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

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(Gameobjects));
        Get<GameObject>((int)Gameobjects.Exit).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });
    }

    private void Awake()
    {
        Initialize();
        _contents = Get<GameObject>((int)Gameobjects.Contents).transform;

        // Create Slot
        var inventory = Managers.Game.Player.Inventory;
        inventory.OnUpdated += OnItemSlotUpdated;
        CreateItemSlots(inventory);//inventory 를 넘겨주는게 나을듯
        gameObject.SetActive(false);
    }

    private void CreateItemSlots(InventorySystem inventory)
    {
        int capacity = InventorySystem.maxCapacity;

        for(int i = 0; i < capacity; ++i)
        {
            var itemSlotUI = Managers.Resource.Instantiate("UIItemSlot", Literals.PATH_UI, _contents);
            var uiItemSlot = itemSlotUI.GetComponent<UIItemSlot>();
            uiItemSlot?.SetIndex(i);
            uiItemSlot?.Set(inventory.slots[i]);
            _uiItemSlots.Add(uiItemSlot);
        }
    }

    private void OnItemSlotUpdated(int index, ItemSlot itemSlot)
    {
        _uiItemSlots[index].Set(itemSlot);
    }
}
