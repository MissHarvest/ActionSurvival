using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 18 Byun Jeongmin
// 레시피 UI 기능을 포함하는 공통 클래스
public abstract class UICraftBase : UIPopup
{
    protected enum Gameobjects
    {
        Contents,
        Exit,
    }

    protected Transform _contents;
    protected List<UIRecipeItemSlot> _uiRecipeSlots = new List<UIRecipeItemSlot>();

    protected abstract UIConfirmBase GetConfirmPopup();
    protected abstract List<RecipeSO> GetDataList();

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(Gameobjects));
        Get<GameObject>((int)Gameobjects.Exit).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });
    }

    public virtual void Awake()
    {
        Initialize();
        _contents = Get<GameObject>((int)Gameobjects.Contents).transform;

        // 비활성화 상태에서 시작
        gameObject.SetActive(false);
    }

    public virtual void OnEnable()
    {
        ShowData(GetDataList());
    }

    // 제작할 데이터를 표시하는 메서드
    protected virtual void ShowData(List<RecipeSO> dataList)
    {
        ClearSlots();

        for (int i = 0; i < dataList.Count; i++)
        {
            var recipeSlotPrefab = Managers.Resource.GetCache<GameObject>("UIRecipeItemSlot.prefab");
            var recipeSlotGO = Instantiate(recipeSlotPrefab, _contents);
            var recipeSlot = recipeSlotGO.GetComponent<UIRecipeItemSlot>();
            recipeSlot?.Set(new ItemSlot(dataList[i].completedItemData));

            // UIRecipeItemSlot에 인덱스를 설정
            recipeSlot.SetIndex(i);

            // 아이템 클릭 시 Confirm 판넬 띄우기
            recipeSlotGO.BindEvent((x) =>
            {
                if (recipeSlotGO.activeSelf)
                {
                    var confirmPopup = GetConfirmPopup();

                    // 선택한 레시피의 재료를 가져와서 Confirm에 전달
                    confirmPopup.SetIngredients(dataList[recipeSlot.Index].requiredItems, recipeSlot.Index);

                    gameObject.SetActive(false);
                }
            });

            _uiRecipeSlots.Add(recipeSlot);
        }
    }

    private void ClearSlots()
    {
        foreach (var slot in _uiRecipeSlots)
        {
            Destroy(slot.gameObject);
        }
        _uiRecipeSlots.Clear();
    }
}
