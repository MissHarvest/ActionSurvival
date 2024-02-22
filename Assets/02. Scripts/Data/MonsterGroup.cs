using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 2024. 01. 18 Park Jun Uk
public class MonsterGroup
{
    private GameObject[] _monsterType;
    private Stack<int> _stack = new Stack<int>();
    private List<int> _list = new List<int>();

    public MonsterGroup(GameObject[] monsters, int length)
    {
        _monsterType = monsters;
        SetLength(length);
    }

    private void SetLength(int length)
    {        
        // 만들어야할 배열의 최대 길이
        for (int i = 0; i < length; ++i)
        {
            _list.Add(i % _monsterType.Length);
        }
        _list = _list.OrderBy(x => Random.Range(0.0f, 1.0f)).ToList();

        foreach(var n in _list)
        {
            _stack.Push(n);
        }
    }

    public GameObject Get()
    {
        if (_stack.Count == 0) return null;

        int type = _stack.Pop();
        return _monsterType[type];
    }

    public GameObject GetRandomMonster()
    {
        var index = Random.Range(0, _monsterType.Length);
        return index == _monsterType.Length ? null : _monsterType[index];
    }
}
