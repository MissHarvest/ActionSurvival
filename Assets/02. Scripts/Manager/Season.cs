using System;
using System.Diagnostics;
//using static UnityEditorInternal.VersionControl.ListControl;

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

    public void Initialize(int date)
    {
        Data = Managers.Resource.GetCache<SeasonData>("SeasonData.data");
        Update(date);
    }

    public void Update(int date)
    {
        _value = SetValue(date);

        State currentState = SetState();
        
        if (_lastState != currentState)
        {
            _lastState = currentState;
            OnSeasonChanged?.Invoke(currentState);
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