using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInventorySlot : UIItemSlot
{
    enum GameObjects
    {
        Equip,
        Regist,
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
    }

    public override void Clear()
    {
        base.Clear();
        Get<GameObject>((int)GameObjects.Equip).SetActive(false);
        Get<GameObject>((int)GameObjects.Regist).SetActive(false);
    }
}
