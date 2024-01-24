using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public abstract class UIItemHelper : UIBase
{
    enum GameObjects
    {
        Options,
    }

    protected Dictionary<string, GameObject> _buttons = new Dictionary<string, GameObject>();
        
    protected GameObject OptionBox => Get<GameObject>((int)GameObjects.Options);

    public override void Initialize()
    {
        Bind<GameObject>(typeof(GameObjects));
        gameObject.BindEvent((x) => { gameObject.SetActive(false); });
    }

    private void Awake()
    {
        Initialize();
        CreateButtons();
        gameObject.SetActive(false);
    }

    protected abstract void CreateButtons();

    protected void CreateButton(string label)//, UnityAction action)
    {
        var go = Managers.Resource.GetCache<GameObject>("UIOptionButton.prefab");
        go = Instantiate(go, Get<GameObject>((int)GameObjects.Options).transform);
        var optionButton = go.GetComponent<UIOptionButton>();
        optionButton.SetText(label);
        _buttons.TryAdd(label, optionButton.gameObject);
    }

    public abstract void ShowOption(ItemSlot selectedSlot, Vector3 position);

    protected void ShowButton(string name)
    {
        _buttons[name].SetActive(true);
    }

    protected void Clear()
    {
        foreach (var go in _buttons.Values)
        {
            go.SetActive(false);
        }
    }
}
