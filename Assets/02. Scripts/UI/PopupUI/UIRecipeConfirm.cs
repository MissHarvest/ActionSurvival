using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// 2024. 01. 11 Byun Jeongmin
public class UIRecipeConfirm : UIPopup
{
    enum Gameobjects
    {
        Contents,
        Character,
        Exit,
        YesButton,
        NoButton
    }

    [SerializeField] private GameObject ingredientPrefab;

    private Action confirmationAction;

    private Transform _contents;
    private Transform _character;

    private GameObject _yesButton;
    private GameObject _noButton;

    // 재료 UI를 저장할 리스트
    private List<GameObject> ingredientUIList = new List<GameObject>();

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(Gameobjects));
        Get<GameObject>((int)Gameobjects.Exit).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });

        _yesButton = Get<GameObject>((int)Gameobjects.YesButton);
        _noButton = Get<GameObject>((int)Gameobjects.NoButton);

        _yesButton.BindEvent((x) => { OnCraftConfirmed(); });
        _noButton.BindEvent((x) => { OnCraftCanceled(); });
    }

    private void Awake()
    {
        Initialize();
        _contents = Get<GameObject>((int)Gameobjects.Contents).transform;
        _character = Get<GameObject>((int)Gameobjects.Character).transform;
        gameObject.SetActive(false);
    }

    public void SetConfirmationAction(Action action)
    {
        confirmationAction = action;
    }

    private void OnCraftConfirmed()
    {
        // UIRecipe에서 설정한 함수를 실행
        confirmationAction?.Invoke();
        // 설정된 함수를 초기화
        confirmationAction = null;

        ClearIngredients();

        gameObject.SetActive(false);
    }

    private void OnCraftCanceled()
    {
        Debug.Log("제작 취소");

        // 아니오 버튼이 눌렸을 때
        gameObject.SetActive(false);
    }

    public void SetIngredients(Dictionary<ItemData, int> ingredients)
    {
        // 기존 재료 UI 초기화
        ClearIngredients();

        // 재료 아이템 프리팹을 이용하여 UI에 추가
        foreach (var ingredient in ingredients)
        {
            GameObject ingredientUI = Instantiate(ingredientPrefab, _contents);
            Image ingredientIcon = ingredientUI.transform.Find("Icon").GetComponent<Image>();
            TextMeshProUGUI ingredientQuantity = ingredientUI.GetComponentInChildren<TextMeshProUGUI>();

            ingredientIcon.sprite = ingredient.Key.iconSprite;
            ingredientQuantity.text = ingredient.Value.ToString();

            ingredientUIList.Add(ingredientUI);
        }
    }

    private void ClearIngredients()
    {
        foreach (GameObject ingredientUI in ingredientUIList)
        {
            Destroy(ingredientUI);
        }
        ingredientUIList.Clear();
    }
}
