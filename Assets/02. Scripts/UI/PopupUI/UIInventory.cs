using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : UIPopup
{
    enum Gameobjects
    {
        Block,
    }

    enum Container
    {
        Contents
    }

    enum ArmorSlotContainer
    {
        ArmorSlotContainer
    }

    enum Helper
    {
        UsageHelper,
    }

    private int _selectedIndex;
    private Canvas _canvas;
    private ArmorSystem _armorSystem;
    private PlayerInventorySystem _playerInventory;

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(Gameobjects));
        Bind<UIItemSlotContainer>(typeof(Container));
        Bind<UIArmorSlotContainer>(typeof(ArmorSlotContainer));
        Bind<UIItemUsageHelper>(typeof(Helper));
        Get<GameObject>((int)Gameobjects.Block).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });
    }

    private void Awake()
    {
        Debug.Log($"UI Inventory Awake [{gameObject.name}] [{this.name}]");

        Initialize();

        _playerInventory = GameManager.Instance.Player.Inventory;
        _armorSystem = GameManager.Instance.Player.ArmorSystem;

        var prefab = Managers.Resource.GetCache<GameObject>("UIInventorySlot.prefab");
        var container = Get<UIItemSlotContainer>((int)Container.Contents);
        container.CreateItemSlots<UIInventorySlot>(prefab, _playerInventory.maxCapacity);
        container.Init<UIInventorySlot>(_playerInventory, ActivateItemUsageHelper);

        var armorSlotPrefab = Managers.Resource.GetCache<GameObject>("UIArmorSlot.prefab");
        var armorSlotContainer = Get<UIArmorSlotContainer>((int)ArmorSlotContainer.ArmorSlotContainer);
        armorSlotContainer.CreatArmorSlots<UIArmorSlot>(armorSlotPrefab, 2);
        armorSlotContainer.Init<UIArmorSlot>(_armorSystem.GetEquippedArmorsArray());

        _canvas = GetComponent<Canvas>();

        _playerInventory.OnUpdated += container.OnItemSlotUpdated;

        gameObject.SetActive(false);
    }

    private void Start()
    {
        var helper = Get<UIItemUsageHelper>((int)Helper.UsageHelper);
        helper.BindEventOfButton(UIItemUsageHelper.Functions.Regist, RegistItem);
        helper.BindEventOfButton(UIItemUsageHelper.Functions.UnRegist, UnregistItem);
        helper.BindEventOfButton(UIItemUsageHelper.Functions.Use, EatFood);//
        helper.BindEventOfButton(UIItemUsageHelper.Functions.Build, BuildItem);//
        helper.BindEventOfButton(UIItemUsageHelper.Functions.Destroy, DestroyItem);
        helper.BindEventOfButton(UIItemUsageHelper.Functions.Equip, Equip);//
        helper.BindEventOfButton(UIItemUsageHelper.Functions.UnEquip, UnEquip);//
    }

    private void ActivateItemUsageHelper(UIItemSlot itemSlotUI)
    {
        _selectedIndex = itemSlotUI.Index;

        var offset = itemSlotUI.RectTransform.sizeDelta.x;
        offset = offset / 1920 * _canvas.renderingDisplaySize.x;

        var pos = new Vector3
            (
            itemSlotUI.transform.position.x + offset,
            itemSlotUI.transform.position.y
            );
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).ShowOption
            (
            _playerInventory.Get(_selectedIndex),
            pos)
            ;        
    }

    private void DestroyItem()
    {
        _playerInventory.DestroyItem(_selectedIndex);
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).gameObject.SetActive(false);
    }

    private void EatFood()
    {
        _playerInventory.Owner.ItemUsageHelper.Use(_selectedIndex);
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).gameObject.SetActive(false);
    }

    private void BuildItem()
    {
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).gameObject.SetActive(false);
        Managers.UI.ClosePopupUI(this);
        _playerInventory.Owner.ItemUsageHelper.Use(_selectedIndex);
    }

    private void RegistItem()
    {
        Managers.UI.ShowPopupUI<UIToolRegister>().Set(_selectedIndex, _playerInventory.Get(_selectedIndex));
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).gameObject.SetActive(false);
    }

    private void UnregistItem()
    {
        _playerInventory.Owner.QuickSlot.UnRegist(_selectedIndex);
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).gameObject.SetActive(false);
    }

    private void Equip()
    {
        _playerInventory.Owner.ItemUsageHelper.Use(_selectedIndex);
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).gameObject.SetActive(false);
    }

    private void UnEquip()
    {
        ItemParts part = _playerInventory.GetPart(_selectedIndex);
        GameManager.Instance.Player.ArmorSystem.UnEquip(part);
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
