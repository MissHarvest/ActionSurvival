using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : UIPopup
{
    enum Gameobjects
    {
        Exit,
    }

    enum Container //UIArmorSlotContainer용 enum을 만들자. 이건 기능으로 엮는 것이 아니라 class 별로 구분하는 것
    {
        Contents,
        ArmorSlotContainer
    }

    enum Helper
    {
        UsageHelper,
    }

    private int _selectedIndex;
    private Canvas _canvas;
    private InventorySystem _playerInventory;
    private ArmorSystem _armorSystem;
    private PlayerInventorySystem _playerInventory;

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(Gameobjects));
        Bind<UIItemSlotContainer>(typeof(Container));
        Bind<UIArmorSlotContainer>(typeof(Container));
        Bind<UIItemUsageHelper>(typeof(Helper));
        Get<GameObject>((int)Gameobjects.Exit).BindEvent((x) => { Managers.UI.CloseAllPopupUI(); });
    }

    private void Awake()
    {
        Debug.Log($"UI Inventory Awake [{gameObject.name}] [{this.name}]");

        Initialize();

        _playerInventory = Managers.Game.Player.Inventory;
        _armorSystem = Managers.Game.Player.ArmorSystem;

        var prefab = Managers.Resource.GetCache<GameObject>("UIInventorySlot.prefab");
        var container = Get<UIItemSlotContainer>((int)Container.Contents);
        container.CreateItemSlots<UIInventorySlot>(prefab, _playerInventory.maxCapacity);
        container.Init<UIInventorySlot>(_playerInventory, ActivateItemUsageHelper);

        var armorSlotPrefab = Managers.Resource.GetCache<GameObject>("UIArmorSlot.prefab");
        var armorSlotContainer = Get<UIArmorSlotContainer>((int)Container.ArmorSlotContainer);
        armorSlotContainer.CreatArmorSlots<UIArmorSlot>(armorSlotPrefab, 2);
        armorSlotContainer.Init<UIArmorSlot>(_armorSystem.equippedArmors);

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

    private void ActivateItemUsageHelper(UIItemSlot itemslotUI)
    {
        _selectedIndex = itemslotUI.Index;

        var offset = itemslotUI.RectTransform.sizeDelta.x;
        offset = offset / 1920 * _canvas.renderingDisplaySize.x;

        var pos = new Vector3
            (
            itemslotUI.transform.position.x + offset,
            itemslotUI.transform.position.y
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
        Managers.Game.Player.QuickSlot.UnRegist(_selectedIndex);
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).gameObject.SetActive(false);
    }

    private void Equip()
    {
        Managers.Game.Player.ArmorSystem.Equip(SelectedSlot);
        Get<UIItemUsageHelper>((int)Helper.UsageHelper).gameObject.SetActive(false);
    }

    private void UnEquip()
    {
        Managers.Game.Player.ArmorSystem.UnEquip(SelectedSlot);
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
