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
                    CreateUnRegistButton(index);
                }
                else
                {
                    CreateRegistButton(index);
                }                

                if(itemSlot.bUse)
                {
                    CreateUnEquipButton(index);
                }
                else
                {
                    CreateEquipButton(index);
                }
                break;

            case ConsumeItemData _:
                CreateUseButton(index);
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

    private void CreateUseButton(int index)
    {
        var optionButton = CreateButton("Use");
        optionButton.Bind(() =>
        {
            Managers.Game.Player.Inventory.UseItemByIndex(index);
            Managers.UI.ClosePopupUI(this);
        });
    }

    private void CreateRegistButton(int index)
    {
        var optionButton = CreateButton("Regist");
        optionButton.Bind(() =>
        {
            Managers.UI.ClosePopupUI(this); // 위에서 부터 순서대로 닫는데, this 가 최근이 아니라서 안 닫힘
            Managers.UI.ShowPopupUI<UIToolRegister>().Set(index);            
        });
    }

    private void CreateUnRegistButton(int index)
    {
        var optionButton = CreateButton("UnRegist");
        optionButton.Bind(() =>
        {
            Managers.Game.Player.QuickSlot.UnRegist(index);
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
