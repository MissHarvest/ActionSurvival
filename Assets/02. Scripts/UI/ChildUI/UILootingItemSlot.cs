using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 2024. 01. 21 Park Jun Uk
public class UILootingItemSlot : UIItemSlot
{
    enum Texts
    {
        Name,
        Quantity,
    }

    public override void Initialize()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        base.Initialize();
    }

    public override void Clear()
    {
        base.Clear();
        Get<TextMeshProUGUI>((int)Texts.Name).gameObject.SetActive(false);
    }

    public override void Set(ItemSlot_Class itemSlot)
    {
        base.Set(itemSlot);
        Get<TextMeshProUGUI>((int)Texts.Name).text = itemSlot.itemData.displayName;
        Get<TextMeshProUGUI>((int)Texts.Quantity).text = itemSlot.quantity.ToString();
    }
}
