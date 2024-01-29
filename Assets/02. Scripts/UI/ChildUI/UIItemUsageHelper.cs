using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class UIItemUsageHelper : UIItemHelper
{
    public enum Functions
    {
        Regist,
        UnRegist,
        Use,
        Destroy
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void CreateButtons()
    {
        CreateButtonByEnum(Functions.Regist);//, RegistItem);
        CreateButtonByEnum(Functions.UnRegist);//, UnregistItem);
        CreateButtonByEnum(Functions.Use);//, ConsumeItem);
        CreateButtonByEnum(Functions.Destroy);//, DestryoItem);
    }

    public override void ShowOption(ItemSlot selectedSlot, Vector3 position)
    {
        Clear();
        gameObject.SetActive(true);

        OptionBox.transform.position = position;

        // other Button Add
        switch (selectedSlot.itemData)
        {
            case ToolItemData _:
                ShowRegistButton(selectedSlot.registed);
                break;

            case ConsumeItemData _:
                ShowRegistButton(selectedSlot.registed);
                ShowButtonByEnum(Functions.Use);
                break;
        }
        ShowButtonByEnum(Functions.Destroy);

        // Set My Size

    }

    #region ShowButton

    private void ShowRegistButton(bool regist)
    {
        if(regist)
        {
            ShowButtonByEnum(Functions.UnRegist);            
        }
        else
        {
            ShowButtonByEnum(Functions.Regist);
        }
    }

    private void CreateButtonByEnum(Functions label)//, UnityAction action)
    {
        CreateButton(label.ToString());//, action);
    }

    private void ShowButtonByEnum(Functions function)
    {
        ShowButton(function.ToString());
    }

    public void BindEventOfButton(Functions functions, UnityAction action)
    {
        string name = functions.ToString();
        if(_buttons.TryGetValue(name, out GameObject button))
        {
            button.GetComponent<UIOptionButton>().Bind(action);
        }
    }

    #endregion
}
