using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//Lee gyuseong 24.02.24

public class UICookingSlot : UIBase
{
    enum Images
    {
        IngredientsIcon,
        RequiredIcon
    }

    enum Texts
    {
        Text
    }

    private int _ingredientsQuantity;
    private int _requiredQuantity;

    public override void Initialize()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        Get<Image>((int)Images.IngredientsIcon).raycastTarget = false;
        Get<Image>((int)Images.RequiredIcon).raycastTarget = false;
        Get<TextMeshProUGUI>((int)Texts.Text).raycastTarget = false;
    }
}
