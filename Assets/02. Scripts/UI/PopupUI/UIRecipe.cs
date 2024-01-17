using System.Collections.Generic;
using System;
using UnityEngine;

// 2024. 01. 11 Byun Jeongmin
public class UIRecipe : UIPopup
{
    enum Gameobjects
    {
        Contents,
        Exit,
        PickAxe,
        Axe,
        Sword,
        CraftingTable,
        Stick,
        Greatsword, 
        BonFire
    }

    private Transform _contents;
    private Dictionary<Gameobjects, Action> _recipeActions;

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(Gameobjects));
        Get<GameObject>((int)Gameobjects.Exit).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });

        //람다식 이용해서 액션 매핑
        _recipeActions = new Dictionary<Gameobjects, Action>
        {
            { Gameobjects.PickAxe, () => {Managers.Game.Player.Recipe.MakeItem("PickAxe"); } },
            { Gameobjects.Axe, () => {Managers.Game.Player.Recipe.MakeItem("Axe"); } },
            { Gameobjects.Sword, () => {Managers.Game.Player.Recipe.MakeItem("Sword"); } },
            { Gameobjects.CraftingTable, () => {Managers.Game.Player.Recipe.MakeItem("CraftingTable"); } },
            { Gameobjects.Stick, () => {Managers.Game.Player.Recipe.MakeItem("Stick"); } },
            { Gameobjects.Greatsword, () => {Managers.Game.Player.Recipe.MakeItem("Greatsword"); } },
            { Gameobjects.BonFire, () => {Managers.Game.Player.Recipe.MakeItem("BonFire"); } },
        };
    }

    private void Awake()
    {
        Initialize();
        _contents = Get<GameObject>((int)Gameobjects.Contents).transform;

        foreach (Gameobjects recipeObject in Enum.GetValues(typeof(Gameobjects)))
        {
            if (recipeObject == Gameobjects.Contents || recipeObject == Gameobjects.Exit)
                continue;

            GameObject uiElement = Get<GameObject>((int)recipeObject);
            uiElement.BindEvent((x) => OnUIElementClick(recipeObject));
        }
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        string currentToolName = Managers.Game.Player.ToolSystem.GetToolName(Managers.Game.Player.ToolSystem.ItemInUse);

        // Greatsword 레시피 UI 활성화
        if (currentToolName == "Handable_CraftingTable")
        {
            SetGreatswordUIActive(true);
        }
        else
        {
            SetGreatswordUIActive(false);
        }
    }

    // 고급 레시피 UI의 활성화 여부를 설정하는 메서드
    private void SetGreatswordUIActive(bool active)
    {
        List<Gameobjects> greatswordUIElements = new List<Gameobjects>
    {
        Gameobjects.Greatsword,
        Gameobjects.BonFire
    };

        foreach (Gameobjects uiElement in greatswordUIElements)
        {
            GameObject uiObject = Get<GameObject>((int)uiElement);
            uiObject.SetActive(active);
        }
    }


    private void OnUIElementClick(Gameobjects recipeObject)
    {
        // 확인 팝업 열기
        UIRecipeConfirm confirmPopup = Managers.UI.ShowPopupUI<UIRecipeConfirm>();

        // Managers.Data.recipeDataList에서 레시피를 찾아옴
        RecipeSO recipe = Managers.Data.recipeDataList.Find(r => r.itemName == recipeObject.ToString());

        if (recipe != null)
        {
            confirmPopup.SetIngredients(Managers.Game.Player.Recipe.ToDictionary(recipe.requiredItems));

            // YesButton이 클릭될 때 실행될 함수
            confirmPopup.SetConfirmationAction(() =>
            {
                if (_recipeActions.TryGetValue(recipeObject, out var action))
                {
                    action.Invoke();
                }
                else
                {
                    Debug.Log("문제가 발생해 제작에 실패했어요.");
                }
            });

            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError($"레시피를 찾을 수 없습니다: {recipeObject}");
        }
    }
}
