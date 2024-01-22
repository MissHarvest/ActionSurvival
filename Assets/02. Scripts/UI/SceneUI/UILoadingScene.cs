using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 2024-01-22 WJY
public class UILoadingScene : UIScene
{
    #region Enums

    private enum Images
    {
        LoadingCircle,
    }

    private enum Texts
    {
        LoadingArgument,
    }

    #endregion

    [SerializeField] private Gradient gradient;
    private float time = 0f;
    private Image _image;
    private TMP_Text _text;

    #region Initialize

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time > 1f) time -= 1f;
        _image.color = gradient.Evaluate(time);
    }

    public override void Initialize()
    {
        base.Initialize();

        BindImage(typeof(Images));
        BindText(typeof(Texts));

        _image = GetImage((int)Images.LoadingCircle);
        _text = GetText((int)Texts.LoadingArgument);
    }

    public void ReceiveCallbacks(string argument)
    {
        _text.text = argument;
    }

    #endregion
}
