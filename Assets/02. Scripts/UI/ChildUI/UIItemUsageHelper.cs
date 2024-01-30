using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UIItemUsageHelper : UIItemHelper
{
    public enum Functions
    {
        Regist,
        UnRegist,
        Use,
        Destroy
    }

    private string[] _functions = new string[] { "등록하기", "해제하기", "사용하기", "버리기" };
    
    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void CreateButtons()
    {
        CreateButtonByEnum(Functions.Regist);
        CreateButtonByEnum(Functions.UnRegist);
        CreateButtonByEnum(Functions.Use);
        CreateButtonByEnum(Functions.Destroy);
    }

    public override void ShowOption(ItemSlot selectedSlot, Vector3 position)
    {
        Clear();
        gameObject.SetActive(true);
        SetItemName(selectedSlot.itemData.displayName);

        Container.transform.position = position;

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

    private void CreateButtonByEnum(Functions label)
    {
        CreateButton(label.ToString(), _functions[(int)label]);
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
