using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 2024. 01. 15. Park Jun Uk
public class UIClock : UIBase
{
    #region Init
    enum Texts
    {
        Date,
    }

    enum GameObjects
    {
        Pivot,
    }

    public override void Initialize()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
    }
    #endregion

    private float _angle = 0.0f;
    private float _intervalAngle = 7.5f;
    private const float _maxAngle = 360.0f;

    private void Awake()
    {
        Initialize();
        var dayCycle = GameManager.DayCycle;

        dayCycle.OnStarted += OnDayStarted;
        dayCycle.OnDateUpdated += OnDateUpdated;
        dayCycle.OnTimeUpdated += OnTimeUpdated;
    }

    private void OnDayStarted(DayInfo day)
    {
        Debug.Log("[Day]" + GameManager.DayCycle.Day);
        _intervalAngle = 360.0f / GameManager.DayCycle.Day;
        OnDateUpdated(day.date);
        _angle = _intervalAngle * day.time;
        Get<GameObject>((int)GameObjects.Pivot).transform.rotation = Quaternion.Euler(0, 0, -_angle);
    }

    private void OnDateUpdated(int date)
    {
        Get<TextMeshProUGUI>((int)Texts.Date).text = date.ToString();
    }

    private void OnTimeUpdated()
    {
        _angle = Mathf.Min(_angle + _intervalAngle, _maxAngle);
        if (_angle >= _maxAngle) _angle = 0.0f;
        Get<GameObject>((int)GameObjects.Pivot).transform.rotation = Quaternion.Euler(0, 0, -_angle);
    }
}
