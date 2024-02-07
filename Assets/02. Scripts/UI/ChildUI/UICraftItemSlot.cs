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

    public int Index
    {
        get { return _index; }
        private set { _index = value; }
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

    public virtual void Set(ItemSlot itemSlot)
    {
        if (itemSlot.itemData == null)
        {
            Clear();
            return;
        }

        Get<Image>((int)Images.Icon).sprite = itemSlot.itemData.iconSprite;
        Get<TextMeshProUGUI>((int)Texts.ItemName).text = itemSlot.itemData.displayName;
        Get<Image>((int)Images.Icon).gameObject.SetActive(true);
        Get<TextMeshProUGUI>((int)Texts.ItemName).gameObject.SetActive(true);
    }

    public virtual void Clear()
    {
        Get<Image>((int)Images.Icon).gameObject.SetActive(false);
        Get<TextMeshProUGUI>((int)Texts.ItemName).gameObject.SetActive(false);
    }
}
