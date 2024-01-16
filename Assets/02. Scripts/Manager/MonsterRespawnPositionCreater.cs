using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using UnityEngine;

// 2024. 01. 16 Park Jun Uk
public class MonsterRespawnPositionCreater : MonoBehaviour
{
    public struct Node
    {
        public bool locked;
    }

    public GameObject monster;
    public GameObject resource;
    public GameObject area;
    public int range = 10;
    public int count = 3;
    public int field = 0;

    public Node[,] nodes;

    public int monsterInterval = 1;
    private Vector3 _offset;
    private float _interval = 4.5f;
    public float boundary = 5.0f;
    [field:SerializeField] private List<GameObject> _monsters = new List<GameObject>();
    private List<GameObject> _resources = new List<GameObject>();
    public int centerRadius = 5;

    public Color[] colors;
    public int[] rankCount = new int[] { 5, 15 };

    private void Awake()
    {
        nodes = new Node[range, range];
        field = range * range;

        _offset = new Vector3((transform.localScale.x - boundary) / 2, 0, (transform.localScale.z - boundary) / 2);
    }

    private void Start()
    {
        Stopwatch watch = new Stopwatch();
        watch.Start();

        bool finished = false;
        while(!finished)
        {
            ResetData();

            var d = range / 2;
            
            for(int x = d - centerRadius; x <= d + centerRadius; ++x)
            {
                for(int z = d - centerRadius; z <= d + centerRadius; ++z)
                {
                    //Debug.Log($"Area ; [{x},{z}]");
                    nodes[x, z].locked = true;
                    var pos = new Vector3(x * _interval, 2, z * _interval) - _offset;
                    var go = Instantiate(area , pos, Quaternion.identity);
                    _resources.Add(go);
                    --field;
                }
            }

            finished = CreateMonster();
        }

        // Set Monster Rank,
        List<(int, float)> dist = new List<(int, float)>();
        for(int i = 0; i < _monsters.Count; ++i)
        {
            var d = _monsters[i].transform.position - transform.position;
            d.y = 0;
            dist.Add((i, d.sqrMagnitude));
        }

        // 거리 계산, 리스트 담기
        // sort
        dist = dist.OrderBy(x => x.Item2).ToList();

        // monster List. 순차적
        for(int i = 0; i < rankCount[0]; ++i)
        {
            _monsters[dist[i].Item1].GetComponent<Light>().color = colors[2];
        }

        for(int i = rankCount[0]; i < rankCount[0] + rankCount[1]; ++i)
        {
            _monsters[dist[i].Item1].GetComponent<Light>().color = colors[1];
        }

        watch.Stop();
        UnityEngine.Debug.Log($"{watch.ElapsedMilliseconds} ms");
    }

    private void ResetData()
    {
        for (int x = 0; x < nodes.GetLength(0); ++x)
        {
            for (int z = 0; z < nodes.GetLength(1); ++z)
            {
                nodes[x, z].locked = false;
            }
        }

        foreach (var go in _monsters)
        {
            Destroy(go);
        }
        _monsters.Clear();

        foreach (var go in _resources)
        {
            Destroy(go);
        }
        _resources.Clear();
        field = range * range;
    }

    private bool CreateMonster()
    {
        int cnt = count;
        for (int x = 0; x < nodes.GetLength(0); ++x)
        {
            for (int z = 0; z < nodes.GetLength(1); ++z)
            {
                if (0 == cnt) return true;
                if (nodes[x, z].locked) continue;

                // 배치할 갯수 / 남은 공간 과 랜덤 비교
                var radio = (float)cnt / field;
                var ran = UnityEngine.Random.Range(0.0f, 1.0f);

                //Debug.Log($"[{x},{z}] Cube / Field {cnt} / {field} = {radio}");
                --field;
                nodes[x, z].locked = true;
                if (!RepeatPercent(radio)) continue;

                // 배치 하기
                var pos = new Vector3(x * _interval, 2, z * _interval) - _offset;
                var go = Instantiate(monster, pos, Quaternion.identity);
                go.name = $"[{x},{z}]";
                _monsters.Add(go);
                
                --cnt;

                // 주변에 _interval 만큼 잠구기. 본인 포함
                LockAround(x, z);
            }
        }

        return cnt == 0;
    }

    private bool RepeatPercent(float rat)
    {
        for(int i = 0; i < 2; ++i)
        {
            var ran = UnityEngine.Random.Range(0.0f, 1.0f);
            if (rat >= ran) return true;
        }
        return false;
    }

    private void LockAround(int x, int z)
    {
        var minX = Math.Max(0, x - monsterInterval);
        var maxX = Math.Min(range - 1, x + monsterInterval);
        var minZ = Math.Max(0, z - monsterInterval);
        var maxZ = Math.Min(range - 1, z + monsterInterval);

        var origin = Mathf.Abs(x) + Mathf.Abs(z);

        for (int i = minX; i <= maxX; ++i)
        {
            for (int j = minZ; j <= maxZ; ++j)
            {
                var abs = Mathf.Abs(i - x) + Mathf.Abs(j - z);
                if (monsterInterval >= abs)
                {
                    if (nodes[i, j].locked == false) --field;
                    nodes[i, j].locked = true;
                    //Debug.Log($"LOCK [{i},{j}]");
                    var pos = new Vector3(i * _interval, 1, j * _interval) - _offset;
                    var go = Instantiate(resource, pos, Quaternion.identity);
                    _resources.Add(go);
                }
            }
        }
    }
}
