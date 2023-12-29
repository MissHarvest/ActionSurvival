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
            SceneManager.LoadScene("Test Scene");
        });
    }
}
