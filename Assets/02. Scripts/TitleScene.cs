using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    public Button LoadNewScene;
    public Button NewStart;

    public IEnumerator Start()
    {
        UITitleScene titleScene = null;
        Managers.Resource.LoadAsync<UnityEngine.Object>("UITitleScene.prefab", (obj) =>
        {
            titleScene = Managers.UI.ShowSceneUI<UITitleScene>();
        });

        yield return new WaitWhile(()=> titleScene == null);

        ResourceLoad((key, count, total) =>
        {            
            if (count == total)
            {
                titleScene.ActivateButtons();

                Managers.Sound.Init();
                Managers.Sound.PlayBGM("TitleBGM");
            }
        });
    }

    private void ResourceLoad(Action<string, int, int> callback)
    {
        Managers.Resource.LoadAllAsync<UnityEngine.Object>("MainScene", callback);
    }
}
