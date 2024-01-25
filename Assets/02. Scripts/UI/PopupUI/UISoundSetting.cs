using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundSetting : UIPopup
{
    enum SoundControllers
    {
        MasterController,
        BGMController,
        SFXController,
    }

    enum GameObjects
    {
        Block
    }

    public override void Initialize()
    {
        base.Initialize();
        Bind<UISoundController>(typeof(SoundControllers));
        Bind<GameObject>(typeof(GameObjects));

        Get<GameObject>((int)GameObjects.Block).BindEvent((x) => Managers.UI.ClosePopupUI(this));
        
        UISoundController soundController;
        soundController = Get<UISoundController>((int)SoundControllers.MasterController);
        soundController.Init("Master");
        soundController.Slider.onValueChanged.AddListener(OnMasterVolumeChanged);

        soundController = Get<UISoundController>((int)SoundControllers.BGMController);
        soundController.Init("BGM");
        soundController.Slider.onValueChanged.AddListener(OnBGMVolumeChanged);


        soundController = Get<UISoundController>((int)SoundControllers.SFXController);
        soundController.Init("SFX");
        soundController.Slider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void Awake()
    {
        Initialize();
        gameObject.SetActive(false);
    }

    private void OnMasterVolumeChanged(float volume)
    {
        Managers.Sound.Set("Master", volume);
    }

    private void OnBGMVolumeChanged(float volume)
    {
        Managers.Sound.Set("BGM", volume);
    }

    private void OnSFXVolumeChanged(float volume)
    {
        Managers.Sound.Set("SFX", volume);
    }
}
