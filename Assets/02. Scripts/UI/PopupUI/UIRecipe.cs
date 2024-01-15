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
        Stick
    }

    private Transform _contents;
    private Dictionary<Gameobjects, Action> recipeActions;

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(Gameobjects));
        Get<GameObject>((int)Gameobjects.Exit).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });

        //람다식 이용해서 액션 매핑
        recipeActions = new Dictionary<Gameobjects, Action>
        {
            { Gameobjects.PickAxe, () => Managers.Game.Player.Recipe.MakePickAxe() },
            { Gameobjects.Axe, () => Managers.Game.Player.Recipe.MakeAxe() },
            { Gameobjects.Sword, () => Managers.Game.Player.Recipe.MakeSword() },
            { Gameobjects.CraftingTable, () => Managers.Game.Player.Recipe.MakeCraftingTable() },
            { Gameobjects.Stick, () => Managers.Game.Player.Recipe.MakeStick() },
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

    private void OnUIElementClick(Gameobjects recipeObject)
    {
        // 확인 팝업 열기
        UIRecipeConfirm confirmPopup = Managers.UI.ShowPopupUI<UIRecipeConfirm>();
        confirmPopup.SetIngredients(Managers.Data.recipeData[recipeObject.ToString()]);

        // YesButton이 클릭될 때 실행될 함수
        confirmPopup.SetConfirmationAction(() =>
        {
            if (recipeActions.TryGetValue(recipeObject, out var action))
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
}
