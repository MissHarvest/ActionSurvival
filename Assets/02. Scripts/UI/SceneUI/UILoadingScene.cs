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
    [SerializeField] private AnimationCurve _curve1;
    [SerializeField] private AnimationCurve _curve2;
    private float time = 0f;
    private Image _loadingCircleImage;
    private Transform _loadingCircleTransform;
    private TMP_Text _loadingArgumentText;

    #region Initialize

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time > 1f) time -= 1f;
        _loadingCircleImage.color = gradient.Evaluate(time);
        _loadingCircleImage.fillAmount = _curve1.Evaluate(time);
        _loadingCircleTransform.Rotate(0, 0, -820f * _curve2.Evaluate(time) * Time.deltaTime);
    }

    public override void Initialize()
    {
        base.Initialize();

        BindImage(typeof(Images));
        BindText(typeof(Texts));

        _loadingCircleImage = GetImage((int)Images.LoadingCircle);
        _loadingCircleTransform = _loadingCircleImage.transform;
        _loadingArgumentText = GetText((int)Texts.LoadingArgument);
    }

    public void ReceiveCallbacks(string argument)
    {
        _loadingArgumentText.text = argument;
    }

    #endregion
}
