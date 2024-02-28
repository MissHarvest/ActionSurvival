using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Jobs;
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

[System.Serializable]
public class Island
{
    private int _cellCount = 61;
    private int[] _rankCount = new int[3];
    private Transform _islandMonsterRoot;
    private IslandSO _islandSO;
    private GameObject _bossMonster;
    private MonsterGroup[] _monsterGroups = new MonsterGroup[3];    
    private List<SpawnPoint> _spawnablePoints = new List<SpawnPoint>();
    private List<GameObject> _diedMonsters = new List<GameObject>(); // >> 오브젝 풀링
    
    [SerializeField] private float _temperature;
    [SerializeField] private float _influence;
    [SerializeField] private bool _bossMonsterDied;

    public Transform SpawnMonsterRoot
    {
        get
        {
            if (_islandMonsterRoot == null)
                _islandMonsterRoot = new GameObject(_islandSO.Property.name).transform;
            return _islandMonsterRoot;
        }
    }
    public IslandProperty Property => _islandSO.Property;
    public float Temperature
    {
        get => _temperature;
        set
        {
            _temperature = value;
            GameManager.Temperature.OnTemperatureChange();
        }
    }
    public float Influence
    {
        get => _influence;
        set
        {
            _influence = value;
            GameManager.Temperature.OnTemperatureChange();
        }
    }
    public string Name => _islandSO.Property.name;
    public Island(IslandSO islandSO)
    {
        this._islandSO = islandSO;
        this._temperature = _islandSO.Property.Temperature;
        this._influence = _islandSO.Property.Influence;

        var totalCount = islandSO.Monster.SpawnData.MonsterCount;
        _rankCount[0] = Mathf.RoundToInt(islandSO.Monster.Distribution.LowerMonsterRatio * totalCount);
        _rankCount[1] = Mathf.RoundToInt(islandSO.Monster.Distribution.MiddleMonsterRatio * totalCount);
        _rankCount[2] = Mathf.RoundToInt(islandSO.Monster.Distribution.UpperMonsterRatio * totalCount);
        
        _monsterGroups[0] = new MonsterGroup(islandSO.Monster.Distribution.LowerMonsters, _rankCount[0]);
        _monsterGroups[1] = new MonsterGroup(islandSO.Monster.Distribution.MiddleMonsters, _rankCount[1]);
        _monsterGroups[2] = new MonsterGroup(islandSO.Monster.Distribution.UpperMonsters, _rankCount[2]);

        GameManager.Instance.OnSaveCallback += Save;
        GameManager.DayCycle.OnMorningCame += RespawnMonsters;
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
        if (_diedMonsters.Count == 0) return;
        
        for (int i = 0; i < _diedMonsters.Count; ++i)
        {
            _diedMonsters[i].SetActive(true);
            _diedMonsters[i].GetComponent<Monster>().Respawn();
        }
        _diedMonsters.Clear();
    }

    public void Release(GameObject gameObject)
    {
        _diedMonsters.Add(gameObject);
    }

    #region SpawnPoint
    private void CheckSpawnablePoint()
    {
        CreateSpawnPoint();
        SetSpawnPointRank();
    }

