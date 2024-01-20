using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 2024. 01. 18 Park Jun Uk
public class MonsterGroup
{
    private List<GameObject> _monsterType = new List<GameObject>();
    private Stack<int> _stack = new Stack<int>();
    private List<int> _list = new List<int>();

    public void AddMonsterType(string[] monsterNames)
    {
        // 그룹에 담을 몬스터 종류
        for (int i = 0; i < monsterNames.Length; ++i)
        {
            _monsterType.Add(Managers.Resource.GetCache<GameObject>($"{monsterNames[i]}.prefab"));
        }
        Debug.Log($"Monster Type : [{_monsterType.Count}]");
    }

    public void SetLength(int length)
    {        
        // 만들어야할 배열의 최대 길이
        for (int i = 0; i < length; ++i)
        {
            _list.Add(i % _monsterType.Count);
        }
        _list = _list.OrderBy(x => Random.Range(0.0f, 1.0f)).ToList();

        foreach(var n in _list)
        {
            _stack.Push(n);
        }

        Debug.Log($"Stack [{_stack.Count}");
    }

    public GameObject Get()
    {
        if (_stack.Count == 0) return null;

        int type = _stack.Pop();
        return _monsterType[type];
    }

    public GameObject GetRandomMonster()
    {
        var index = Random.Range(0, _monsterType.Count);
        return _monsterType[index];
    }
}
