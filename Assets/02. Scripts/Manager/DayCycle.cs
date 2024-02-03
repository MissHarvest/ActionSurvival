// 작성 날짜 : 2024. 01. 11
// 작성자 : Park Jun Uk

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Season
{
    Summer,
    Winter,
}

[System.Serializable]
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
    private int[] _eventCount = new int[3];
    [field:SerializeField] public int Date { get; private set; } = 1;
    private int _time = 0;
    private int _currentTimeZone = 0;
    private int _eventInterval = 10;
    public Season Season { get; private set; }

    public event Action OnMorningCame;
    public event Action OnEveningCame;
    public event Action OnNightCame;

    public event Action<int> OnDateUpdated;    
    public event Action OnTimeUpdated;

    public void Init()
    {
        OnDateUpdated = null;
        OnTimeUpdated = null;
        OnMorningCame = null;
        OnEveningCame = null;
        OnNightCame = null;

        CoroutineManagement.Instance.StartCoroutine(StartDayCycle());

        var dayLight = Managers.Resource.GetCache<GameObject>("DayLight.prefab");
        dayLight = UnityEngine.Object.Instantiate(dayLight);
        dayLight.name = "@DayLight";

        _eventCount[0] = _cycle[0] / _eventInterval;
        for (int i = 1; i < _eventCount.Length; ++i)
        {
            _eventCount[i] = _eventCount[i - 1] + _cycle[i] / _eventInterval;
        }
        Load();
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
                OnDateUpdated?.Invoke(++Date);
                Season = (Season)(Date / 7 % 2);
                _time = 0;
                Save();
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
        //Debug.Log($"TIME : {_time}");
        if (_time == _eventCount[_currentTimeZone])
        {            
            _currentTimeZone = (_currentTimeZone + 1) % (int)TimeZone.Max;
            BroadCast();
        }
    }

    private void Load()
    {
        if(SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, "WorldDate") == false)
        {
            Date = 1;
        }
        OnDateUpdated?.Invoke(Date);
    }

    private void Save()
    {
        var json = JsonUtility.ToJson(this);
        SaveGame.CreateJsonFile("WorldDate", json, SaveGame.SaveType.Runtime);
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
        while(_time != _eventCount[(int)TimeZone.Night] - 1)
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
        while (_time != _eventCount[(int)TimeZone.Morning] - 1)
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
        while (_time != _eventCount[(int)TimeZone.Evening] - 1)
        {
            yield return new WaitForSeconds(0.1f);
            FlowTime();
        }
    }
#endif
}
