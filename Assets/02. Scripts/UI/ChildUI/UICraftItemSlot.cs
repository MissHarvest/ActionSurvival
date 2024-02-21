using TMPro;
using UnityEngine.UI;

// 2024. 02. 06 Byun Jeongmin
public class UICraftItemSlot : UIBase
{
    enum Images
    {
        Icon,
    }
    enum Texts
    {
        ItemName,
    }


    private int _index;
    private int _quantity;

    public int Index
    {
        get { return _index; }
        private set { _index = value; }
    }

    public int Quantity // 완성 아이템 제작 수량
    {
        get { return _quantity; }
        private set { _quantity = value; }
    }

    protected Image Icon => Get<Image>((int)Images.Icon);

    public override void Initialize()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        Get<Image>((int)Images.Icon).raycastTarget = false;
        Get<TextMeshProUGUI>((int)Texts.ItemName).raycastTarget = false;
    }

    private void Awake()
    {
        Initialize();
    }

    public void SetIndex(int index)
    {
        _index = index;
    }

    //public void SetQuantity(int quantity)
    //{
    //    _quantity = quantity;
    //}

    public virtual void Set(ItemData itemData, RecipeSO recipeSO)
    {
        if (itemData == null)
        {
            Clear();
            return;
        }

        Get<Image>((int)Images.Icon).sprite = itemData.iconSprite;

        if (recipeSO.Quantity > 1)
            Get<TextMeshProUGUI>((int)Texts.ItemName).text = $"{itemData.displayName}({recipeSO.Quantity}개)";
        else
            Get<TextMeshProUGUI>((int)Texts.ItemName).text = itemData.displayName;

        Get<Image>((int)Images.Icon).gameObject.SetActive(true);
        Get<TextMeshProUGUI>((int)Texts.ItemName).gameObject.SetActive(true);

        _quantity = recipeSO.Quantity;
    }

    public virtual void Clear()
    {
        Get<Image>((int)Images.Icon).gameObject.SetActive(false);
        Get<TextMeshProUGUI>((int)Texts.ItemName).gameObject.SetActive(false);
    }
}
