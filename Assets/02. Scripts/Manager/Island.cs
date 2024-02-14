using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// 2024. 01. 18 Park Jun Uk
public enum MonsterLevel
{
    Lower,    
    Middle,
    Upper,
}

public class SpawnPoint
{
    public Vector3 point;
    public MonsterLevel level;

    public SpawnPoint(Vector3 point, MonsterLevel level = MonsterLevel.Lower)
    {
        this.point = point;
        this.level = level;
    }
}

public class Island
{
    public string BossName = string.Empty;
    private GameObject _boss;

    private string name;
    // 각 등급 별 몬스터 위치
    public int cellCount = 61;

    // 스폰할 몬스터 종류

    private Vector3 _offset;
    private readonly int _interval = 5;
    private LayerMask _unspawnableLayers = 64;

    private IslandProperty _property;
    public int[] rankCount = new int[] { 30, 15, 5 };
    private int _centerRadius = 10;
    private MonsterGroup[] _monsterGroups = new MonsterGroup[3];
    private Transform _islandMonsterRoot;

    [SerializeField] private List<SpawnPoint> _spawnablePoints = new List<SpawnPoint>();

    public List<GameObject> DiedMonsters = new List<GameObject>();

    public Island(IslandProperty property)
    {
        _property = property;
        _offset = new Vector3(property.diameter / 2, 0, property.diameter / 2) - _property.center;

        for (int i = 0; i < _monsterGroups.Length; ++i)
        {
            _monsterGroups[i] = new MonsterGroup();
        }
    }

    public Transform SpawnMonsterRoot
    {
        get
        {
            if (_islandMonsterRoot == null)
                _islandMonsterRoot = new GameObject(_property.name).transform;
            return _islandMonsterRoot;
        }
    }

    public IslandProperty Property => _property;
    public float Temperature
    {
        get => _property.Temperature; 
        set 
        {
            _property.Temperature = value;
            Managers.Game.Temperature.OnTemperatureChange(); 
        }
    }
    public float Influence
    {
        get => _property.Influence;
        set 
        {
            _property.Influence = value;
            Managers.Game.Temperature.OnTemperatureChange();
        }
    }

    public GameObject Spawn()
    {
        var index = Random.Range(0, _spawnablePoints.Count);

        // 일단은 하급 생성
        var monster = _monsterGroups[(int)MonsterLevel.Lower].GetRandomMonster();
        if (monster == null) return null;

        var pos = _spawnablePoints[index].point;
        pos.y = 0.5f;
        var monsterObj = Object.Instantiate(monster, pos, Quaternion.identity);
        monsterObj.name = monsterObj.name + "[Extra]";
        return monsterObj;
    }

    private void RespawnMonsters()
    {
        // [ Do ] // Set Monster RespawnDuration. (day)
        if (DiedMonsters.Count == 0) return;

        for (int i = 0; i < DiedMonsters.Count; ++i)
        {
            DiedMonsters[i].SetActive(true);
            DiedMonsters[i].GetComponent<Monster>().Respawn();
        }
        DiedMonsters.Clear();
    }

    public void AddMonsterType(string[][] monsterNames)
    {
        for (int i = 0; i < monsterNames.Length; ++i)
        {
            _monsterGroups[i].AddMonsterType(monsterNames[i]);
            _monsterGroups[i].SetLength(rankCount[i]);
        }
    }

