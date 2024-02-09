using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 2024. 01. 18 Byun Jeongmin
// 레시피 UI 기능을 포함하는 공통 클래스
public abstract class UICraftBase : UIPopup
{
    protected enum Gameobjects
    {
        Content,
        Confirm,
        Contents,
        YesButton,
        MinusButton,
        PlusButton,
        Exit,
    }

    enum Texts
    {
        AskingText,
        QuantityText,
    }

    protected GameObject _itemPrefab;
    protected int _selectedIndex = -1;

    protected Transform _content;
    protected Transform _confirm;
    protected Transform _contents;

    protected GameObject _yesButton;
    protected GameObject _minusButton;
    protected GameObject _plusButton;

    protected List<GameObject> _itemUIList = new List<GameObject>();

    protected int _craftQuantity = 1;

    protected virtual List<RecipeSO.Ingredient> GetRequiredDataList() => null;
    protected virtual List<RecipeSO> GetDataList() => null;

    protected List<UICraftItemSlot> _uiCraftSlots = new List<UICraftItemSlot>();

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(Gameobjects));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Get<GameObject>((int)Gameobjects.Exit).BindEvent((x) =>
        {
            _confirm.gameObject.SetActive(false);
            Managers.UI.ClosePopupUI(this);
        });

        Get<TextMeshProUGUI>((int)Texts.AskingText).raycastTarget = false;
        Get<TextMeshProUGUI>((int)Texts.QuantityText).raycastTarget = false;

        _yesButton = Get<GameObject>((int)Gameobjects.YesButton);
        _minusButton = Get<GameObject>((int)Gameobjects.MinusButton);
        _plusButton = Get<GameObject>((int)Gameobjects.PlusButton);
        _yesButton.BindEvent((x) => { OnConfirmedBase(); });
        _minusButton.BindEvent((x) => { OnMinusQuantity(); });
        _plusButton.BindEvent((x) => { OnPlusQuantity(); });
    }

    public virtual void Awake()
    {
        Initialize();
        _content = Get<GameObject>((int)Gameobjects.Content).transform;
        _confirm = Get<GameObject>((int)Gameobjects.Confirm).transform;
        _contents = Get<GameObject>((int)Gameobjects.Contents).transform;

        // 비활성화 상태에서 시작
        _confirm.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public virtual void OnEnable()
    {
        _craftQuantity = 1;
        ShowData(GetDataList());
        var dataList = GetRequiredDataList();
    }

    // 제작할 데이터를 표시하는 메서드
    protected virtual void ShowData(List<RecipeSO> dataList)
    {
        ClearSlots();

        for (int i = 0; i < dataList.Count; i++)
        {
            var craftSlotPrefab = Managers.Resource.GetCache<GameObject>("UICraftItemSlot.prefab");
            var craftSlotGO = Instantiate(craftSlotPrefab, _content);
            var craftSlot = craftSlotGO.GetComponent<UICraftItemSlot>();
            craftSlot?.Set(new ItemSlot(dataList[i].completedItemData));

            // UIRecipeItemSlot에 인덱스 및 수량 설정
            craftSlot.SetIndex(i);
            craftSlot.SetQuantity(_craftQuantity);

            // 아이템 클릭 시 Confirm 판넬 띄우기
            craftSlotGO.BindEvent((x) =>
            {
                if (craftSlotGO.activeSelf)
                {
                    _confirm.gameObject.SetActive(true);
                    var craftItemName = dataList[craftSlot.Index].completedItemData.displayName;
                    Get<TextMeshProUGUI>((int)Texts.AskingText).text = $"{craftItemName}을(를) {craftSlot.Quantity}개\n제작하시겠습니까?";

                    // 선택한 레시피의 재료를 가져와서 Confirm에 전달
                    SetIngredients(dataList[craftSlot.Index].requiredItems, craftSlot.Index);

                    _craftQuantity = craftSlot.Quantity;
                    UpdateCraftUI();
                }
            });

            _uiCraftSlots.Add(craftSlot);
        }

        // 초기 수량 설정
        //UpdateCraftUI();
    }


    protected void OnConfirmedBase()
    {
        if (_selectedIndex != -1)
        {
            List<RecipeSO.Ingredient> items = GetDataList()[_selectedIndex].requiredItems;
            ItemData completedItemData = GetDataList()[_selectedIndex].completedItemData;

            // 플레이어 인벤토리에서 아이템 확인 및 소모
            if (CheckItems(items))
            {
                if (Managers.Game.Player.Inventory.IsFull(completedItemData, _craftQuantity))
                {
                    _confirm.gameObject.SetActive(false);
                    var warning = Managers.UI.ShowPopupUI<UIWarning>();
                    warning.SetWarning("인벤토리가 가득 찼습니다.");
                }
                else
                {
                    _confirm.gameObject.SetActive(false);
                    var confirm = Managers.UI.ShowPopupUI<UICraftConfirm>();
                    confirm.SetCraft($"{completedItemData.displayName} 제작 완료!");
                    Managers.Game.Player.Inventory.AddItem(completedItemData, _craftQuantity);
                    //Debug.Log($"{completedItemData.displayName}을 제작했어요.");

                    ConsumeItems(items);
                }
            }
            else
            {
                _confirm.gameObject.SetActive(false);
                var warning = Managers.UI.ShowPopupUI<UIWarning>();
                warning.SetWarning("재료가 부족합니다.");
            }
        }
        ClearItems();
    }

    protected void OnMinusQuantity()
    {
        if (_craftQuantity > 1)
        {
            _craftQuantity--;
            UpdateCraftUI();
        }
    }

    protected void OnPlusQuantity()
    {
        // 한 번에 20개까지만 제작 가능
        if (_craftQuantity < 20)
        {
            _craftQuantity++;
            UpdateCraftUI();
        }
    }

    private void UpdateCraftUI()
    {
        SetIngredients(GetDataList()[_selectedIndex].requiredItems, _selectedIndex);
        string craftItemName = GetDataList()[_selectedIndex].completedItemData.displayName;

        // UI에 수량 업데이트
        Get<TextMeshProUGUI>((int)Texts.AskingText).text = $"{craftItemName}을(를) {_craftQuantity}개\n제작하시겠습니까?";
        Get<TextMeshProUGUI>((int)Texts.QuantityText).text = _craftQuantity.ToString();
    }


    protected void SetIngredients(List<RecipeSO.Ingredient> items, int index)
    {
        // 기존 아이템 UI 초기화
        ClearItems();

        _selectedIndex = index;

        // 필요한 아이템 목록을 순회하면서 UI에 추가
        foreach (var item in items)
        {
            GameObject itemUI = Instantiate(_itemPrefab, _contents);
            Image itemIcon = itemUI.transform.Find("Icon").GetComponent<Image>();
            TextMeshProUGUI itemQuantity = itemUI.GetComponentInChildren<TextMeshProUGUI>();

            // 아이템 아이콘 설정
            itemIcon.sprite = item.item.iconSprite;

            // 필요 수량 설정
            int requiredQuantity = item.quantity * _craftQuantity;

            // 플레이어 인벤토리에서 해당 아이템의 수량 확인
            int availableQuantity = Managers.Game.Player.Inventory.GetItemCount(item.item);

            itemQuantity.text = (availableQuantity + "/" + requiredQuantity).ToString();

            // 수량에 따라 텍스트 색상 변경(초록/빨강)
            if (availableQuantity >= requiredQuantity)
            {
                itemQuantity.color = new Color32(0, 200, 50, 255); // 초록색
            }
            else
            {
                itemQuantity.color = new Color32(222, 35, 0, 255); // 빨간색
            }

            _itemUIList.Add(itemUI);
        }
    }

    protected bool CheckItems(List<RecipeSO.Ingredient> items)
    {
        foreach (var item in items)
        {
            int requiredQuantity = item.quantity * _craftQuantity;
            int availableQuantity = Managers.Game.Player.Inventory.GetItemCount(item.item);

            // 아이템이 부족하면 false 반환
            if (availableQuantity < requiredQuantity)
            {
                return false;
            }
        }
        return true;
    }

    protected void ConsumeItems(List<RecipeSO.Ingredient> items)
    {
        foreach (var item in items)
        {
            ItemData requiredItemData = item.item;
            int requiredCount = item.quantity * _craftQuantity;

            Managers.Game.Player.Inventory.RemoveItem(requiredItemData, requiredCount);
        }
    }

    private void ClearSlots()
    {
        foreach (var slot in _uiCraftSlots)
        {
            Destroy(slot.gameObject);
        }
        _uiCraftSlots.Clear();
    }

    private void ClearItems()
    {
        foreach (GameObject itemUI in _itemUIList)
        {
            Destroy(itemUI);
        }
        _itemUIList.Clear();
    }
}
