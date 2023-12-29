using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class UIItemUsageHelper : UIPopup
{
    enum GameObjects
    {
        Block,
        Options,        
    }

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(GameObjects));
        Get<GameObject>((int)GameObjects.Block).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });
    }

    private void Awake()
    {
        Initialize();
        gameObject.SetActive(false);
    }

    public void ShowOption(int index, Vector3 position)
    {
        Clear();

        var itemSlot = Managers.Game.Player.Inventory.slots[index];
        if (itemSlot == null) return;
        Get<GameObject>((int)GameObjects.Options).transform.position = position;

        // other Button Add
        switch(itemSlot.itemData)
        {
            case ToolItemData _:
                if(itemSlot.bUse)
                {
                    CreateUnEquipButton(index);
                }
                else
                {
                    CreateEquipButton(index);
                }
                break;
        }
        CreateDetroyButton(index);

        // Set My Size

    }

    private void CreateDetroyButton(int index)
    {
        var optionButton = CreateButton("Destroy");
        optionButton.Bind(()=> 
        { 
            Managers.Game.Player.Inventory.DestroyItemByIndex(index);
            Managers.UI.ClosePopupUI(this);
        });
    }

    private void CreateEquipButton(int index)
    {
        var optionButton = CreateButton("Equip");
        optionButton.Bind(() =>
        {
            Managers.Game.Player.Inventory.EquipItemByIndex(index);
            Managers.UI.ClosePopupUI(this);
        });
    }

    private void CreateUnEquipButton(int index)
    {
        var optionButton = CreateButton("UnEquip");
        optionButton.Bind(() =>
        {
            Managers.Game.Player.Inventory.UnEquipItemByIndex(index);
            Managers.UI.ClosePopupUI(this);
        });
    }

    private void CreateRegisteButton(int index)
    {
        var optionButton = CreateButton("Registe");
        optionButton.Bind(() =>
        {
            // Managers.Game.Player.Inventory.RegisteItemByIndex(index);
            // Mangers.UI.ShowPopup<QuickSlotRegister>();
            Managers.UI.ClosePopupUI(this);
        });
    }

    private UIOptionButton CreateButton(string name)
    {
        var go = Managers.Resource.Instantiate("UIOptionButton", Literals.PATH_UI, Get<GameObject>((int)GameObjects.Options).transform);
        var optionButton = go.GetComponent<UIOptionButton>();
        optionButton.SetText(name);
        return optionButton;
    }

    private void Clear()
    {
        var root = Get<GameObject>((int)GameObjects.Options).transform;
        var buutonCount = root.childCount;
        for (int i = 0; i < buutonCount; ++i)
        {
            Destroy(root.GetChild(i).gameObject);
        }
    }
}
