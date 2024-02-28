using TMPro;
using UnityEngine;

// 2024. 02. 07 Byun Jeongmin
public class UIMinimap : UIPopup
{
    enum GameObjects
    {
        Block,
    }

    enum Texts
    {
        CoordinatesText,
    }

    private Camera _minimapCam;

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Get<GameObject>((int)GameObjects.Block).BindEvent((x) => { Managers.UI.ClosePopupUI(this); });
    }

    private void Awake()
    {
        Initialize();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (_minimapCam == null)
            FindMinimapCamera();
        _minimapCam.enabled = true;
    }

    private void OnDisable()
    {
        if (_minimapCam != null)
            _minimapCam.enabled = false;
    }

    private void Update()
    {
        UpdateCoordinatesText();
    }

    private void FindMinimapCamera()
    {
        var _minimapCamGO = GameObject.FindObjectOfType<MapIlluminator>();

        if (_minimapCamGO != null)
            _minimapCam = _minimapCamGO.GetComponent<Camera>();
    }

    private void UpdateCoordinatesText()
    {
        Vector3 playerPosition = GameManager.Instance.Player.transform.position;
        Get<TextMeshProUGUI>((int)Texts.CoordinatesText).text = playerPosition.ToString("F0");
    }
}
