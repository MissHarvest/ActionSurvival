using System.Collections.Generic;

// 2024. 01. 16 Byun Jeongmin
public class UICooking : UICraftBase
{
    private void Awake()
    {
        base.Awake();
    }

    protected void OnEnable()
    {
        base.OnEnable();
    }

    protected override UIConfirmBase GetConfirmPopup()
    {
        return Managers.UI.ShowPopupUI<UICookingConfirm>();
    }

    protected override List<RecipeSO> GetDataList()
    {
        return Managers.Data.cookingDataList;
    }
}