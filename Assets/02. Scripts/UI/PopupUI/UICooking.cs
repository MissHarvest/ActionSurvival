using System.Collections.Generic;

// 2024. 01. 16 Byun Jeongmin
public class UICooking : UICraftBase
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnable()
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