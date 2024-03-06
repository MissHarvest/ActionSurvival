using TMPro;
using UnityEngine.UI;

// 2024. 02. 29 Byun Jeongmin
public class UISensitivityController : UIBase
{
    enum Texts
    {
        Category,
    }

    enum Sliders
    {
        SensitivitySlider,
    }

    public Slider Slider => Get<Slider>((int)Sliders.SensitivitySlider);

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
