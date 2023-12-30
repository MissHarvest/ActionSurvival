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
    private List<UIInventorySlot> _uiItemSlots = new List<UIInventorySlot>();

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

        CreateItemSlots(inventory);
        gameObject.SetActive(false);
    }

    private void CreateItemSlots(InventorySystem inventory)
    {
        int capacity = InventorySystem.maxCapacity;

        for(int i = 0; i < capacity; ++i)
        {
            var itemSlotUI = Managers.Resource.Instantiate("UIInventorySlot", Literals.PATH_UI, _contents);
            var inventoryslotUI = itemSlotUI.GetComponent<UIInventorySlot>();
            inventoryslotUI?.Init(this, i, inventory.slots[i]);
            _uiItemSlots.Add(inventoryslotUI);
        }
    }

    private void OnItemSlotUpdated(int index, ItemSlot itemSlot)
    {
        _uiItemSlots[index].Set(itemSlot);
    }
}
