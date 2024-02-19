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
    }

    public void SetIndex(int index)
    {
        _index = index;
    }

    public virtual void Set(ItemSlot_Class itemSlot)
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
