using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;

public class UIStorage : UIPopup
{
    enum GameObjects
    {
        Exit
    }

    enum Container
    {
        PlayerInventory,
        StorageInventory,
    }

    private Storage _targetStorage;
    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(GameObjects));
        Bind<UIItemSlotContainer>(typeof(Container));
        Get<GameObject>((int)GameObjects.Exit).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });
    }

    private void Awake()
    {
        Initialize();
        var slotPrefab = Managers.Resource.GetCache<GameObject>("UIInventorySlot.prefab");

        var inventory = Managers.Game.Player.Inventory;
        var inventoryContainer = Get<UIItemSlotContainer>((int)Container.PlayerInventory);
        inventoryContainer.CreateItemSlots<UIInventorySlot>(slotPrefab, inventory.maxCapacity);
        inventoryContainer.Init<UIInventorySlot>(inventory, ActivateItemTransferHelper);
        inventory.OnUpdated += inventoryContainer.OnItemSlotUpdated;

        var storageContainer = Get<UIItemSlotContainer>((int)Container.StorageInventory);
        storageContainer.CreateItemSlots<UIInventorySlot>(slotPrefab, 20);

        gameObject.SetActive(false);
    }

    public void SetStorage(Storage storage)
    {
        var container = Get<UIItemSlotContainer>((int)Container.StorageInventory);
        container.Init<UIInventorySlot>(storage.InventorySystem, ActivateItemTransferHelper);

        storage.InventorySystem.OnUpdated += container.OnItemSlotUpdated;

        
        for (int i = 0; i < storage.InventorySystem.maxCapacity; ++i)
        {
            container.OnItemSlotUpdated(i, storage.InventorySystem.slots[i]);
        }
    }

    private void OnDisable()
    {
        var container = Get<UIItemSlotContainer>((int)Container.StorageInventory);
        if (_targetStorage)
        {
            _targetStorage.InventorySystem.OnUpdated -= container.OnItemSlotUpdated;
        }        
    }


    private void ActivateItemTransferHelper(UIItemSlot itemslotUI)
    {

    }
}
