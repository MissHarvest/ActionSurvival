using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 2024. 02. 01 Park Jun Uk
public class UIArchitectureHP : UIBase
{
    enum Sliders
    {
        Slider,
    }

    private Coroutine _coroutine;

    public override void Initialize()
    {
        Bind<Slider>(typeof(Sliders));
    }

    private void Awake()
    {
        Initialize();
        var buildObject = GetComponentInParent<BuildableObject>();
        buildObject.OnHit += SetValue;
        SetValue(buildObject.HP.GetPercentage());
        
        buildObject.OnRenamed += RotateToCamera;
        Get<Slider>((int)Sliders.Slider).gameObject.SetActive(false);
    }

    public void SetValue(float percentage)
    {
        Get<Slider>((int)Sliders.Slider).gameObject.SetActive(true);
        Get<Slider>((int)Sliders.Slider).value = percentage;
        if(_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(Inactivate());
    }

    private IEnumerator Inactivate()
    {
        yield return new WaitForSeconds(2.0f);
        Get<Slider>((int)Sliders.Slider).gameObject.SetActive(false);
    }

    private void RotateToCamera()
    {
        //Vector3 dir = Camera.main.transform.position - transform.position;
        //transform.rotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.identity;
    }
}
