using UnityEngine.UI;

public class UIRecipeItemSlot : UIBase
{
    enum Images
    {
        Icon,
    }

    private int _index;

    public int Index
    {
        get { return _index; }
        private set { _index = value; }
    }

    protected Image Icon => Get<Image>((int)Images.Icon);

    public override void Initialize()
    {
        Bind<Image>(typeof(Images));

        Get<Image>((int)Images.Icon).raycastTarget = false;
        Clear();
    }

    private void Awake()
    {
        Initialize();

        //// 요리 클릭 시 UICookingConfirm 판넬 띄움
        //gameObject.BindEvent((x) =>
        //{
        //    if (Icon.gameObject.activeSelf)
        //    {  
        //        var cookingConfirmPopup = Managers.UI.ShowPopupUI<UICookingConfirm>();
        //        int index = GetIndex(); // 선택한 UIRecipeItemSlot의 인덱스 가져오기
        //        _index = index;
        //        // 선택한 레시피의 재료를 가져와서 UICookingConfirm에 전달
        //        cookingConfirmPopup.SetIngredients(Managers.Data.cookingDataList[index].requiredItems);
                
        //        var cookingPanel = Managers.UI.FindPopupUI<UICooking>();
        //        cookingPanel?.gameObject.SetActive(false);
        //    }
        //});
    }

    public void SetIndex(int index)
    {
        _index = index;
    }

    public int GetIndex()
    {
        return _index;
    }

    public virtual void Set(ItemSlot itemSlot)
    {
        if (itemSlot.itemData == null)
        {
            Clear();
            return;
        }

        Get<Image>((int)Images.Icon).sprite = itemSlot.itemData.iconSprite;
        Get<Image>((int)Images.Icon).gameObject.SetActive(true);
    }

    public virtual void Clear()
    {
        Get<Image>((int)Images.Icon).gameObject.SetActive(false);
    }
}
