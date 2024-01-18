using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;

// 2024. 01. 16 Park Jun Uk
public class MonsterRespawnPositionCreater : MonoBehaviour
{
    [Header("Ground Size")]
    public Vector3 isolationCenter;
    public int isolationRadius = 300; // World Size
    public int boundary = 3;
    public struct Node
    {
        public bool locked;
    }

    public GameObject monster;
    public GameObject resource;
    public GameObject area;
    public int range = 10;
    public int count = 3; // 배치할 몬스터 수
    private int _field = 0;

    public Node[,] nodes;

    public int monsterInterval = 1;
    private Vector3 _offset;
    private int _interval;
    
    [field:SerializeField] private List<GameObject> _monsters = new List<GameObject>();
    private List<GameObject> _resources = new List<GameObject>();
    public int centerRadius = 5;

    public Color[] colors;
    public int[] rankCount = new int[] { 5, 15 };
    public LayerMask spawnLockLayers;

    private void Awake()
    {
        nodes = new Node[range, range];
        _field = range * range;

        _interval = 5;// (isolationRadius-1) / range;

        _offset = new Vector3(isolationRadius / 2, 0, isolationRadius/ 2);
    }

    private void Start()
    {
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        CheckUnusableNodes();
        
        LockBoundary();

        LockCenter();

        LoopCreateMonster();

        SetMonsterRank();

        watch.Stop();
        UnityEngine.Debug.Log($"{watch.ElapsedMilliseconds} ms");
    }

    private void SetMonsterRank()
    {
        List<(int, float)> dist = new List<(int, float)>();
        for (int i = 0; i < _monsters.Count; ++i)
        {
            var d = _monsters[i].transform.position - transform.position;
            d.y = 0;
            dist.Add((i, d.sqrMagnitude));
        }

        // 거리 계산, 리스트 담기
        // sort
        dist = dist.OrderBy(x => x.Item2).ToList();

        // monster List. 순차적
        for (int i = 0; i < rankCount[0]; ++i)
        {
            _monsters[dist[i].Item1].GetComponent<Light>().color = colors[2];
        }

        for (int i = rankCount[0]; i < rankCount[0] + rankCount[1]; ++i)
        {
            _monsters[dist[i].Item1].GetComponent<Light>().color = colors[1];
        }
    }

    private void LoopCreateMonster()
    {
        bool finished = false;
        while (!finished)
        {
            ResetData();
            Debug.Log($"Create Monster Start[{_field}]");
            finished = CreateMonster((Node[,])nodes.Clone(), _field);
            Debug.Log($"Create Monster End[{_field}] [{finished}]");
        }
    }

    private void CheckUnusableNodes()
    {
        float maxDistance = 100;
        for (int x = 0; x < nodes.GetLength(0); ++x)
        {
            for(int z = 0; z < nodes.GetLength(1); ++z)
            {
                RaycastHit hit;
                var start = new Vector3(x * _interval, 10, z * _interval) - _offset;
                if(!Physics.BoxCast(start, Vector3.one * 0.5f, Vector3.down, out hit, Quaternion.identity, maxDistance))
                {
                    nodes[x, z].locked = true;
                    --_field;
                }
                else
                {
                    if(spawnLockLayers == (spawnLockLayers | 1 << hit.collider.gameObject.layer))
                    {
                        nodes[x, z].locked = true;
                        --_field;
                    }
                }
            }
        }
    }

    private void LockBoundary()
    {
        int cnt = boundary;
        Vector2[] directions = { new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1) };
        List<int> directionIndexs = new List<int>();

