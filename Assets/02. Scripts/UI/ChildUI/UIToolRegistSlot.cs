using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class UIToolRegistSlot : UIItemSlot
{
    public UIToolRegister UIToolRegister { get; private set; }

    private void Awake()
    {
        Initialize();

        gameObject.BindEvent((x) =>
        {
            GameManager.Instance.Player.QuickSlot.Regist(Index, UIToolRegister.SelectedSlot);
            Managers.UI.ClosePopupUI(UIToolRegister);
        });
    }

    public void Init(UIToolRegister toolRegisterUI, int index, ItemSlot itemSlot)
    {
        UIToolRegister = toolRegisterUI;
        BindGroup(null, index);
        Set(itemSlot);
    }

    public override void Set(ItemSlot itemSlot)
    {
        if(itemSlot.itemData == null)
        {
            Clear();
            return;
        }

        base.Set(itemSlot);
    }
}
