// 작성 날짜 : 2024. 01. 11
// 작성자 : Park Jun Uk

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DayLight : MonoBehaviour
{
    public Light Light { get; private set; }

    public Color[] lightColors = new Color[3];
    public Material[] skyMaterial = new Material[3];
    public Color[] fogColors = new Color[3];

    private Skybox _skyBox;
    private void Awake()
    {       
        RenderSettings.ambientIntensity = 0.0f;
        RenderSettings.reflectionIntensity = 0.0f;
        RenderSettings.fog = true;

        Light = GetComponent<Light>();
        _skyBox = Camera.main.GetComponent<Skybox>();        
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
        SetEnviroment(0);
    }

    private void OnEveningCame()
    {
        SetEnviroment(1);
    }

    private void OnNightCame()
    {
        SetEnviroment(2);
    }

    private void SetEnviroment(int index)
    {
        Light.color = lightColors[index];
        _skyBox.material = skyMaterial[index];
        RenderSettings.fogColor = fogColors[index];
    }
}
