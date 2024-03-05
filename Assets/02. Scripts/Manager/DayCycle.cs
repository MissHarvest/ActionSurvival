// 작성 날짜 : 2024. 01. 11
// 작성자 : Park Jun Uk

using System;
using System.Collections;
using UnityEngine;

public struct DayInfo
{
    public int date;
    public int time;
    public DayCycle.TimeZone zone;

    public DayInfo(int date, int time, DayCycle.TimeZone zone)
    {
        this.date = date;
        this.time = time;
        this.zone = zone;
    }
}

[System.Serializable]
public class DayCycle
{
    public enum TimeZone
    {
        Morning,
        Evening,
        Night,
        Max,
    }

    private int[] _cycle = { 180, 60, 120 };
    public int Day { get; private set; }
    private int[] _eventCount = new int[3];
    [SerializeField] private int _date = 1;
    [SerializeField] private int _time = 0;
    [SerializeField] private int _currentTimeZone = 0;
    private int _eventInterval = 5;

    public event Action OnMorningCame;
    public event Action OnEveningCame;
    public event Action OnNightCame;

    public event Action<int> OnDateUpdated;    
    public event Action OnTimeUpdated;
    public event Action<int, int> OnUpdated;
    public event Action<DayInfo> OnStarted;

    public void Init()
    {
        var dayLight = Managers.Resource.GetCache<GameObject>("DayLight.prefab");
        dayLight = UnityEngine.Object.Instantiate(dayLight);
        dayLight.name = "@DayLight";

        _eventCount[0] = _cycle[0] / _eventInterval;
        for (int i = 1; i < _eventCount.Length; ++i)
        {
            _eventCount[i] = _eventCount[i - 1] + _cycle[i] / _eventInterval;
        }
        Day = _eventCount[2];
        Load();
    }    

    public void BindEvent()
    {
        GameManager.Instance.OnSaveCallback += Save;
    }

    IEnumerator StartDayCycle()
    {
        var obj = new WaitForSeconds(_eventInterval);
        while (true)
        {
            yield return obj;
            FlowTime();
        }
    }

    private void BroadCast()
    {
        switch((TimeZone)_currentTimeZone)
        {
            case TimeZone.Morning:
                _time = 0;
                OnDateUpdated?.Invoke(++_date);
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

        OnUpdated?.Invoke(_date, _time);
    }

    //private void Load()
    //{
    //    SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, "WorldDate");
    //}

    //private void Save()
    //{
    //    var json = JsonUtility.ToJson(this);
    //    SaveGame.CreateJsonFile("WorldDate", json, SaveGame.SaveType.Runtime);
    //}

    private async void Load()
    {
        bool firebaseDataExists = await SaveGameUsingFirebase.CheckIfNodeDataExists("WorldDate");
        if (firebaseDataExists)
            SaveGameUsingFirebase.LoadFromFirebase<DayCycle>("WorldDate", this, () => {
                OnDateUpdated?.Invoke(_date);
                OnTimeUpdated?.Invoke();
                OnUpdated?.Invoke(_date, _time);

                CoroutineManagement.Instance.StartCoroutine(StartDayCycle());
                Debug.Log($"[Day Cycle Start]{_date}/{_time}");
                OnStarted?.Invoke(new DayInfo(_date, _time, (TimeZone)_currentTimeZone));
            });
        else
        {
            CoroutineManagement.Instance.StartCoroutine(StartDayCycle());
            Debug.Log($"[Day Cycle Start]{_date}/{_time}");
            OnStarted?.Invoke(new DayInfo(_date, _time, (TimeZone)_currentTimeZone));
        }
    }

    //private async void Load()
    //{
    //    var loadedData = await SaveGameUsingFirebase.LoadFromFirebase<DayCycle>("WorldDate");
    //    if (loadedData != null)
    //    {
    //        _date = loadedData._date;
    //        _time = loadedData._time;
    //        _currentTimeZone = loadedData._currentTimeZone;

    //        OnDateUpdated?.Invoke(_date);
    //        OnTimeUpdated?.Invoke();
    //        OnUpdated?.Invoke(_date, _time);
    //    }
    //    else
    //    {
    //        _date = 1;
    //        _time = 0;
    //        _currentTimeZone = 0;

    //        OnDateUpdated?.Invoke(_date);
    //        OnTimeUpdated?.Invoke();
    //        OnUpdated?.Invoke(_date, _time);
    //    }
    //}


    private void Save()
    {
        var json = JsonUtility.ToJson(this);
        SaveGameUsingFirebase.SaveToFirebase("WorldDate", json);
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
