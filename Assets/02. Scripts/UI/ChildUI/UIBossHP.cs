using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBossHP : UIBase
{
    enum Sliders
    {
        HpSlider,
    }

    enum Texts
    {
        HpText,
    }

    public BossMonster Boss { get; private set; } = null;

    public override void Initialize()
    {
        Bind<Slider>(typeof(Sliders));
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    private void Awake()
    {
        Initialize();
        gameObject.SetActive(false);
    }

    public void SetBoss(BossMonster boss)
    {
        Boss = boss;
        boss.HP.OnUpdated += OnBossHpUpdated;
        OnBossHpUpdated(boss.HP.GetPercentage());
        gameObject.SetActive(true);
    }
    
    private void OnBossHpUpdated(float percentage)
    {
        Get<Slider>((int)Sliders.HpSlider).value = percentage;
        Get<TextMeshProUGUI>((int)Texts.HpText).text = string.Format("{0:N0} %", percentage * 100);
    }

    private void OnDisable()
    {
        if (Boss != null)
        {
            Boss.HP.OnUpdated -= OnBossHpUpdated;
            Boss = null;
        }
    }
}
