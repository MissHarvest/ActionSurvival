using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycle
{
    enum TimeZone
    {
        Morning,
        Evening,
        Night,
        Max,
    }

    private int[] _cycle = { 300, 60, 120 };
    private int[] eventCount = new int[3];
    private int _date = 0;
    private int _time = 0;
    private int _currentTimeZone = 0;
    private int _eventInterval = 10;

    public event Action OnMorningCame;
    public event Action OnEveningCame;
    public event Action OnNightCame;

    public event Action<int> OnDateUpdated;    
    public event Action OnTimeUpdated;

    public void Init()
    {
        CoroutineManagement.Instance.StartCoroutine(StartDayCycle());

        var dayLight = Managers.Resource.GetCache<GameObject>("DayLight.prefab");
        dayLight = UnityEngine.Object.Instantiate(dayLight);
        dayLight.name = "@DayLight";
        //Managers.Resource.Instantiate("DayLight").name = "@DayLight";

        eventCount[0] = _cycle[0] / _eventInterval;
        for (int i = 1; i < eventCount.Length; ++i)
        {
            eventCount[i] = eventCount[i - 1] + _cycle[i] / _eventInterval;
        }
    }    

    IEnumerator StartDayCycle()
    {
        while(true)
        {
            yield return new WaitForSeconds(_eventInterval);
            FlowTime();
        }
    }

    private void BroadCast()
    {
        switch((TimeZone)_currentTimeZone)
        {
            case TimeZone.Morning:
                OnDateUpdated?.Invoke(++_date);
                _time = 0;
                OnMorningCame?.Invoke();
                break;

            case TimeZone.Evening:
                OnEveningCame?.Invoke();
                break;

            case TimeZone.Night:
                OnNightCame?.Invoke();
                break;
        }
    }

    private void FlowTime()
    {
        ++_time;
        OnTimeUpdated?.Invoke();
        Debug.Log($"TIME : {_time}");
        if (_time == eventCount[_currentTimeZone])
        {            
            _currentTimeZone = (_currentTimeZone + 1) % (int)TimeZone.Max;
            BroadCast();
        }
    }

#if UNITY_EDITOR
    public void SkipTime()
    {
        FlowTime();
    }

    public void PassToMorning()
    {
        CoroutineManagement.Instance.StartCoroutine(SkipToMoring());
    }

    IEnumerator SkipToMoring()
    {
        while(_time != 47) // Sum Cycle - one time
        {
            yield return new WaitForSeconds(0.1f);
            FlowTime();
        }
    }

    public void PassToEvening()
    {
        CoroutineManagement.Instance.StartCoroutine(SkipToEvening());
    }

    IEnumerator SkipToEvening()
    {
        while (_time != 29) // 300 - one time
        {
            yield return new WaitForSeconds(0.1f);
            FlowTime();
        }
    }

    public void PassToNight()
    {
        CoroutineManagement.Instance.StartCoroutine(SkipToNight());
    }

    IEnumerator SkipToNight()
    {
        while (_time != 35) // Sum Cycle - one time
        {
            yield return new WaitForSeconds(0.1f);
            FlowTime();
        }
    }
#endif
}
