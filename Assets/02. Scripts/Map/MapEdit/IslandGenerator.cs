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

    private float[,] _heightMap;
    private List<(Vector3Int pos, MapData data)> _data = new();

    public List<(Vector3Int pos, MapData data)> Data => _data;


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
                        _data.Add((new Vector3Int(x - _sizeX / 2, y, z - _sizeZ / 2) + Vector3Int.RoundToInt(_position), _topBlock));
                    else 
                        _data.Add((new Vector3Int(x - _sizeX / 2, y, z - _sizeZ / 2) + Vector3Int.RoundToInt(_position), _defaultBlock));
                }
            }
        }

        yield return null;
    }

    public IEnumerator Generate()
    {
        yield return StartCoroutine(GenerateHeightMap());
        yield return StartCoroutine(GenerateData());
    }
}