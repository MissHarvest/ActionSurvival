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

    enum Texts
    {
        RequiredQuantity,
        CookedFoodItemQuantity,
        Text
    }

    private TextMeshProUGUI _requiredQuantity;
    private TextMeshProUGUI _cookedFoodItemQuantity;
    private TextMeshProUGUI _text;
    private Image _icon;
    private Slider _timeTakenToCookSlider;
    public int index;

    public override void Initialize()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        _icon = Get<Image>((int)Images.CookedFoodItemIcon);
        _icon.gameObject.SetActive(false);

        Get<Image>((int)Images.CookedFoodItemIcon).raycastTarget = false;

        _requiredQuantity = Get<TextMeshProUGUI>((int)Texts.RequiredQuantity);
        _cookedFoodItemQuantity = Get<TextMeshProUGUI>((int)Texts.CookedFoodItemQuantity);
        _text = Get<TextMeshProUGUI>((int)Texts.Text);

        _requiredQuantity.raycastTarget = false;
        _cookedFoodItemQuantity.raycastTarget = false;
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
            _icon.gameObject.SetActive(false);
            _requiredQuantity.text = "0";
            return;
        }

        _icon.gameObject.SetActive(true);
        _icon.sprite = itemSlot.itemData.iconSprite;

        _requiredQuantity.text = itemSlot.quantity.ToString();        
    }

    public void SetCookedFoodIcon(ItemSlot itemSlot)
    {
        _icon.gameObject.SetActive(true);
        _icon.sprite = itemSlot.itemData.iconSprite;
    }

    public void SetDisableIcon()
    {
        _icon.gameObject.SetActive(false);
    }

    public void SetCookedFoodItemQuantity(ItemSlot itemSlot)
    {
        if (itemSlot.itemData == null)
        {
            _cookedFoodItemQuantity.text = "0";
            return;
        }
        _cookedFoodItemQuantity.text = itemSlot.quantity.ToString();
    }

    public void SetMaxTimeTakenToCookSlider(int maxTimeRequiredToCook)
    {
        _timeTakenToCookSlider.maxValue = maxTimeRequiredToCook;
    }

    public void UpdatedTimeTakenToCookSlider(int currentTime)
    {
        _timeTakenToCookSlider.value = currentTime;
    }

    public void SetText(string text)
    {
        _text.text = text;
    }
}
