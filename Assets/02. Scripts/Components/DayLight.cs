// 작성 날짜 : 2024. 01. 11
// 작성자 : Park Jun Uk

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DayLight : MonoBehaviour
{
    public Light Light { get; private set; }

    public Color[] lightColors = new Color[3];
    public Color[] fogColors = new Color[3];
    public Color[] skyColors = new Color[3];
    public Material skyMaterial;
    private Skybox _skyBox;

    private void Awake()
    {       
        RenderSettings.ambientIntensity = 0.0f;
        RenderSettings.reflectionIntensity = 0.0f;
        RenderSettings.fog = true;
        
        Light = GetComponent<Light>();
        //_skyBox = Camera.main.GetComponent<Skybox>();
        RenderSettings.skybox = skyMaterial;
    }

    // Start is called before the first frame update
    void Start()
    {
        var go = GameObject.Find("Directional Light");
        if (go != null)
        {
            Destroy(go);
        }

        Managers.Game.DayCycle.OnMorningCame += OnMorningCame;
        Managers.Game.DayCycle.OnEveningCame += OnEveningCame;
        Managers.Game.DayCycle.OnNightCame += OnNightCame;

        OnMorningCame();
    }
    
    private void OnMorningCame()
    {
        StartCoroutine(LerpEnviroment(2, 0));
    }

    private void OnEveningCame()
    {
        StartCoroutine(LerpEnviroment(0, 1));
    }

    private void OnNightCame()
    {
        StartCoroutine(LerpEnviroment(1, 2));
    }

    IEnumerator LerpEnviroment(int start, int end)
    {
        float t = 0.0f;
        while(t <= 10.0f)
        {
            yield return new WaitForSeconds(0.1f);
            t += 0.1f;
            Light.color = Color.Lerp(lightColors[start], lightColors[end], t * 0.1f);

            Color skycolor = Color.Lerp(skyColors[start], skyColors[end], t * 0.1f);
            RenderSettings.skybox.SetColor("_Tint", skycolor);

            RenderSettings.fogColor = Color.Lerp(fogColors[start], fogColors[end], t * 0.1f);
        }
    }
}
