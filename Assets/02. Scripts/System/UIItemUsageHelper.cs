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

    public void ShowOption(int index, Transform transform)
    {
        var itemSlot = Managers.Game.Player.Inventory.slots[index];
        if (itemSlot == null) return;

        // other Button Add

        CreateOptionButtonAsDestory(index);
    }

    private void CreateOptionButtonAsDestory(int index)
    {
        var go = Managers.Resource.Instantiate("UIOptionButton", Literals.PATH_UI, Get<GameObject>((int)GameObjects.Options).transform);
        var optionButton = go.GetComponent<UIOptionButton>();
        optionButton.SetText("Destroy");
        optionButton.Bind(()=> 
        { 
            Managers.Game.Player.Inventory.DestroyItemByIndex(index);
            Managers.UI.ClosePopupUI(this);
        });
    }
}
