using System;

// 2024-02-14 WJY
public class Season
{
    private int _lastState;
    private float _value;
    public SeasonData Data { get; private set; }
    public event Action OnSeasonChanged;
    public float CurrentValue => _value;
    public bool IsRestPeriod => !IsFireIslandActive && !IsIceIslandActive;
    public bool IsFireIslandActive => _value >= Data.FireIslandActivateThreshold;
    public bool IsIceIslandActive => _value <= Data.IceIslandActivateThreshold;

    public void Initialize(int date)
    {
        OnSeasonChanged = null;
        Data = Managers.Resource.GetCache<SeasonData>("SeasonData.data");
        Update(date);
    }

    public void Update(int date)
    {
        _value = SetValue(date);

        int currentState = SetState();
        if (_lastState != currentState)
        {
            _lastState = currentState;
            OnSeasonChanged?.Invoke();
        }
    }

    private int SetState()
    {
        int state;
        if (IsFireIslandActive)
            state = 1;
        else if (IsIceIslandActive)
            state = 2;
        else
            state = 0;
        return state;
    }

    private float SetValue(int date)
    {
        float t = ((float)date % Data.CycleValue) / Data.CycleValue;
        return Data.SeasonCurve.Evaluate(t);
    }
}