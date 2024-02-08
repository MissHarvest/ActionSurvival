using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeightedRandomPicker<TKey>
{
    private Dictionary<TKey, float> _items = new Dictionary<TKey, float>();
    private float _sumOfWeight = 0.0f;
    public int Count => _items.Count;

    public void Print()
    {
        string line = string.Empty;
        foreach(var pair in _items)
        {
            line += $"/ [ {pair.Key.GetType()} - {pair.Value}]";
        }
        Debug.Log($"[Random Picker] {line} | {_sumOfWeight}");
    }

    public void AddItem(TKey key, float value)
    {
        if (CheckExistedItem(key) || CheckValidValue(value)) return;

        _items.TryAdd(key, value);
        _sumOfWeight += value;
    }

    public void RemoveItem(TKey key)
    {
        if(CheckExistedItem(key))
        {
            _sumOfWeight -= _items[key];
            _items.Remove(key);
        }
    }

    public void ModifyItem(TKey key, float value)
    {
        if(CheckExistedItem(key))
        {
            _sumOfWeight -= _items[key];
            _items[key] = value;
            _sumOfWeight += _items[key];
        }
    }

    public TKey AddWeightAndPick(Func<BossAttackState, bool> func, float weight)
    {
        Dictionary<TKey, float> temp = new(_items);
        var tempWeight = _sumOfWeight;
        for(int i = 0; i < _items.Count; ++i)
        {
            if (_items.Keys.ElementAt(i) is BossAttackState state)
            {
                if(func(state))
                {
                    ModifyItem(_items.Keys.ElementAt(i), _items[_items.Keys.ElementAt(i)] + weight);
                }
            }
        }
        Print();

        var result = Pick();
        _sumOfWeight = tempWeight;
        _items = temp;
        RemoveItem(result);

        return result;
    }

    private bool CheckExistedItem(TKey key)
    {
        return _items.ContainsKey(key);
    }

    private bool CheckValidValue(float value)
    {
        return value < 0;
    }

    public TKey Pick()
    {
        var chance = UnityEngine.Random.Range(0.0f, 1.0f);
        chance *= _sumOfWeight;

        var current = 0.0f;
        foreach(var pair in _items)
        {
            current += pair.Value;
            if (current > chance)
            {
                return pair.Key;
            }
        }

        return default(TKey);
    }
}
