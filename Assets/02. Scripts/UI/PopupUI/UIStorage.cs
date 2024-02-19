using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.Port; lgs 24.01.29

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

    enum Helper
    {
        TransitionHelper,
    }

    private Storage _targetStorage;
    public QuickSlot SelectedSlot { get; private set; } = new QuickSlot();

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(GameObjects));
        Bind<UIItemSlotContainer>(typeof(Container));
        Bind<UIItemTransitionHelper>(typeof(Helper));

        Get<GameObject>((int)GameObjects.Exit).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });
    }

    private void Awake()
    {
        Initialize();
        var slotPrefab = Managers.Resource.GetCache<GameObject>("UIInventorySlot.prefab");

        var inventory = Managers.Game.Player.Inventory;
        var inventoryContainer = Get<UIItemSlotContainer>((int)Container.PlayerInventory);
        inventoryContainer.CreateItemSlots<UIInventorySlot>(slotPrefab, inventory.maxCapacity);
        inventoryContainer.Init<UIInventorySlot>(inventory, ActivateTransitionHelperToStorage);
        inventory.OnUpdated += inventoryContainer.OnItemSlotUpdated;

        var storageContainer = Get<UIItemSlotContainer>((int)Container.StorageInventory);
        storageContainer.CreateItemSlots<UIInventorySlot>(slotPrefab, 20);

        gameObject.SetActive(false);
    }

    private void Start()
    {
        var helper = Get<UIItemTransitionHelper>((int)Helper.TransitionHelper);
        helper.BindEventOfButton(UIItemTransitionHelper.Functions.Store, StoreItem);
        helper.BindEventOfButton(UIItemTransitionHelper.Functions.TakeOut, TakeOutItem);
    }

    public void SetStorage(Storage storage)
    {
        _targetStorage = storage;
        var container = Get<UIItemSlotContainer>((int)Container.StorageInventory);
        container.Init<UIInventorySlot>(storage.InventorySystem, ActivateTransitionHelperToInventory);

        storage.InventorySystem.OnUpdated += container.OnItemSlotUpdated;
                
        for (int i = 0; i < storage.InventorySystem.maxCapacity; ++i)
        {
            container.OnItemSlotUpdated(i, storage.InventorySystem.Get(i));
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

    private void ActivateTransitionHelperToInventory(UIItemSlot itemslotUI)
    {
        SelectedSlot.Set(itemslotUI.Index, _targetStorage.InventorySystem.Get(itemslotUI.Index));
        var inventoryslotui = itemslotUI as UIInventorySlot;
        var pos = new Vector3(inventoryslotui.transform.position.x + inventoryslotui.RectTransform.sizeDelta.x,
            inventoryslotui.transform.position.y);
        Get<UIItemTransitionHelper>((int)Helper.TransitionHelper).ShowOption(SelectedSlot.itemSlot, pos);
    }

    private void ActivateTransitionHelperToStorage(UIItemSlot itemslotUI)
    {
        SelectedSlot.Set(itemslotUI.Index, Managers.Game.Player.Inventory.Get(itemslotUI.Index));
        var inventoryslotui = itemslotUI as UIInventorySlot;
        var pos = new Vector3(inventoryslotui.transform.position.x + inventoryslotui.RectTransform.sizeDelta.x,
            inventoryslotui.transform.position.y);
        Get<UIItemTransitionHelper>((int)Helper.TransitionHelper).ShowOption(SelectedSlot.itemSlot, pos);
    }

    private void StoreItem()
    {
        //SelectedSlot
        if (SelectedSlot.itemSlot.equipped || SelectedSlot.itemSlot.registed)
        {
            Managers.UI.ShowPopupUI<UIWarning>().SetWarning(
                "장착 또는 등록된 아이템은 이동할 수 없습니다.");
            return;
        }

        Managers.Game.Player.Inventory.TransitionItem(_targetStorage.InventorySystem, SelectedSlot.targetIndex);
        Get<UIItemTransitionHelper>((int)Helper.TransitionHelper).gameObject.SetActive(false);
    }

    private void TakeOutItem()
    {
        _targetStorage.InventorySystem.TransitionItem(Managers.Game.Player.Inventory, SelectedSlot.targetIndex);
        Get<UIItemTransitionHelper>((int)Helper.TransitionHelper).gameObject.SetActive(false);
    }
}
