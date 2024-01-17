using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 2024. 01. 17 Byun Jeongmin
public class UICookingConfirm : UIPopup
{
    enum Gameobjects
    {
        Contents,
        Character,
        Exit,
        YesButton,
        NoButton
    }

    [SerializeField] private GameObject _ingredientPrefab;

    private int _selectedIndex = -1;

    private Transform _contents;
    private Transform _character;

    private GameObject _yesButton;
    private GameObject _noButton;

    // 재료 UI를 저장할 리스트
    private List<GameObject> _ingredientUIList = new List<GameObject>();

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(Gameobjects));
        Get<GameObject>((int)Gameobjects.Exit).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });

        _yesButton = Get<GameObject>((int)Gameobjects.YesButton);
        _noButton = Get<GameObject>((int)Gameobjects.NoButton);

        _yesButton.BindEvent((x) => { OnCookingConfirmed(); });
        _noButton.BindEvent((x) => { OnCookingCanceled(); });
    }

    private void Awake()
    {
        Initialize();
        _contents = Get<GameObject>((int)Gameobjects.Contents).transform;
        _character = Get<GameObject>((int)Gameobjects.Character).transform;

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        var cookingDataList = Managers.Data.cookingDataList;
    }

    private void OnCookingConfirmed()
    {
        if (_selectedIndex != -1)
        {
            List<RecipeSO.Ingredient> ingredients = Managers.Data.cookingDataList[_selectedIndex].requiredItems;

            // 플레이어 인벤토리에서 재료 확인 및 소모
            if (Managers.Game.Player.Cooking.CheckIngredients(ingredients))
            {
                if (Managers.Game.Player.Inventory.IsFull())
                {
                    Debug.Log("인벤토리가 가득 찼습니다. 음식을 추가할 수 없습니다.");
                }
                else
                {
                    ItemData completedItemData = Managers.Data.cookingDataList[_selectedIndex].completedItemData;
                    Managers.Game.Player.Inventory.AddItem(completedItemData, 1);
                    //Debug.Log("현재 선택한 인덱스 번호는 " + _selectedIndex);
                    Debug.Log($"{completedItemData.displayName}을 요리했어요.");

                    Managers.Game.Player.Cooking.ConsumeIngredients(ingredients);
                }
            }
            else
            {
                Debug.Log("재료가 부족합니다.");
            }
        }
        ClearIngredients();

        gameObject.SetActive(false);
    }

    private void OnCookingCanceled()
    {
        Debug.Log("제작 취소");

        // 아니오 버튼이 눌렸을 때
        gameObject.SetActive(false);
    }

    public void SetIngredients(List<RecipeSO.Ingredient> ingredients, int index) // 매개변수로 인덱스 추가
    {
        // 기존 재료 UI 초기화
        ClearIngredients();

        _selectedIndex = index;

        // 필요한 재료 목록을 순회하면서 UI에 추가
        foreach (var ingredient in ingredients)
        {
            GameObject ingredientUI = Instantiate(_ingredientPrefab, _contents);
            Image ingredientIcon = ingredientUI.transform.Find("Icon").GetComponent<Image>();
            TextMeshProUGUI ingredientQuantity = ingredientUI.GetComponentInChildren<TextMeshProUGUI>();

            // 재료 아이콘 설정
            ingredientIcon.sprite = ingredient.item.iconSprite;

            // 필요 수량 설정
            int requiredQuantity = ingredient.quantity;
            ingredientQuantity.text = requiredQuantity.ToString();

            // 플레이어 인벤토리에서 해당 아이템의 수량 확인
            int availableQuantity = Managers.Game.Player.Inventory.GetItemCount(ingredient.item);

            // 수량에 따라 텍스트 색상 변경(초록/빨강)
            if (availableQuantity >= requiredQuantity)
            {
                ingredientQuantity.color = new Color32(0, 200, 50, 255); // 초록색
            }
            else
            {
                ingredientQuantity.color = new Color32(222, 35, 0, 255); // 빨간색
            }

            _ingredientUIList.Add(ingredientUI);
        }
    }


    private void ClearIngredients()
    {
        foreach (GameObject ingredientUI in _ingredientUIList)
        {
            Destroy(ingredientUI);
        }
        _ingredientUIList.Clear();
    }
}
