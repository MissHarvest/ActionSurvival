using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIItemUsageHelper : UIPopup
{
    enum GameObjects
    {
        Block,
        Options,        
    }

    enum Functions
    {
        Regist,
        UnRegist,
        Equip,
        UnEquip,
        Use,
        Destroy
    }

    private Dictionary<Functions, GameObject> Buttons = new Dictionary<Functions, GameObject>();

    public QuickSlot SelectedItem { get; private set; } = new QuickSlot();

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(GameObjects));
        Get<GameObject>((int)GameObjects.Block).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });
    }

    private void Awake()
    {
        Initialize();
        CreateButtons();
        gameObject.SetActive(false);
    }

    private void CreateButtons()
    {        
        Buttons.TryAdd(Functions.Regist, CreateRegistButton().gameObject);
        Buttons.TryAdd(Functions.UnRegist, CreateUnRegistButton().gameObject);
        Buttons.TryAdd(Functions.Equip, CreateEquipButton().gameObject);
        Buttons.TryAdd(Functions.UnEquip, CreateUnEquipButton().gameObject);
        Buttons.TryAdd(Functions.Use, CreateUseButton().gameObject);
        Buttons.TryAdd(Functions.Destroy, CreateDetroyButton().gameObject);
    }

    public void ShowOption(int index, Vector3 position)
    {
        Clear();

        SelectedItem.Set(index, Managers.Game.Player.Inventory.slots[index]);
        if (SelectedItem == null) return;
        Get<GameObject>((int)GameObjects.Options).transform.position = position;

        // other Button Add
        switch (SelectedItem.itemSlot.itemData)
        {
            case ToolItemData _:
                ShowRegistButton(SelectedItem.itemSlot.registed);
                break;

            case ConsumeItemData _:
                ShowRegistButton(SelectedItem.itemSlot.registed);
                ShowButton(Functions.Use);
                break;
        }
        ShowButton(Functions.Destroy);

        // Set My Size

    }

    #region CreateButton

    private UIOptionButton CreateDetroyButton()
    {
        var optionButton = CreateButton(Functions.Destroy.ToString());
        optionButton.Bind(()=> 
        { 
            Managers.Game.Player.Inventory.DestroyItemByIndex(SelectedItem);
            Managers.UI.ClosePopupUI(this);
        });
        return optionButton;
    }

    private UIOptionButton CreateEquipButton()
    {
        var optionButton = CreateButton(Functions.Equip.ToString());
        optionButton.Bind(() =>
        {
            Managers.Game.Player.Inventory.EquipItemByIndex(SelectedItem.targetIndex);
            Managers.UI.ClosePopupUI(this);
        });
        return optionButton;
    }

    private UIOptionButton CreateUnEquipButton()
    {
        var optionButton = CreateButton(Functions.UnEquip.ToString());
        optionButton.Bind(() =>
        {
            Managers.Game.Player.Inventory.UnEquipItemByIndex(SelectedItem.targetIndex);
            Managers.UI.ClosePopupUI(this);
        });
        return optionButton;
    }

    private UIOptionButton CreateUseButton()
    {
        var optionButton = CreateButton(Functions.Use.ToString());
        optionButton.Bind(() =>
        {
            Managers.Game.Player.Inventory.UseItemByIndex(SelectedItem.targetIndex);
            Managers.UI.ClosePopupUI(this);
        });
        return optionButton;
    }

    private UIOptionButton CreateRegistButton()
    {
        var optionButton = CreateButton(Functions.Regist.ToString());
        optionButton.Bind(() =>
        {
            Managers.UI.ClosePopupUI(this); // 위에서 부터 순서대로 닫는데, this 가 최근이 아니라서 안 닫힘
            Managers.UI.ShowPopupUI<UIToolRegister>().Set(SelectedItem);            
        });
        return optionButton;
    }

    private UIOptionButton CreateUnRegistButton()
    {
        var optionButton = CreateButton(Functions.UnRegist.ToString());
        optionButton.Bind(() =>
        {
            Managers.Game.Player.QuickSlot.UnRegist(SelectedItem);
            Managers.UI.ClosePopupUI(this);
        });
        return optionButton;
    }

    private UIOptionButton CreateButton(string name)
    {
        var go = Managers.Resource.Instantiate("UIOptionButton", Literals.PATH_UI, Get<GameObject>((int)GameObjects.Options).transform);
        var optionButton = go.GetComponent<UIOptionButton>();
        optionButton.SetText(name);
        return optionButton;
    }

    #endregion

    #region ShowButton
    private void ShowEquipButton(bool regist)
    {
        if (regist)
        {
            ShowButton(Functions.UnEquip);
        }
        else
        {
            ShowButton(Functions.Equip);
        }
    }

    private void ShowRegistButton(bool regist)
    {
        if(regist)
        {
            ShowButton(Functions.UnRegist);            
        }
        else
        {
            ShowButton(Functions.Regist);
        }
    }

    private void ShowButton(Functions function)
    {
        Buttons[function].SetActive(true);
    }

    #endregion

    private void Clear()
    {
        foreach(var go in Buttons.Values)
        {
            go.SetActive(false);
        }
    }
}
