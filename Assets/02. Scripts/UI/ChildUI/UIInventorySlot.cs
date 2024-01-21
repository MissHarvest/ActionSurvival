using System;
using UnityEngine;
using UnityEngine.UI;

public class UIInventorySlot : UIItemSlot
{
    enum GameObjects
    {
        Equip,
        Regist,
        DurabilityBase,
        DurabilityBar,
    }

    public int index { get; private set; }
    public RectTransform RectTransform { get; private set; }
    public UIInventory UIInventory { get; private set; }

    public override void Initialize()
    {
        Bind<GameObject>(typeof(GameObjects));
        base.Initialize();

        RectTransform = GetComponent<RectTransform>();
    }

    private void Awake()
    {
        Initialize();

        gameObject.BindEvent((x) =>
        {
            if (Icon.gameObject.activeSelf)
            {
                var helper = Managers.UI.ShowPopupUI<UIItemUsageHelper>();
                helper.ShowOption(index, new Vector3(transform.position.x + RectTransform.sizeDelta.x, transform.position.y));
            }
        });
    }

    public void Init(UIInventory inventoryUI, int index, ItemSlot itemSlot)
    {
        UIInventory = inventoryUI;
        this.index = index;
        Set(itemSlot);
    }

    public override void Set(ItemSlot itemSlot)
    {
        base.Set(itemSlot);
        Get<GameObject>((int)GameObjects.Equip).SetActive(itemSlot.equipped);
        Get<GameObject>((int)GameObjects.Regist).SetActive(itemSlot.registed);

        UpdateDurabilityUI(itemSlot);
    }

    public void UpdateDurabilityUI(ItemSlot itemSlot)
    {
        GameObject durabilityBase = Get<GameObject>((int)GameObjects.DurabilityBase);
        GameObject durabilityBar = Get<GameObject>((int)GameObjects.DurabilityBar);

        if (itemSlot.itemData is ToolItemData toolItem)
        {
            durabilityBase.SetActive(true);
            durabilityBar.SetActive(true);

            Transform durabilityBarRectTransform = durabilityBar.GetComponent<Transform>();
            Image durabilityBarImage = durabilityBar.GetComponent<Image>();

            float maxDurability = toolItem.maxDurability;
            float currentDurability = itemSlot.currentDurability;

            float durabilityPercentage = Mathf.Clamp01(currentDurability / maxDurability);
            Color durabilityColor = GetDurabilityColor(durabilityPercentage);

            durabilityBarImage.color = durabilityColor;

            durabilityBarRectTransform.localScale = new Vector3(1f, durabilityPercentage, 1f);
        }
        else
        {
            // 내구도가 없는 아이템은 내구도 바 비활성화
            durabilityBase.SetActive(false);
            durabilityBar.SetActive(false);
        }
    }

    private Color GetDurabilityColor(float durabilityPercentage)
    {
        if (durabilityPercentage > 0.5f)
        {
            return Color.Lerp(Color.green, Color.yellow, (durabilityPercentage - 0.5f) * 2f);
        }
        else if (durabilityPercentage > 0.25f)
        {
            return Color.Lerp(Color.yellow, new Color(1f, 0.5f, 0f), (durabilityPercentage - 0.25f) * 4f);
        }
        else
        {
            return Color.Lerp(new Color(1f, 0.5f, 0f), Color.red, durabilityPercentage * 4f);
        }
    }

    public override void Clear()
    {
        base.Clear();
        Get<GameObject>((int)GameObjects.Equip).SetActive(false);
        Get<GameObject>((int)GameObjects.Regist).SetActive(false);
        Get<GameObject>((int)GameObjects.DurabilityBase).SetActive(false);
        Get<GameObject>((int)GameObjects.DurabilityBar).SetActive(false);
    }
}
