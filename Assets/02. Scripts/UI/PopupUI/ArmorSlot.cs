using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;
using static BossMonster;
// Lee gyuseong 24.02.06 save 기능 추가 필요

public class ArmorSlot : UIItemSlot
{
    enum Images
    {
        Icon,
    }

    public ItemParts part;

    public override void Initialize()
    {
        Bind<Image>(typeof(Images));
        Get<Image>((int)Images.Icon).gameObject.SetActive(false);
        Get<Image>((int)Images.Icon).raycastTarget = false;
    }

    private void Awake()
    {
        Initialize();
        Managers.Game.Player.ToolSystem.OnEquip += EquipArmor;
        Managers.Game.Player.ToolSystem.OnUnEquip += UnEquipArmor;
        Managers.Game.Player.ArmorSystem.UnEquipArmor += UnEquipArmor;

        Load();

        Managers.Game.OnSaveCallback += Save;
    }

    public void EquipArmor(QuickSlot quickSlot)
    {
        int parts = GetPart(quickSlot);

        switch (part)
        {
            case ItemParts.Head:
                if (parts == 0)
                    Set(quickSlot.itemSlot);
                break;
            case ItemParts.Body:
                if (parts == 1)
                    Set(quickSlot.itemSlot);
                break;
        }
    }
    public void UnEquipArmor(QuickSlot quickSlot)
    {
        int parts = GetPart(quickSlot);

        switch (part)
        {
            case ItemParts.Head:
                if (parts == 0)
                    Clear();
                break;
            case ItemParts.Body:
                if (parts == 1)
                    Clear();
                break;
        }
    }

    public override void Set(ItemSlot itemSlot)
    {
        Get<Image>((int)Images.Icon).sprite = itemSlot.itemData.iconSprite;
        Get<Image>((int)Images.Icon).gameObject.SetActive(true);
    }

    public override void Clear()
    {
        Get<Image>((int)Images.Icon).gameObject.SetActive(false);
    }

    private int GetPart(QuickSlot slot)
    {
        var itemData = slot.itemSlot.itemData as EquipItemData;
        if (itemData == null) return -1;
        return (int)itemData.part;
    }

    private void Load()
    {
        if (SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, "ArmorSlot"))
        {
            foreach (var itemData in Managers.Game.Player.ToolSystem.Equipments)
            {
                int parts = GetPart(itemData);
                switch (part)
                {
                    case ItemParts.Head:
                        if (parts == 0)
                            Set(itemData.itemSlot);
                        break;
                    case ItemParts.Body:
                        if (parts == 1)
                            Set(itemData.itemSlot);
                        break;
                }
            }         
        }
    }

    private void Save()
    {
        var json = JsonUtility.ToJson(this);
        SaveGame.CreateJsonFile("ArmorSlot", json, SaveGame.SaveType.Runtime);
    }
}
