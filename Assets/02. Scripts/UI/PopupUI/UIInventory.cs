//using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
    private Canvas _canvas;

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(Gameobjects));
        Bind<UIItemSlotContainer>(typeof(Container));
        Bind<UIItemUsageHelper>(typeof(Helper));
        Get<GameObject>((int)Gameobjects.Exit).BindEvent((x) => { Managers.UI.CloseAllPopupUI(); });
    }

    private void Awake()
    {
        Debug.Log($"UI Inventory Awake [{gameObject.name}] [{this.name}]");
        Initialize();
        var inventory = Managers.Game.Player.Inventory;
        var prefab = Managers.Resource.GetCache<GameObject>("UIInventorySlot.prefab");
        var container = Get<UIItemSlotContainer>((int)Container.Contents);
        container.CreateItemSlots<UIInventorySlot>(prefab, inventory.maxCapacity);
        container.Init<UIInventorySlot>(inventory, ActivateItemUsageHelper);

        _canvas = GetComponent<Canvas>();

        inventory.OnUpdated += container.OnItemSlotUpdated;

        gameObject.SetActive(false);
    }

    private void Start()
    {
        var helper = Get<UIItemUsageHelper>((int)Helper.UsageHelper);
        helper.BindEventOfButton(UIItemUsageHelper.Functions.Regist, RegistItem);
        helper.BindEventOfButton(UIItemUsageHelper.Functions.UnRegist, UnregistItem);
        helper.BindEventOfButton(UIItemUsageHelper.Functions.Use, ConsumeItem);
        helper.BindEventOfButton(UIItemUsageHelper.Functions.Build, BuildItem);
        helper.BindEventOfButton(UIItemUsageHelper.Functions.Destroy, DestroyItem);
        helper.BindEventOfButton(UIItemUsageHelper.Functions.Equip, Equip);
        helper.BindEventOfButton(UIItemUsageHelper.Functions.UnEquip, UnEquip);
    }

    private void ActivateItemUsageHelper(UIItemSlot itemslotUI)
    {   
        var inventoryslotui = itemslotUI as UIInventorySlot;
        SelectedSlot.Set(inventoryslotui.Index, Managers.Game.Player.Inventory.slots[inventoryslotui.Index]);

        var offset = inventoryslotui.RectTransform.sizeDelta.x;
        offset = offset / 1920 * _canvas.renderingDisplaySize.x;

        var pos = new Vector3
            (
            inventoryslotui.transform.position.x + offset,
            inventoryslotui.transform.position.y
            );
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).ShowOption
            (
            SelectedSlot.itemSlot,
            pos)
            ;
    }

    private void DestroyItem()
    {
        Managers.Game.Player.Inventory.DestroyItemByIndex(SelectedSlot);
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).gameObject.SetActive(false);
    }

    private void ConsumeItem()
    {
        Managers.Game.Player.Inventory.UseConsumeItemByIndex(SelectedSlot.targetIndex);
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).gameObject.SetActive(false);
    }

    private void BuildItem()
    {
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).gameObject.SetActive(false);
        Managers.UI.ClosePopupUI(this); //UIInventory 팝업 끄기
        Managers.Game.Player.Building.CreateArchitectureByIndex(SelectedSlot.targetIndex);
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

    private void Equip()
    {
        Managers.Game.Player.ToolSystem.Equip(SelectedSlot);
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).gameObject.SetActive(false);
    }

    private void UnEquip()
    {
        Managers.Game.Player.ToolSystem.UnEquip(SelectedSlot);
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).gameObject.SetActive(false);
    }

    public void HighLightItemSlot(int index)
    {
        Get<UIItemSlotContainer>((int)Container.Contents).HighLight(index);        
    }

    public void HighLightHelper(UIItemUsageHelper.Functions function)
    {
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).HighLight(function);
    }
}
