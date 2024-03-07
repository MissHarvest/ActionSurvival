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
        soundController.Init("전체");
        soundController.Slider.onValueChanged.AddListener(OnMasterVolumeChanged);

        soundController = Get<UISoundController>((int)SoundControllers.BGMController);
        soundController.Init("배경 음악");
        soundController.Slider.onValueChanged.AddListener(OnBGMVolumeChanged);


        soundController = Get<UISoundController>((int)SoundControllers.SFXController);
        soundController.Init("효과음");
        soundController.Slider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void Awake()
    {
        Initialize();
        var volumes = SaveGame.GetSoundSetting();

        for(int i = 0; i < volumes.Length; ++i)
        {
            Get<UISoundController>(i).Slider.value = volumes[i];
        }

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
