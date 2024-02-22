using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UIItemTransitionHelper : UIItemHelper
{
    public enum Functions
    {
        Store,
        TakeOut,
    }

    private string[] _functions = new string[] { "보관하기", "꺼내기" };

    public void BindEventOfButton(Functions functions, UnityAction action)
    {
        string name = functions.ToString();
        if (_buttons.TryGetValue(name, out GameObject button))
        {
            button.GetComponent<UIOptionButton>().Bind(action);
        }
    }

    public override void ShowOption(ItemSlot selectedSlot, Vector3 position)
    {
        Clear();
        gameObject.SetActive(true);
        SetItemName(selectedSlot.itemData.displayName);

        Container.transform.position = position;

        if (selectedSlot.inventory)
        {
            if(selectedSlot.inventory == GameManager.Instance.Player.Inventory)
            {
                ShowButton(Functions.Store.ToString());
            }
            else
            {
                ShowButton(Functions.TakeOut.ToString());
            }
        }
    }

    protected override void CreateButtons()
    {
        //CreateButton(Functions.Store.ToString());
        //CreateButton(Functions.TakeOut.ToString());
        CreateButton(Functions.Store.ToString(), _functions[(int)Functions.Store]);
        CreateButton(Functions.TakeOut.ToString(), _functions[(int)Functions.TakeOut]);
    }
}
