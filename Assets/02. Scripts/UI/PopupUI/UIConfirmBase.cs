using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 2024. 01. 18 Byun Jeongmin
// Confirm 팝업의 기본 클래스
public class UIConfirmBase : UIPopup
{
    protected enum Gameobjects
    {
        Contents,
        AskingText,
        Exit,
        YesButton,
        NoButton
    }

    protected GameObject _itemPrefab;
    protected int _selectedIndex = -1;
    protected Transform _contents;

    protected GameObject _yesButton;
    protected GameObject _noButton;
    protected List<GameObject> _itemUIList = new List<GameObject>();

    protected virtual List<RecipeSO.Ingredient> GetRequiredDataList() => null;
    protected virtual List<RecipeSO> GetDataList() => null;

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(Gameobjects));
        Get<GameObject>((int)Gameobjects.Exit).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });

        _yesButton = Get<GameObject>((int)Gameobjects.YesButton);
        _noButton = Get<GameObject>((int)Gameobjects.NoButton);

        _yesButton.BindEvent((x) => { OnConfirmedBase(); });
        _noButton.BindEvent((x) => { OnCanceledBase(); });
    }

    public virtual void Awake()
    {
        Initialize();
        _contents = Get<GameObject>((int)Gameobjects.Contents).transform;
        gameObject.SetActive(false);
    }

    public virtual void OnEnable()
    {
        var dataList = GetRequiredDataList();
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
                if (Managers.Game.Player.Inventory.IsFull(completedItemData))
                {
                    Managers.UI.ClosePopupUI(this);
                    var warning = Managers.UI.ShowPopupUI<UIWarning>();
                    warning.SetWarning("인벤토리가 가득 찼습니다.");
                }
                else
                {
                    // 제작 성공 시 팝업이 뜨면 좋을 듯?
                    Managers.Game.Player.Inventory.AddItem(completedItemData, 1);
                    Debug.Log($"{completedItemData.displayName}을 제작했어요.");

                    ConsumeItems(items);
                }
            }
            else
            {
                Managers.UI.ClosePopupUI(this);
                var warning = Managers.UI.ShowPopupUI<UIWarning>();
                warning.SetWarning("재료가 부족합니다.");
            }
        }
        ClearItems();
        Managers.UI.ClosePopupUI(this);
    }

    protected void OnCanceledBase()
    {
        Debug.Log("제작 취소");

        // 아니오 버튼을 눌렸을 때
        Managers.UI.ClosePopupUI(this);
    }

    public void SetIngredients(List<RecipeSO.Ingredient> items, int index)
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
            int requiredQuantity = item.quantity;
            itemQuantity.text = requiredQuantity.ToString();

            // 플레이어 인벤토리에서 해당 아이템의 수량 확인
            int availableQuantity = Managers.Game.Player.Inventory.GetItemCount(item.item);

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

    private void ClearItems()
    {
        foreach (GameObject itemUI in _itemUIList)
        {
            Destroy(itemUI);
        }
        _itemUIList.Clear();
    }

    protected bool CheckItems(List<RecipeSO.Ingredient> items)
    {
        foreach (var item in items)
        {
            int requiredQuantity = item.quantity;
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
            int requiredCount = item.quantity;

            Managers.Game.Player.Inventory.RemoveItem(requiredItemData, requiredCount);
        }
    }
}