using TMPro;

// 2024. 01. 21 Park Jun Uk
public class UILootingItemSlot : UIItemSlot
{
    enum Texts
    {
        Quantity,
        Name,
    }

    public override void Initialize()
    {
        base.Initialize();
        Bind<TextMeshProUGUI>(typeof(Texts));        
    }

    public override void Clear()
    {
        base.Clear();
        Get<TextMeshProUGUI>((int)Texts.Name).gameObject.SetActive(false);
    }

    public override void Set(ItemSlot itemSlot)
    {
        base.Set(itemSlot);
        if (itemSlot.itemData == null)
        {
            Clear();
            return;
        }

        Get<TextMeshProUGUI>((int)Texts.Name).text = itemSlot.itemData.displayName;
        Get<TextMeshProUGUI>((int)Texts.Quantity).text = itemSlot.quantity.ToString();
        Get<TextMeshProUGUI>((int)Texts.Quantity).gameObject.SetActive(true);
    }
}
