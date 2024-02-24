using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 2024. 02. 24. Park Jun Uk
public class UITipInformation : UIPopup
{
    enum GameObjects
    {
        Block,
        Content,
        Information
    }

    enum Images
    {
        Image,
    }

    enum Texts
    {
        Text,
    }

    private List<TipData> _tips;

    public override void Initialize()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Get<GameObject>((int)GameObjects.Block).BindEvent((x) =>
        {
            Managers.UI.ClosePopupUI(this);
        });

        CreateSlot();
    }

    private void Awake()
    {
        Initialize();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ShowInformation(0);
    }

    private void CreateSlot()
    {
        var prefab = Managers.Resource.GetCache<GameObject>("UITipSlot.prefab");
        _tips = Managers.Resource.GetCache<TipsSO>("Tips.data").tips;

        for(int i = 0; i < _tips.Count; ++i)
        {
            var go = Instantiate(prefab, Get<GameObject>((int)GameObjects.Content).transform);
            var tipSlotUI = go.GetComponent<UITipSlot>();
            tipSlotUI?.Set(_tips[i], ShowInformation);
        }

        _tips = _tips.OrderBy(x => x.id).ToList();
    }

    public void ShowInformation(int index)
    {
        Debug.Log($"[Show Inform]{index}");
        Get<Image>((int)Images.Image).sprite = _tips[index].sprite;
        Get<TextMeshProUGUI>((int)Texts.Text).text = _tips[index].information;
        Get<GameObject>((int)GameObjects.Information).SetActive(true);
    }
}
