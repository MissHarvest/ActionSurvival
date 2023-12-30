using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToolRegistSlot : UIItemSlot
{
    public UIToolRegister UIToolRegister { get; private set; }
    public int index { get; private set; }

    private void Awake()
    {
        Initialize();

        gameObject.BindEvent((x) =>
        {
            Managers.Game.Player.QuickSlot.Regist(index, UIToolRegister.sourceIndex);
            Managers.UI.ClosePopupUI(UIToolRegister);
        });
    }

    public void Init(UIToolRegister toolRegisterUI, int index, ItemSlot itemSlot)
    {
        UIToolRegister = toolRegisterUI;
        this.index = index;
        Set(itemSlot);
    }

    public override void Set(ItemSlot itemSlot)
    {
        if(itemSlot.bUse == false)
        {
            Clear();
            return;
        }

        base.Set(itemSlot);
    }
}
