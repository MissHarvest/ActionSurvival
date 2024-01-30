using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public abstract class UIItemHelper : UIBase
{
    enum GameObjects
    {
        Container,
        Options,
    }

    enum Texts
    {
        ItemName,
    }

    protected Dictionary<string, GameObject> _buttons = new Dictionary<string, GameObject>();
        
    protected GameObject OptionBox => Get<GameObject>((int)GameObjects.Options);
    protected GameObject Container => Get<GameObject>((int)GameObjects.Container);
    
    public override void Initialize()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));
        gameObject.BindEvent((x) => { gameObject.SetActive(false); });
    }

    private void Awake()
    {
        Initialize();
        CreateButtons();
        gameObject.SetActive(false);
    }

    protected abstract void CreateButtons();

    protected void CreateButton(string key, string label = null)
    {
        if (label == null) label = key;

        var go = Managers.Resource.GetCache<GameObject>("UIOptionButton.prefab");
        go = Instantiate(go, Get<GameObject>((int)GameObjects.Options).transform);
        var optionButton = go.GetComponent<UIOptionButton>();
        optionButton.SetText(label);
        _buttons.TryAdd(key, optionButton.gameObject);
    }

    public abstract void ShowOption(ItemSlot selectedSlot, Vector3 position);

    public void SetItemName(string name)
    {
        Get<TextMeshProUGUI>((int)Texts.ItemName).text = name;
    }

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
