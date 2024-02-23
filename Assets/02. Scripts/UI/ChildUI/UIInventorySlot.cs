using UnityEngine;
using UnityEngine.UI;

public class UIInventorySlot : UIItemSlot
{
    enum GameObjects
    {
        Equip,
        Regist,
    }


    private Slider _durabilitySlider;

    public override void Initialize()
    {
        Bind<GameObject>(typeof(GameObjects));
        base.Initialize();

        _durabilitySlider = GetComponentInChildren<Slider>();
    }

    private void Awake()
    {
        Initialize();
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
        if (itemSlot.itemData == null || itemSlot.itemData is ArchitectureItemData || itemSlot.itemData.MaxDurability == 0)
        {
            _durabilitySlider.gameObject.SetActive(false);
            return;
        }

        _durabilitySlider.gameObject.SetActive(true);

        float maxDurability = itemSlot.itemData.MaxDurability;
        float currentDurability = itemSlot.currentDurability;

        float durabilityPercentage = Mathf.Clamp01(currentDurability / maxDurability);
        Color durabilityColor = GetDurabilityColor(durabilityPercentage);

        _durabilitySlider.value = durabilityPercentage;
        _durabilitySlider.fillRect.GetComponent<Image>().color = durabilityColor;
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
        Get<GameObject>((int)GameObjects.Equip).SetActive(false);
        Get<GameObject>((int)GameObjects.Regist).SetActive(false);
    }
}
