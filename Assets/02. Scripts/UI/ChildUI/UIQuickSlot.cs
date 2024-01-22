using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuickSlot : UIItemSlot
{
    public UIQuickSlotController UIQuickSlotController { get; private set; }
    public int index { get; private set; }

    private Slider durabilitySlider;

    public override void Initialize()
    {
        base.Initialize();
        durabilitySlider = GetComponentInChildren<Slider>();
    }

    public void Init(UIQuickSlotController quickSlotControllerUI, int index, ItemSlot itemSlot)
    {
        UIQuickSlotController = quickSlotControllerUI;
        this.index = index;
        Set(itemSlot);
    }

    private void Awake()
    {
        Initialize();

        gameObject.BindEvent((x) =>
        {
            Managers.Game.Player.QuickSlot.OnQuickUseInput(index);
        });
    }

    public override void Set(ItemSlot itemSlot)
    {
        if (itemSlot.itemData == null)
        {
            Clear();
            return;
        }

        base.Set(itemSlot);

        if (itemSlot.registed)
        {
            durabilitySlider.gameObject.SetActive(true);
            UpdateDurabilityUI(itemSlot);
        }
        else
        {
            durabilitySlider.gameObject.SetActive(false);
        }
    }


    public void UpdateDurabilityUI(ItemSlot itemSlot)
    {
        if (itemSlot.itemData is ToolItemData toolItem)
        {
            if (Managers.Game.Player.QuickSlot.slots[0].itemSlot.itemData != null)
            {
                durabilitySlider.gameObject.SetActive(true);

                float maxDurability = toolItem.maxDurability;
                float currentDurability = itemSlot.currentDurability;

                float durabilityPercentage = Mathf.Clamp01(currentDurability / maxDurability);
                Color durabilityColor = GetDurabilityColor(durabilityPercentage);

                durabilitySlider.value = durabilityPercentage;
                durabilitySlider.fillRect.GetComponent<Image>().color = durabilityColor;
            }
            else
            {
                durabilitySlider.gameObject.SetActive(false);
            }
        }
        else
        {
            // 내구도가 없는 아이템은 내구도 바 비활성화
            durabilitySlider.gameObject.SetActive(false);
        }
    }

    private Color GetDurabilityColor(float durabilityPercentage)
    {
        if (durabilityPercentage > 0.75f)
        {
            return Color.Lerp(Color.green, Color.yellow, (durabilityPercentage - 0.75f) * 1f);
        }
        else if (durabilityPercentage > 0.5f)
        {
            return Color.Lerp(Color.yellow, new Color(1f, 0.5f, 0f), (durabilityPercentage - 0.5f) * 1f);
        }
        else if (durabilityPercentage > 0.25f)
        {
            return Color.Lerp(new Color(1f, 0.5f, 0f), Color.red, (durabilityPercentage - 0.25f) * 1f);
        }
        else
        {
            return Color.red;
        }
    }

    public override void Clear()
    {
        base.Clear();
        durabilitySlider = GetComponentInChildren<Slider>();
        durabilitySlider.gameObject.SetActive(false);
    }
}
