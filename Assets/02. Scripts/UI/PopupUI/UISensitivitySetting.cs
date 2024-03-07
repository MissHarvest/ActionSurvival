using Cinemachine;
using System.Collections;
using UnityEngine;

//2024. 02. 29 Byun Jeongmin
public class UISensitivitySetting : UIPopup
{
    enum SensitivityControllers
    {
        SensitivityController,
    }

    enum GameObjects
    {
        Block
    }

    private ViewPointRotation _viewPointRotation;

    public override void Initialize()
    {
        base.Initialize();
        Bind<UISensitivityController>(typeof(SensitivityControllers));
        Bind<GameObject>(typeof(GameObjects));

        Get<GameObject>((int)GameObjects.Block).BindEvent((x) => Managers.UI.ClosePopupUI(this));

        UISensitivityController sensitivityController;
        sensitivityController = Get<UISensitivityController>((int)SensitivityControllers.SensitivityController);
        sensitivityController.Init("회전 감도");
        sensitivityController.Slider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    private void Awake()
    {
        Initialize();
        FindViewPointRotation();
        var sensitivity = SaveGame.GetSensitivitySetting();
        UISensitivityController sensitivityController;
        sensitivityController = Get<UISensitivityController>((int)SensitivityControllers.SensitivityController);
        sensitivityController.Slider.value = sensitivity;

        gameObject.SetActive(false);
    }

    private void FindViewPointRotation()
    {
        var viewArea = GameObject.FindObjectOfType<ViewPointRotation>();
        _viewPointRotation = viewArea.GetComponent<ViewPointRotation>();
    }

    private void OnSensitivityChanged(float sensitivity)
    {
        if (_viewPointRotation != null)
        {
            _viewPointRotation.SetRotationSpeed(sensitivity);
        }
    }
}
