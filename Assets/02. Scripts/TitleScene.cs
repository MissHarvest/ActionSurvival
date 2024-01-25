using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    public Button LoadNewScene;

    private void Awake()
    {        
        LoadNewScene.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Main Scene");
        });
    }

    private void Start()
    {
        Managers.Resource.LoadAsync<UnityEngine.Object>("TitleBGM.wav",(x) => 
        {
            Managers.Sound.Init();
            Managers.Sound.PlayBGM("TitleBGM");
        });
    }
}
