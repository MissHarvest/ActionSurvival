using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISoundController : UIBase
{
    enum Texts
    {
        Category,
    }

    enum Sliders
    {
        SoundSlider,
    }

    public Slider Slider => Get<Slider>((int)Sliders.SoundSlider);

    public override void Initialize()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Slider>(typeof(Sliders));
    }

    public void Init(string category)
    {
        Initialize();
        Get<TextMeshProUGUI>((int)Texts.Category).text = category;
    }

}
