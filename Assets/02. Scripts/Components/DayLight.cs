using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayLight : MonoBehaviour
{
    public Light Light { get; private set; }

    public Color[] lightColors = new Color[3];
    private void Awake()
    {       
        RenderSettings.ambientIntensity = 0.0f;
        RenderSettings.reflectionIntensity = 0.0f;

        Light = GetComponent<Light>();
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
    }
    
    private void OnMorningCame()
    {
        Light.color = lightColors[0];
    }

    private void OnEveningCame()
    {
        Light.color = lightColors[1];
    }

    private void OnNightCame()
    {
        Light.color = lightColors[2];
    }
}
