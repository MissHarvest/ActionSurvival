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
        Block,
        Content,
        Confirm,
        Contents,
        YesButton,
        MinusButton,
        PlusButton,
    }

    enum Texts
    {
        AskingText,
        QuantityText,
    }

    protected int _selectedIndex = -1;

    protected Transform _content;
    protected Transform _confirm;
    protected Transform _contents;

    protected GameObject _yesButton;
    protected GameObject _minusButton;
    protected GameObject _plusButton;

    protected List<RecipeSO> _recipeOrCookingList;
    protected List<GameObject> _itemUIList = new List<GameObject>();
    protected List<UICraftItemSlot> _uiCraftSlots = new List<UICraftItemSlot>();

    protected CraftBase _craftBase;

    protected RecipeSO SelectedRecipe
    {
        get
        {
            if (_selectedIndex != -1 && _selectedIndex < _recipeOrCookingList.Count)
                return _recipeOrCookingList[_selectedIndex];
            else
                return null;
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(Gameobjects));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Get<GameObject>((int)Gameobjects.Block).BindEvent((x) =>
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
        _minusButton.BindEvent((x) => { _craftBase.OnMinusQuantity(); });
        _plusButton.BindEvent((x) => { _craftBase.OnPlusQuantity(); });
    }

    public virtual void Awake()
    {
        Initialize();
        _content = Get<GameObject>((int)Gameobjects.Content).transform;
        _confirm = Get<GameObject>((int)Gameobjects.Confirm).transform;
        _contents = Get<GameObject>((int)Gameobjects.Contents).transform;

        GetData();

        _craftBase.OnCountChanged += OnCraftBaseCountChanged;

        // 비활성화 상태에서 시작
        _confirm.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public virtual void OnEnable()
    {
        ShowData(_recipeOrCookingList);
    }

    protected abstract void GetData();

    protected abstract string GetConfirmationText(string displayName, int itemQuantity, int craftCount);

    private void OnCraftBaseCountChanged(int count)
    {
        UpdateCraftUI();
    }

    // 제작할 데이터를 표시하는 메서드
    protected virtual void ShowData(List<RecipeSO> dataList)
    {
        // 최초 한 번만 Instantiate
        if (_uiCraftSlots.Count == 0)
        {
            for (int i = 0; i < dataList.Count; i++)
            {
                var craftSlotPrefab = Managers.Resource.GetCache<GameObject>("UICraftItemSlot.prefab");
                var craftSlotGO = Instantiate(craftSlotPrefab, _content);
                var craftSlot = craftSlotGO.GetComponent<UICraftItemSlot>();

                // UIRecipeItemSlot에 인덱스 및 수량 설정
                craftSlot.SetIndex(i);
                craftSlot?.Set(dataList[i].completedItemData, dataList[i]);

                // 아이템 클릭 시 Confirm 띄우기
                craftSlotGO.GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (craftSlotGO.activeSelf)
                    {
                        _confirm.gameObject.SetActive(true);
                        _craftBase.InitializeCount();
                        var craftItemName = dataList[craftSlot.Index].completedItemData.displayName;
                        var craftItemQuantity = dataList[craftSlot.Index].Quantity;
                        Get<TextMeshProUGUI>((int)Texts.AskingText).text = GetConfirmationText(craftItemName, craftItemQuantity, _craftBase.Count);

                        // 선택한 레시피의 재료를 가져와서 Confirm에 전달
                        SetIngredients(dataList[craftSlot.Index].requiredItems, craftSlot.Index);
                        _craftBase.SetMaxCount(_craftBase.GetMaxCraftableCount(dataList[craftSlot.Index].requiredItems));
                        UpdateCraftUI();
                    }
                });

                _uiCraftSlots.Add(craftSlot);
            }
        }
        else
        {
            // 이미 생성된 경우에는 활성화
            for (int i = 0; i < dataList.Count; i++)
            {
                _uiCraftSlots[i].gameObject.SetActive(true);
                _uiCraftSlots[i].Set(dataList[i].completedItemData, dataList[i]);
            }
        }
    }

    protected virtual void OnConfirmedBase()
    {
        if (SelectedRecipe != null)
        {
            List<RecipeSO.Ingredient> items = SelectedRecipe.requiredItems;
            ItemData completedItemData = SelectedRecipe.completedItemData;

            // 플레이어 인벤토리에서 아이템 확인 및 소모
            if (_craftBase.CheckItems(items)) 
            {
                _craftBase.ConsumeItems(items);
                int totalQuantity = _craftBase.Count * SelectedRecipe.Quantity;
                if (GameManager.Instance.Player.Inventory.TryAddItem(completedItemData, totalQuantity))
                {
                    var confirm = Managers.UI.ShowPopupUI<UICraftConfirm>();
                    confirm.SetCraft($"{completedItemData.displayName} 제작 완료!");
                }
                else // 재료가 충분하고 소모도 했는데 아이템 들어갈 공간이 없을 경우 재료를 돌려줘야함
                {
                    _craftBase.AddItems(items);
                }
            }
            else
            {
                Managers.UI.ShowPopupUI<UIWarning>().SetWarning("아이템 수량이 부족합니다.");
            }

            _confirm.gameObject.SetActive(false);
            _craftBase.InitializeCount();
            return;
        }
    }

    public void UpdateCraftUI()
    {
        if (SelectedRecipe != null)
        {
            SetIngredients(SelectedRecipe.requiredItems, _selectedIndex);
            var craftItemName = SelectedRecipe.completedItemData.displayName;
            var initialQuantity = SelectedRecipe.Quantity;

            //// UI에 수량 업데이트
            Get<TextMeshProUGUI>((int)Texts.AskingText).text = GetConfirmationText(craftItemName, initialQuantity, _craftBase.Count);
            Get<TextMeshProUGUI>((int)Texts.QuantityText).text = _craftBase.Count.ToString();
        }
    }

    protected void SetIngredientUI(RecipeSO.Ingredient item, int quantity, int count, GameObject itemUI)
    {
        Image itemIcon = itemUI.transform.Find("Icon").GetComponent<Image>();
        TextMeshProUGUI itemQuantity = itemUI.GetComponentInChildren<TextMeshProUGUI>();
        itemIcon.sprite = item.item.iconSprite;

        int requiredQuantity = quantity * count;
        int availableQuantity = GameManager.Instance.Player.Inventory.GetItemCount(item.item);

        itemQuantity.text = (availableQuantity + "/" + requiredQuantity).ToString();

        // 수량에 따라 텍스트 색상 변경
        if (availableQuantity >= requiredQuantity)
            itemQuantity.color = new Color32(0, 200, 50, 255); // 초록색
        else
            itemQuantity.color = new Color32(222, 35, 0, 255); // 빨간색
    }

    protected void SetIngredients(List<RecipeSO.Ingredient> items, int index)
    {
        _selectedIndex = index;

        // 현재까지 생성된, 필요한 재료 UI의 개수
        int createdItemUICount = _itemUIList.Count;

        for (int i = 0; i < items.Count; i++)
        {
            GameObject itemUI;

            if (i < createdItemUICount)
            {
                itemUI = _itemUIList[i];
            }
            else // 기존에 생성된 UI가 부족하면 새로 생성
            {
                var recipeSlotPrefab = Managers.Resource.GetCache<GameObject>("UIRecipeSlot.prefab");
                itemUI = Instantiate(recipeSlotPrefab, _contents);
                _itemUIList.Add(itemUI);
            }

            SetIngredientUI(items[i], items[i].quantity, _craftBase.Count, itemUI);
            
            itemUI.SetActive(true);
        }

        // 필요한 재료 종류보다 UI가 많으면 비활성화
        for (int i = items.Count; i < createdItemUICount; i++)
        {
            _itemUIList[i].SetActive(false);
        }
    }

    public void SetAdvancedRecipeUIActive(int maxRecipeLevel)
    {
        foreach (var slot in _uiCraftSlots)
        {
            var recipe = _recipeOrCookingList[slot.Index];
            bool isAdvancedRecipe = recipe.recipeLevel <= maxRecipeLevel + 1;
            slot.gameObject.SetActive(isAdvancedRecipe);
        }
    }
}