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

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(Gameobjects));
        Bind<UIItemSlotContainer>(typeof(Container));
        //Bind<UIItemUsageHelper>(typeof(Helper));
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

    private void ActivateItemUsageHelper(UIItemSlot itemslotUI)
    {
        var helper = Managers.UI.ShowPopupUI<UIItemUsageHelper>();
        var inventoryslotui = itemslotUI as UIInventorySlot;
        var pos = new Vector3(inventoryslotui.transform.position.x + inventoryslotui.RectTransform.sizeDelta.x,
            inventoryslotui.transform.position.y);
        helper.ShowOption(inventoryslotui.Index, pos);
    }
}
