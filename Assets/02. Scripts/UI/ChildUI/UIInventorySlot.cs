using UnityEngine;
using UnityEngine.UI;

public class UIInventorySlot : UIItemSlot
{
    enum GameObjects
    {
        Equip,
        Regist,
    }
        
    public RectTransform RectTransform { get; private set; }    

    private Slider durabilitySlider;

    public override void Initialize()
    {
        Bind<GameObject>(typeof(GameObjects));
        base.Initialize();

        RectTransform = GetComponent<RectTransform>();
        durabilitySlider = GetComponentInChildren<Slider>();
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
        if (itemSlot.itemData is EquipItemData toolItem) //lgs 24.02.06
        {
            if (itemSlot.itemData.displayName != "빈 손")
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
        Get<GameObject>((int)GameObjects.Equip).SetActive(false);
        Get<GameObject>((int)GameObjects.Regist).SetActive(false);
    }
}
