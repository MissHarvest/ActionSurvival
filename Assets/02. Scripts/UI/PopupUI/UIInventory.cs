using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class UIInventory : UIPopup
{
    enum Gameobjects
    {
        Exit,
    }

    enum Container
    {
        Contents,
    }

    enum Helper
    {
        UsageHelper,
    }

    public QuickSlot SelectedSlot { get; private set; } = new QuickSlot();

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(Gameobjects));
        Bind<UIItemSlotContainer>(typeof(Container));
        Bind<UIItemUsageHelper>(typeof(Helper));
        Get<GameObject>((int)Gameobjects.Exit).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });
    }

    private void Awake()
    {
        Initialize();
        var inventory = Managers.Game.Player.Inventory;
        var prefab = Managers.Resource.GetCache<GameObject>("UIInventorySlot.prefab");
        var container = Get<UIItemSlotContainer>((int)Container.Contents);
        container.CreateItemSlots<UIInventorySlot>(prefab, inventory.maxCapacity);
        container.Init<UIInventorySlot>(inventory, ActivateItemUsageHelper);

        inventory.OnUpdated += container.OnItemSlotUpdated;

        gameObject.SetActive(false);
    }

    private void Start()
    {
        var helper = Get<UIItemUsageHelper>((int)Helper.UsageHelper);
        helper.BindEventOfButton(UIItemUsageHelper.Functions.Regist, RegistItem);
        helper.BindEventOfButton(UIItemUsageHelper.Functions.UnRegist, UnregistItem);
        helper.BindEventOfButton(UIItemUsageHelper.Functions.Use, ConsumeItem);
        helper.BindEventOfButton(UIItemUsageHelper.Functions.Destroy, DestryoItem);
    }

    private void ActivateItemUsageHelper(UIItemSlot itemslotUI)
    {   
        var inventoryslotui = itemslotUI as UIInventorySlot;
        SelectedSlot.Set(inventoryslotui.Index, Managers.Game.Player.Inventory.slots[inventoryslotui.Index]);
        var pos = new Vector3
            (
            inventoryslotui.transform.position.x + inventoryslotui.RectTransform.sizeDelta.x,
            inventoryslotui.transform.position.y
            );
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).ShowOption
            (
            SelectedSlot.itemSlot,
            pos)
            ;
    }

    private void DestryoItem()
    {
        Managers.Game.Player.Inventory.DestroyItemByIndex(SelectedSlot);
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).gameObject.SetActive(false);
    }

    private void ConsumeItem()
    {
        Managers.Game.Player.Inventory.UseConsumeItemByIndex(SelectedSlot.targetIndex);
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).gameObject.SetActive(false);
    }

    private void RegistItem()
    {
        Managers.UI.ShowPopupUI<UIToolRegister>().Set(SelectedSlot);
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).gameObject.SetActive(false);
    }

    private void UnregistItem()
    {
        Managers.Game.Player.QuickSlot.UnRegist(SelectedSlot);
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).gameObject.SetActive(false);
    }
}
