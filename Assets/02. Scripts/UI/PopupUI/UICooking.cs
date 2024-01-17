using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 16 Byun Jeongmin
public class UICooking : UIPopup
{
    enum Gameobjects
    {
        Contents,
        Exit,
    }

    private Transform _contents;
    private List<UIRecipeItemSlot> _uiRecipeSlots = new List<UIRecipeItemSlot>();

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(Gameobjects));
        Get<GameObject>((int)Gameobjects.Exit).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });
    }

    private void Awake()
    {
        Initialize();
        _contents = Get<GameObject>((int)Gameobjects.Contents).transform;
        
        // 비활성화 상태에서 시작
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ShowCookingData(Managers.Data.cookingDataList);
    }

    // CookingData를 표시하는 메서드
    public void ShowCookingData(List<RecipeSO> cookingDataList)
    {
        ClearRecipeSlots();

        for (int i = 0; i < cookingDataList.Count; i++)
        {
            var recipeSlotPrefab = Managers.Resource.GetCache<GameObject>("UIRecipeItemSlot.prefab");
            var recipeSlotGO = Instantiate(recipeSlotPrefab, _contents);
            var recipeSlot = recipeSlotGO.GetComponent<UIRecipeItemSlot>();
            recipeSlot?.Set(new ItemSlot(cookingDataList[i].completedItemData));

            // UIRecipeItemSlot에 인덱스를 설정
            recipeSlot.SetIndex(i);

            // 요리 클릭 시 UICookingConfirm 판넬 띄움
            recipeSlotGO.BindEvent((x) =>
            {
                if (recipeSlotGO.activeSelf)
                {
                    var cookingConfirmPopup = Managers.UI.ShowPopupUI<UICookingConfirm>();
                    //recipeSlot.Index = recipeSlot.GetIndex(); // 선택한 UIRecipeItemSlot의 인덱스 가져오기
                    //_index = index;
                    // 선택한 레시피의 재료를 가져와서 UICookingConfirm에 전달
                    cookingConfirmPopup.SetIngredients(Managers.Data.cookingDataList[recipeSlot.Index].requiredItems);
                    Debug.Log("재료 인덱스는 " + recipeSlot.Index);
                    var cookingPanel = Managers.UI.FindPopupUI<UICooking>();
                    cookingPanel?.gameObject.SetActive(false);
                }
            });

            _uiRecipeSlots.Add(recipeSlot);
        }
    }

    private void ClearRecipeSlots()
    {
        foreach (var slot in _uiRecipeSlots)
        {
            Destroy(slot.gameObject);
        }
        _uiRecipeSlots.Clear();
    }
}