    private void CreateMonsters()
    {
        for (int i = 0; i < _spawnablePoints.Count; ++i)
        {
            var pos = _spawnablePoints[i].point;
            //Debug.Log($"[Monster] |{_islandSO.Property.name}||SpawnablePoint| {pos}");
            pos.y = 50;
            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.down, out hit, 100.0f, 1 << 12))
            {
                var point = hit.point;
                point.y += 0.5f;
                //Debug.Log($"[Monster] |RayCast||SpawnablePoint| {point}");
                var mon = _monsterGroups[(int)_spawnablePoints[i].level].Get();
                var go = Object.Instantiate(mon, point, Quaternion.identity);
                go.transform.SetParent(SpawnMonsterRoot);
                go.GetComponent<Monster>().SetIsland(this);
                go.name = $"{mon.name}[{_spawnablePoints[i].level}]";

                //Debug.Log($"[Monster] |{go.name}|{point}");
            }
        }
    }

    private void CreateBossMonster()
    {
        if (_bossMonsterDied) return;
        var bossPrefab = _islandSO.Monster.BossMonster;
        if (bossPrefab == null) return;
        var center = _islandSO.Property.Center;

        if (Physics.Raycast(center + Vector3.up * 100, Vector3.down, out RaycastHit hit, 200, 1 << 12))
        {
            var pos = new Vector3(center.x, bossPrefab.transform.position.y + hit.point.y, center.z);
            _bossMonster = Object.Instantiate(bossPrefab, pos, bossPrefab.transform.rotation);
            _bossMonsterDied = false;
        }
    }

    private void CreateSpawnPoint()
    {
        bool[,] points = new bool[_cellCount, _cellCount];
        var field = _cellCount * _cellCount;
        LockUnusablePoint(points, ref field);
        LockBoundaryPointJob(points, ref field);
        LockOtherObjectPoint(points, ref field);
        LoopFixMonsterSpawnPoint(points, field);
    }

    private void LockUnusablePoint(bool[,] points, ref int field)
    {
        float maxDistance = 100;
        int interval = _islandSO.Monster.SpawnData.Interval;

        for (int x = 0; x < _cellCount; ++x)
        {
            for (int z = 0; z < _cellCount; ++z)
            {
                RaycastHit hit;
                var start = new Vector3(x * interval, 10, z * interval) - _islandSO.Property.Offset;
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
        int interval = _islandSO.Monster.SpawnData.Interval;

        for (int x = 0; x < _cellCount; ++x)
        {
            for (int z = 0; z < _cellCount; ++z)
            {
                RaycastHit hit;
                var start = new Vector3(x * interval, 10, z * interval) - _islandSO.Property.Offset;
                if (Physics.BoxCast(start, Vector3.one * 0.5f, Vector3.down, out hit, Quaternion.identity, maxDistance))
                {
                    if (_islandSO.Monster.SpawnData.UnSpawnableLayer == (_islandSO.Monster.SpawnData.UnSpawnableLayer | 1 << hit.collider.gameObject.layer))
                    {
                        points[x, z] = true;
                        --field;
                    }
                }
            }
        }
    }

    private void LockBoundaryPointJob(bool[,] points, ref int field)
    {
        LockBoundaryJob job = new LockBoundaryJob(
            points, 
            field, 
            _islandSO.Monster.SpawnData.Boundry,
            _islandSO.Monster.SpawnData.CenterRadius);

        var handle = job.Schedule();

        handle.Complete();
        var result = job.GetResult();
        
        for(int i = 0; i < points.GetLength(0); ++i)
        {
            for(int j = 0; j < points.GetLength(1); ++j)
            {
                points[i, j] = result.points[i, j];
            }
        }
        field = result.field;
    }

    private void LoopFixMonsterSpawnPoint(bool[,] points, int field)
    {
        UsableMonsterSpawnPoints _usablePoints = new();
        /*
        FixMonsterSpawnPointJob job = new FixMonsterSpawnPointJob(
            points,
            field,
            _islandSO.Monster.SpawnData.MonsterCount,
            _islandSO.Monster.SpawnData.Monsterinterval,
            _islandSO.Monster.SpawnData.Interval,
            GetHashCode());

        var handle = job.Schedule();
        handle.Complete();

        UnityEngine.Debug.Log($"[While Count]{job.whileCount[0]}");

        var result = job.GetResult();
        if (result[0] == Vector3.zero)
        {
            result = _usablePoints.Get();
        }
        */
        var result = _usablePoints.Get();
        for (int i = 0; i < result.Length; ++i)
        {
            _spawnablePoints.Add(new SpawnPoint(
                result[i] - _islandSO.Property.Offset));
        }
        //job.whileCount.Dispose();
    }

    private void SetSpawnPointRank()
    {
        List<(int, float)> dist = new List<(int, float)>();

        for (int i = 0; i < _spawnablePoints.Count; ++i)
        {
            var d = _spawnablePoints[i].point - _islandSO.Property.Center;
            d.y = 0;
            dist.Add((i, d.sqrMagnitude));
        }

        // 거리 계산, 리스트 담기
        // sort
        dist = dist.OrderBy(x => x.Item2).ToList();
        
        int adaptedLevel = 2;
        int cnt = _rankCount[adaptedLevel];
        for(int i = 0; i < dist.Count; ++i)
        {
            if(i >= cnt)
            {
                cnt += _rankCount[--adaptedLevel];
            }
            _spawnablePoints[dist[i].Item1].level = ((MonsterLevel)adaptedLevel);
        }
    }
    #endregion

    public void Load()
    {
        SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, Name);
        Stopwatch watch = new();
        watch.Start();
        CheckSpawnablePoint();
        CreateMonsters();
        CreateBossMonster();
    }

    private void Save()
    {
        string json = JsonUtility.ToJson(this);
        SaveGame.CreateJsonFile($"{Name}", json, SaveGame.SaveType.Runtime);
    }
}
