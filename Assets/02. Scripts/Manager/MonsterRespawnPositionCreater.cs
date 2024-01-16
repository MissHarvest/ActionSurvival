using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class MonsterRespawnPositionCreater : MonoBehaviour
{
    public struct Node
    {
        public bool locked;
        public bool spawn;
    }

    public GameObject monster;
    public GameObject resource;
    public int range = 10;
    public int count = 3;
    public int field = 0;

    public Node[,] nodes;

    public int monsterInterval = 1;
    private Vector3 _offset;
    private float _interval = 4.5f;
    public float boundary = 5.0f;
    private List<GameObject> _monsters = new List<GameObject>();
    private List<GameObject> _resources = new List<GameObject>();
    // lock, value ( 1 / 0 )
    private void Awake()
    {
        nodes = new Node[range, range];
        field = range * range;

        _offset = new Vector3((transform.localScale.x - boundary) / 2, 0, (transform.localScale.z - boundary) / 2);
    }

    // Start is called before the first frame update
    void Start()
    {
        int cnt = count;
        while(cnt != 0)
        {
            for (int x = 0; x < nodes.GetLength(0); ++x)
            {
                for (int z = 0; z < nodes.GetLength(1); ++z)
                {
                    nodes[x, z].locked = false;
                }
            }

            foreach(var go in _monsters)
            {
                Destroy(go);
            }

            foreach(var go in _resources)
            {
                Destroy(go);
            }

            field = range * range;
            cnt = count;
            for (int x = 0; x < nodes.GetLength(0); ++x)
            {
                for (int z = 0; z < nodes.GetLength(1); ++z)
                {
                    if (0 == cnt) return;
                    if (nodes[x, z].locked) continue;

                    // 배치할 갯수 / 남은 공간 과 랜덤 비교
                    var radio = (float)cnt / field;
                    var ran = UnityEngine.Random.Range(0.0f, 1.0f);

                    //Debug.Log($"[{x},{z}] Cube / Field {cnt} / {field} = {radio}");
                    --field;
                    nodes[x, z].locked = true;
                    if (radio < ran) continue;

                    // 배치 하기
                    var pos = new Vector3(x * _interval, 2, z * _interval) - _offset;
                    var go = Instantiate(monster, pos, Quaternion.identity);
                    _monsters.Add(go);
                    //go.name = $"[{x},{z}]";
                    --cnt;

                    // 주변에 _interval 만큼 잠구기. 본인 포함
                    LockAround(x, z);
                }

            }
        }
    }

    private void LockAround(int x, int z)
    {
        var minX = Math.Max(0, x - monsterInterval);
        var maxX = Math.Min(range - 1, x + monsterInterval);
        var minZ = Math.Max(0, z - monsterInterval);
        var maxZ = Math.Min(range - 1, z + monsterInterval);

        var origin = Mathf.Abs(x) + Mathf.Abs(z);

        try
        {
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
        catch(IndexOutOfRangeException e)
        {
            
        }
    }
}