    private void Load()
    {
        SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Compile, $"{name}Island");
    }

    private void Save()
    {
        var json = JsonUtility.ToJson(this);
        SaveGame.CreateJsonFile($"{name}Island", json, SaveGame.SaveType.Runtime);
    }

    #region SpawnPoint
    public void CreateMonsters()
    {
        Managers.Game.DayCycle.OnMorningCame += RespawnMonsters;

        CreateSpawnPoint();

        SetSpawnPointRank();

        //Save();

        // SpawnMonster
        for (int i = 0; i < _spawnablePoints.Count; ++i)
        {
            var pos = _spawnablePoints[i].point;
            pos.y = 50;
            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.down, out hit, 100.0f, 1 << 12))
            {
                pos = hit.point;
                pos.y += 0.5f;

                var mon = _monsterGroups[(int)_spawnablePoints[i].level].Get();
                var go = Object.Instantiate(mon, pos, Quaternion.identity);
                go.transform.SetParent(SpawnMonsterRoot);   
                go.GetComponent<Monster>().SetIsland(this);
                go.name = $"{mon.name}[{_spawnablePoints[i].level}]";
            }
        }

        if(BossName != string.Empty)
        {            
            if(Physics.Raycast(_property.center + Vector3.up * 100, Vector3.down, out RaycastHit hit, 200, 1 << 12))
            {
                var prefab = Managers.Resource.GetCache<GameObject>($"{BossName}.prefab");
                var pos = new Vector3(_property.center.x, prefab.transform.position.y + hit.point.y, _property.center.z);
                _boss = Object.Instantiate(prefab, pos, prefab.transform.rotation);
            }
        }
    }

    private void CreateSpawnPoint()
    {
        bool[,] points = new bool[cellCount, cellCount];
        var field = cellCount * cellCount;
        LockUnusablePoint(points, ref field);
        LockBoundaryPoint(points, ref field);
        LockOtherObjectPoint(points, ref field);
        LockCenterArea(points, ref field);
        LoopCreateMonsterSpawnPoint(points, field);
    }

    private void LockUnusablePoint(bool[,] points, ref int field)
    {
        float maxDistance = 100;
        for (int x = 0; x < cellCount; ++x)
        {
            for (int z = 0; z < cellCount; ++z)
            {
                RaycastHit hit;
                var start = new Vector3(x * _interval, 10, z * _interval) - _offset;
                if (!Physics.BoxCast(start, Vector3.one * 0.5f, Vector3.down, out hit, Quaternion.identity, maxDistance))
                {
                    points[x, z] = true;
                    --field;
                }
            }
        }
    }

    private void LockOtherObjectPoint(bool[,] points, ref int field)
    {
        float maxDistance = 100;
        for (int x = 0; x < cellCount; ++x)
        {
            for (int z = 0; z < cellCount; ++z)
            {
                RaycastHit hit;
                var start = new Vector3(x * _interval, 10, z * _interval) - _offset;
                if (Physics.BoxCast(start, Vector3.one * 0.5f, Vector3.down, out hit, Quaternion.identity, maxDistance))
                {
                    if (_unspawnableLayers == (_unspawnableLayers | 1 << hit.collider.gameObject.layer))
                    {
                        points[x, z] = true;
                        --field;
                    }
                }
            }
        }
    }

    private void LockBoundaryPoint(bool[,] points, ref int field)
    {
        int boundary = _property.boundary;

        Vector2[] directions = { new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1) };
        List<int> directionIndexs = new List<int>();

        while (boundary != 0)
        {
            if (TryGetBoundaryStartPoint(points, out Vector2 start) == false) return;

            var current = start;
            bool stop = false;
            int forward = 0;

            int count = 0;

            while (!stop)
            {
                if (count == 10) break;
                ++count;

                points[(int)current.x, (int)current.y] = true;
                --field;

                AddForwardSideIndex(directionIndexs, forward);

                for (int i = 0; i < directionIndexs.Count; ++i)
                {
                    var next = current + directions[directionIndexs[i]];

                    if (next.x < 0 || next.x >= cellCount || next.y < 0 || next.y >= cellCount) continue;

                    if (points[(int)(next.x), (int)(next.y)] == false)
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

            --boundary;
        }
    }

    private bool TryGetBoundaryStartPoint(bool[,] points, out Vector2 start)
    {
        for (int x = 0; x < points.GetLength(0); ++x)
        {
            for (int z = 0; z < points.GetLength(1); ++z)
            {
                if (points[x, z] == false)
                {
                    start = new Vector2(x, z);
                    return true;
                }
            }
        }
        start = Vector2.zero;
        return false;
    }

    private void AddForwardSideIndex(List<int> list, int forward)
    {
        list.Clear();
        for (int i = 0; i < 3; ++i)
        {
            list.Add((forward + 3 + i) % 4);
        }
    }

    private void LockCenterArea(bool[,] points, ref int field)
    {
        var d = cellCount / 2;

        for (int x = d - _centerRadius; x <= d + _centerRadius; ++x)
        {
            for (int z = d - _centerRadius; z <= d + _centerRadius; ++z)
            {
                if (points[x, z] == false)
                {
                    points[x, z] = true;
                    --field;
                }
            }
        }
    }

    private void LoopCreateMonsterSpawnPoint(bool[,] points, int field)
    {
        bool finished = false;
        int count = 0;

        while (!finished)
        {
            _spawnablePoints.Clear();
            finished = CreateMonsterSpawnPoint(points, field);
            ++count;
            if (count == 10) break;
        }
    }

    private bool CreateMonsterSpawnPoint(bool[,] points, int field)
    {
        int cnt = _property.monsterCount;
        bool[,] copyPoint = (bool[,])points.Clone();

        for (int x = 0; x < copyPoint.GetLength(0); ++x)
        {
            for (int z = 0; z < copyPoint.GetLength(1); ++z)
            {
                if (copyPoint[x, z]) continue;

                if (CheckSpawnablePercentage(cnt, field))
                {
                    var point = new Vector3(x * _interval, 10, z * _interval) - _offset;
                    SpawnPoint spawnPoint = new SpawnPoint(point);
                    _spawnablePoints.Add(spawnPoint);
                    --cnt;
                    LockMonsterAroundPoint(copyPoint, x, z, ref field);
                }
                --field;
                copyPoint[x, z] = true;
            }
        }

        return cnt == 0;
    }

    private bool CheckSpawnablePercentage(int cnt, int field)
    {
        var percentage = (float)cnt / field;

        for (int i = 0; i < 2; ++i)
        {
            var ran = UnityEngine.Random.Range(0.0f, 1.0f);
            if (percentage >= ran) return true;
        }
        return false;
    }

    private void LockMonsterAroundPoint(bool[,] points, int x, int z, ref int field)
    {
        var monsterInterval = _property.monsterInterval;
        var minX = System.Math.Max(0, x - monsterInterval);
        var maxX = System.Math.Min(cellCount - 1, x + monsterInterval);
        var minZ = System.Math.Max(0, z - monsterInterval);
        var maxZ = System.Math.Min(cellCount - 1, z + monsterInterval);

        for (int i = minX; i <= maxX; ++i)
        {
            for (int j = minZ; j <= maxZ; ++j)
            {
                var abs = Mathf.Abs(i - x) + Mathf.Abs(j - z);
                if (monsterInterval >= abs)
                {
                    if (points[i, j] == false) --field;
                    points[i, j] = true;
                }
            }
        }
    }

    private void SetSpawnPointRank()
    {
        List<(int, float)> dist = new List<(int, float)>();

        for (int i = 0; i < _spawnablePoints.Count; ++i)
        {
            var d = _spawnablePoints[i].point - _property.center;
            d.y = 0;
            dist.Add((i, d.sqrMagnitude));
        }

        // 거리 계산, 리스트 담기
        // sort
        dist = dist.OrderBy(x => x.Item2).ToList();
        
        int adaptedLevel = 2;
        int cnt = rankCount[adaptedLevel];
        for(int i = 0; i < dist.Count; ++i)
        {
            if(i >= cnt)
            {
                cnt += rankCount[--adaptedLevel];
            }
            _spawnablePoints[dist[i].Item1].level = (MonsterLevel)adaptedLevel;
        }
    }
    #endregion
}
