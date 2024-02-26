using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

// 2024. 02. 24. Park Jun Uk
public class UITipSlot : UIBase
{
    enum Texts
    {
        Text,
    }

    private Button _button;
    private int _index;

    private void Awake()
    {
        Initialize();
    }

    public override void Initialize()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Get<TextMeshProUGUI>((int)Texts.Text).raycastTarget = false;
        _button = GetComponent<Button>();
    }

    public void Set(TipData tip, UnityAction<int> action)
    {
        _index = tip.id;
        Get<TextMeshProUGUI>((int)Texts.Text).text = tip.displayTitle;

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => { action?.Invoke(_index); });
    }
}
