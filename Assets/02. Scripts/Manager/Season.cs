using System;
using System.Diagnostics;

// 2024-02-14 WJY
public class Season
{
    public enum State
    {
        Spring,
        Summer,
        Winter,
    }

    private State _lastState = State.Spring;
    private float _value;
    public SeasonData Data { get; private set; }
    public event Action<State> OnSeasonChanged;
    public float CurrentValue => _value;
    public bool IsRestPeriod => !IsFireIslandActive && !IsIceIslandActive;
    public bool IsFireIslandActive => _value >= Data.FireIslandActivateThreshold;
    public bool IsIceIslandActive => _value <= Data.IceIslandActivateThreshold;

    public void Init()
    {
        Data = Managers.Resource.GetCache<SeasonData>("SeasonData.data");
    }

    public void BindEvent()
    {
        GameManager.DayCycle.OnStarted += Update;
        GameManager.DayCycle.OnDateUpdated += Update;
    }

    private void Update(DayInfo dayinfo)
    {
        Update(dayinfo.date);
    }

    public void Update(int date)
    {
        _value = SetValue(date);

        State currentState = SetState();
        
        if (_lastState != currentState)
        {
            _lastState = currentState;
            OnSeasonChanged?.Invoke(currentState);
            UnityEngine.Debug.Log($"[Season Changed]{currentState.ToString()}");
        }
    }

    private State SetState()
    {
        State state;
        if (IsFireIslandActive)
            state = State.Summer;
        else if (IsIceIslandActive)
            state = State.Winter;
        else
            state = State.Spring;
        return state;
    }

    private float SetValue(int date)
    {
        float t = ((float)date % Data.CycleValue) / Data.CycleValue;
        return Data.SeasonCurve.Evaluate(t);
    }
}