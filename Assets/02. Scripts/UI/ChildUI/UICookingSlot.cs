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
        CookedFoodItemIcon
    }

    enum Quantity
    {
        RequiredQuantity,
        CookedFoodItemQuantity
    }

    enum Texts
    {
        Text
    }

    private TextMeshProUGUI _requiredQuantity;
    public TextMeshProUGUI cookedFoodItemQuantity;
    private TextMeshProUGUI _text;
    private Slider _timeTakenToCookSlider;
    public int maxTimeRequiredToCook;
    public int index;//인덱스를 넘겨야겠네
    public int cookingLevel = 0;

    public override void Initialize()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Quantity));
        Bind<TextMeshProUGUI>(typeof(Texts));

        Get<Image>((int)Images.CookedFoodItemIcon).raycastTarget = false;

        _requiredQuantity = Get<TextMeshProUGUI>((int)Quantity.RequiredQuantity);
        cookedFoodItemQuantity = Get<TextMeshProUGUI>((int)Quantity.CookedFoodItemQuantity);
        _text = Get<TextMeshProUGUI>((int)Texts.Text);

        _requiredQuantity.raycastTarget = false;
        cookedFoodItemQuantity.raycastTarget = false;
        _text.raycastTarget = false;

        _timeTakenToCookSlider = GetComponentInChildren<Slider>();
    }

    private void Awake()
    {
        Initialize();
    }

    public void Set(ItemSlot itemSlot)
    {
        if (itemSlot.itemData == null)
        {
            _requiredQuantity.text = "0";
            return;
        }

        Get<Image>((int)Images.CookedFoodItemIcon).sprite = itemSlot.itemData.iconSprite;

        _requiredQuantity.text = itemSlot.quantity.ToString();
    }

    public void CookedFoodItemQuantitySet(ItemSlot itemSlot)//완성 아이템 배열의 데이터를 가지고 와서 업데이트
    {
        if (itemSlot.itemData == null) cookedFoodItemQuantity.text = "0";
        cookedFoodItemQuantity.text = itemSlot.quantity.ToString();
    }

    public void SetMaxTimeTakenToCookSlider(int maxTimeRequiredToCook)
    {
        _timeTakenToCookSlider.maxValue = maxTimeRequiredToCook;
    }

    public void UpdatedTimeTakenToCookSlider(int currentTime)
    {
        _timeTakenToCookSlider.value = currentTime;
    }
}
