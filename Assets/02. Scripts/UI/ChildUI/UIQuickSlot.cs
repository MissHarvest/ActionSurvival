using UnityEngine;
using UnityEngine.UI;

public class UIQuickSlot : UIItemSlot
{
    enum GameObjects
    {
        QuantityBackground,
        DurabilityCircle,
        ActivateMark
    }

    public UIQuickSlotController UIQuickSlotController { get; private set; }

    private GameObject durabilityCircle;

    public override void Initialize()
    {
        base.Initialize();
        BindObject(typeof(GameObjects));
        durabilityCircle = Get<GameObject>((int)GameObjects.DurabilityCircle);
    }

    public void Init(UIQuickSlotController quickSlotControllerUI, int index, ItemSlot_Class itemSlot)
    {
        Initialize();
        UIQuickSlotController = quickSlotControllerUI;
        BindGroup(null, index);
        Set(itemSlot);
    }

    public override void Set(ItemSlot_Class itemSlot)
    {
        if (itemSlot.itemData == null)
        {
            Clear();
            return;
        }

        Get<GameObject>((int)GameObjects.QuantityBackground).SetActive(itemSlot.itemData.stackable);
        Get<GameObject>((int)GameObjects.ActivateMark).SetActive(itemSlot.equipped);
        
        base.Set(itemSlot);
        UpdateDurabilityUI(itemSlot);
    }

    private void UpdateDurabilityUI(ItemSlot_Class itemSlot)
    {
        if (itemSlot.itemData is ToolItemData toolItem)
        {
            durabilityCircle.SetActive(true);

            Image _durabilityCircleImage = durabilityCircle.GetComponent<Image>();

            float maxDurability = toolItem.MaxDurability;
            float currentDurability = itemSlot.currentDurability;

            float durabilityPercentage = Mathf.Clamp01(currentDurability / maxDurability);
            Color durabilityColor = GetDurabilityColor(durabilityPercentage);

            _durabilityCircleImage.fillAmount = durabilityPercentage;
            _durabilityCircleImage.color = durabilityColor;
        }
        else
        {
            // 내구도가 없는 아이템은 내구도 바 비활성화
            durabilityCircle.SetActive(false);
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
        Get<GameObject>((int)GameObjects.ActivateMark).SetActive(false);
        Get<GameObject>((int)GameObjects.QuantityBackground).SetActive(false);
        durabilityCircle.SetActive(false);
    }
}
