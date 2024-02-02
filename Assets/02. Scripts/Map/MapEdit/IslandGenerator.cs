using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024-02-01 WJY
public class IslandGernerator : MonoBehaviour
{
    [Header("Testing Default")]
    [SerializeField] private GameObject _cubePrefab;
    [SerializeField] private GameObject _root;
    [SerializeField] private bool _showTestCube;

    [Header("Data Reference")]
    [SerializeField] private WorldData _worldData;
    [SerializeField] private VoxelData _voxelData;

    [Header("Island Info")]
    [SerializeField] private int _sizeX;
    [SerializeField] private int _sizeZ;
    [SerializeField] private Vector3 _position;

    [Header("Height Settings")]
    [SerializeField] private AnimationCurve _heightCurve;               // 섬의 중앙 ~ 끄트머리 부분 높이 값
    [Range(1f, 30f)] [SerializeField] private float _heightCurveScale;

    [Header("Noise Settings")]
    [Range(0.0001f, 10f)] [SerializeField] private float _noiseScale;
    [SerializeField] private int _octaves = 1;          // 노이즈 중첩 횟수
    [SerializeField] private float _lacunarity = 1f;    // 중첩 당 주파수 배율
    [SerializeField] private float _persistence = 1f;   // 중첩 당 진폭 배율
    [SerializeField] private int _seed = 1;             // 의사 난수 시드

    [Header("Block Settings")]
    [SerializeField] private MapData _defaultBlock;
    [SerializeField] private MapData _topBlock;
    [SerializeField] private MapData _slideBlock;

    private float[,] _heightMap;
    private Dictionary<Vector3Int, MapData> _data = new();

    public Dictionary<Vector3Int, MapData> Data => _data;


    private IEnumerator Start()
    {
        yield return StartCoroutine(GenerateHeightMap());
        if (_showTestCube)
            yield return StartCoroutine(TestCubeCreate());
    }

    private IEnumerator GenerateHeightMap()
    {
        _heightMap = new float[_sizeX, _sizeZ];

        // 의사 난수 생성
        System.Random pseudoRandom = new(_seed.GetHashCode());
        float seedX = pseudoRandom.Next(0, 99999);
        float seedZ = pseudoRandom.Next(0, 99999);

        // Height Curve를 사용하기 위해 섬 길이 계산
        Vector2 center = new(_sizeX/2, _sizeZ/2);
        float radius = Mathf.Max(Vector2.Distance(center, new(_sizeX / 2, 0)), Vector2.Distance(center, new(0, _sizeZ / 2)));

        for (int x = 0; x < _sizeX; x++)
        {
            for (int z = 0; z < _sizeZ; z++) 
            {
                float amplitude = 1f;   // 진폭
                float frequency = 1f;   // 주파수
                float noiseHeight = 0f;

                // 옥타브만큼 노이즈 중첩
                for (int i = 0; i < _octaves; i++)
                {
                    float coordX = seedX + x * _noiseScale * frequency;
                    float coordZ = seedZ + z * _noiseScale * frequency;
                    float noise = Mathf.PerlinNoise(coordX, coordZ);
                    noiseHeight += noise * amplitude;
                    amplitude *= _persistence;
                    frequency *= _lacunarity;
                }

                // 계산된 펄린노이즈에 HeightCurve 높이 추가
                Vector2 pos = new(x, z);
                float t = Mathf.Clamp01(Vector2.Distance(pos, center) / radius);
                float curveHeight = _heightCurve.Evaluate(t) * _heightCurveScale;

                _heightMap[x, z] = noiseHeight + curveHeight;
            }
        }

        yield return null;
    }

    private IEnumerator TestCubeCreate()
    {
        for (int x = 0; x < _sizeX; x++)
        {
            for (int z = 0; z < _sizeZ; z++)
            {
                var obj = Instantiate(_cubePrefab, new Vector3(x - _sizeX / 2, (int)_heightMap[x, z], z - _sizeZ / 2), Quaternion.identity);
                obj.transform.position += _position;
                obj.transform.parent = _root.transform;
            }
        }

        yield return null;
    }

    private IEnumerator GenerateData()
    {
        for (int x = 0; x < _sizeX; x++)
        {
            for (int z = 0; z < _sizeZ; z++)
            {
                for (int y = 0; y < _heightMap[x, z]; y++)
                {
                    if (y == (int)_heightMap[x, z])
                        _data.Add(new Vector3Int(x - _sizeX / 2, y, z - _sizeZ / 2) + Vector3Int.RoundToInt(_position), _topBlock);
                    else 
                        _data.Add(new Vector3Int(x - _sizeX / 2, y, z - _sizeZ / 2) + Vector3Int.RoundToInt(_position), _defaultBlock);
                }
            }
        }

        yield return null;
    }

    // 임시... 쓸모없는 블럭들은 지우도록 했습니다.
    private IEnumerator DataPostProcess()
    {
        Dictionary<Vector3Int, WorldMapData> worldMap = new();
        foreach (var data in _data)
        {
            var worldMapData = new WorldMapData()
            {
                type = _worldData.GetType(data.Value.type)[data.Value.typeIndex],
                position = data.Key,
                forward = data.Value.forward,
            };
            worldMap.TryAdd(data.Key, worldMapData);
        }

        Vector3Int[] check = new Vector3Int[4]
        {
            Vector3Int.forward,
            Vector3Int.back,
            Vector3Int.left,
            Vector3Int.right,
        };

        // TSET ==================
        foreach (var block in worldMap.Values)
        {
            Vector3Int? pos = null;
            Vector3Int? forward = null;
            int openCount = 0;
            for (int i = 0; i < 4; i++)
            {
                Vector3Int checkPos = block.position + check[i];
                // 바라보는 칸이 아예 비었음.
                // 그 아래칸은 Solid임
                // 그 아래칸의 바라보는 칸도 Solid임

                if (worldMap.ContainsKey(checkPos) ||
                    !worldMap.TryGetValue(checkPos + Vector3Int.down, out var checkBlock) ||
                    !worldMap.TryGetValue(checkPos + Vector3Int.down + check[i], out var checkBlock2))
                    continue;

                if (checkBlock.type.IsSolid && checkBlock2.type.IsSolid)
                {
                    pos = checkPos;
                    forward = -check[i];
                    openCount++;
                }
            }

            if (pos.HasValue && forward.HasValue && openCount == 1)
            {
                MapData newSlideBlcokData = new()
                {
                    forward = forward.Value,
                    type = _slideBlock.type,
                    typeIndex = _slideBlock.typeIndex,
                };
                _data.TryAdd(pos.Value, newSlideBlcokData);
            }
        }
        //========================

        HashSet<Vector3Int> deleteKey = new();
        foreach (var data in worldMap.Values)
        {
            bool flag = true;
            for (int i = 0; i < 6; i++)
            {
                Vector3Int pos = Vector3Int.FloorToInt(data.position + _voxelData.faceChecks[i]);

                if (!worldMap.ContainsKey(pos))
                {
                    flag = false;
                    break;
                }
                else if (!worldMap[pos].type.IsSolid)
                {
                    flag = false;
                    break;
                }
            }

            if (flag)
                deleteKey.Add(data.position);
        }

        Debug.Log($"delete blocks : {deleteKey.Count}");

        foreach (var key in deleteKey)
            _data.Remove(key);

        yield return null;
    }

    public IEnumerator Generate()
    {
        yield return StartCoroutine(GenerateHeightMap());
        yield return StartCoroutine(GenerateData());
        yield return StartCoroutine(DataPostProcess());
    }
}