        while (cnt != 0)
        {
            //UnityEngine.Debug.Log($"=========={cnt}===========");
            if(TryGetBoundaryStartPoint(out Vector2 start)== false) return;
            

            var current = start;
            bool stop = false;
            int forward = 0;
                        
            while (!stop)
            {
                //UnityEngine.Debug.Log($"Current Position [{current.x}, {current.y}]");
                nodes[(int)current.x, (int)current.y].locked = true;
                --_field;
                GetIndexsToDecideDirection(directionIndexs, forward);

                for (int i = 0; i < directionIndexs.Count; ++i)
                {
                    var next = current + directions[directionIndexs[i]];
                    //UnityEngine.Debug.Log($"Check Position [{next.x}, {next.y}]");
                    if (next.x < 0 || next.x >= range || next.y < 0 || next.y >= range) continue;
                                        
                    if (nodes[(int)(next.x), (int)(next.y)].locked == false)
                    {
                        current = next;
                        forward = directionIndexs[i];
                        break;
                    }
                    else
                    {
                        if (start == current + directions[directionIndexs[i]])
                        {
                            stop = true;
                            break;
                        }
                    }
                }
            }

            --cnt;
        }
    }

    private void LockCenter()
    {
        var d = range / 2;

        for (int x = d - centerRadius; x <= d + centerRadius; ++x)
        {
            for (int z = d - centerRadius; z <= d + centerRadius; ++z)
            {
                //Debug.Log($"Area ; [{x},{z}]");
                if (nodes[x, z].locked == false)
                {
                    nodes[x, z].locked = true;                    
                    --_field;
                }
                
                var pos = new Vector3(x * _interval, 2, z * _interval) - _offset;
                Instantiate(area, pos, Quaternion.identity);
            }
        }
    }

    private bool TryGetBoundaryStartPoint(out Vector2 start)
    {
        for (int x = 0; x < nodes.GetLength(0); ++x)
        {
            for (int z = 0; z < nodes.GetLength(1); ++z)
            {
                if (nodes[x, z].locked == false)
                {
                    start = new Vector2(x, z);
                    return true;
                }
            }
        }
        start = Vector2.zero;
        return false;
    }

    private void GetIndexsToDecideDirection(List<int> list, int forward)
    {
        list.Clear();
        for (int i = 0; i < 3; ++i)
        {
            list.Add((forward + 3 + i) % 4);
        }
    }

    private void ResetData()
    {
        foreach (var go in _monsters)
        {
            Destroy(go);
        }
        _monsters.Clear();

        foreach(var go in _resources)
        {
            Destroy(go);
        }
        _resources.Clear();
    }

    private bool CreateMonster(Node[,] usableNodes, int field)
    {
        int cnt = count;
        for (int x = 0; x < usableNodes.GetLength(0); ++x)
        {
            for (int z = 0; z < usableNodes.GetLength(1); ++z)
            {
                //if (0 == cnt) return true;
                if (usableNodes[x, z].locked) continue;

                // 배치할 갯수 / 남은 공간 과 랜덤 비교
                var radio = (float)cnt / field;

                //Debug.Log($"[{x},{z}] Cube / Field {cnt} / {field} = {radio}");
                --field;
                usableNodes[x, z].locked = true;
                if (!RepeatPercent(radio)) continue;

                // 배치 하기
                var pos = new Vector3(x * _interval, 2, z * _interval) - _offset;
                var go = Instantiate(monster, pos, Quaternion.identity);
                go.name = $"[{x},{z}]";
                _monsters.Add(go);
                
                --cnt;

                // 주변에 _interval 만큼 잠구기. 본인 포함
                LockAround(ref usableNodes, x, z, ref field);
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

    private void LockAround(ref Node[,] usableNodes, int x, int z, ref int field)
    {
        var minX = System.Math.Max(0, x - monsterInterval);
        var maxX = System.Math.Min(range - 1, x + monsterInterval);
        var minZ = System.Math.Max(0, z - monsterInterval);
        var maxZ = System.Math.Min(range - 1, z + monsterInterval);

        var origin = Mathf.Abs(x) + Mathf.Abs(z);

        for (int i = minX; i <= maxX; ++i)
        {
            for (int j = minZ; j <= maxZ; ++j)
            {
                var abs = Mathf.Abs(i - x) + Mathf.Abs(j - z);
                if (monsterInterval >= abs)
                {
                    if (usableNodes[i, j].locked == false) --field;
                    usableNodes[i, j].locked = true;
                    //Debug.Log($"LOCK [{i},{j}]");
                    var pos = new Vector3(i * _interval, 1, j * _interval) - _offset;
                    var go = Instantiate(resource, pos, Quaternion.identity);
                    _resources.Add(go);
                }
            }
        }
    }
}
