using System.Collections;
using UnityEditor;
using UnityEngine;

// 2024-02-06 WJY
public class IslandObjectGenerator : MonoBehaviour
{
    [Header("Prefab Settings")]
    [SerializeField] private GameObject _prefab;

    [Header("Island Settings")]
    [SerializeField] private int _sizeX;
    [SerializeField] private int _sizeZ;
    [SerializeField] private Vector3 _position;

    [Header("Threshold Settings")]
    [Range(0f, 1f)][SerializeField] private float _threshold;

    [Header("Noise Settings")]
    [Range(0.0001f, 10f)][SerializeField] private float _noiseScale;
    [SerializeField] private int _octaves = 1;          // 노이즈 중첩 횟수
    [SerializeField] private float _lacunarity = 1f;    // 중첩 당 주파수 배율
    [SerializeField] private float _persistence = 1f;   // 중첩 당 진폭 배율
    [SerializeField] private int _seed = 1;             // 의사 난수 시드

    private float[,] _noiseMap;

    public IEnumerator Start()
    {
        yield return StartCoroutine(GenerateHeightMap());
        yield return StartCoroutine(InstantiateObject());
    }

    private IEnumerator GenerateHeightMap()
    {
        _noiseMap = new float[_sizeX, _sizeZ];

        // 의사 난수 생성
        System.Random pseudoRandom = new(_seed.GetHashCode());
        float seedX = pseudoRandom.Next(0, 99999);
        float seedZ = pseudoRandom.Next(0, 99999);

        float min = float.MaxValue;
        float max = float.MinValue;

        for (int x = 1; x < _sizeX - 1; x++)
        {
            for (int z = 1; z < _sizeZ - 1; z++)
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

                if (noiseHeight < min) min = noiseHeight;
                if (noiseHeight > max) max = noiseHeight;
                _noiseMap[x, z] = noiseHeight;
            }
        }

        // 0 ~ 1 값으로 변경
        for (int x = 0; x < _sizeX; x++)
            for (int z = 0; z < _sizeZ; z++)
                _noiseMap[x, z] = Mathf.InverseLerp(min, max, _noiseMap[x, z]);

        yield return null;
    }

    private IEnumerator InstantiateObject()
    {
        int cnt = 0;

        for (int x = 1; x < _sizeX - 1; x++)
        {
            for (int z = 1; z < _sizeZ - 1; z++)
            {
                if (_noiseMap[x, z] >= _threshold)
                {
                    Vector3 pos = new Vector3(x + 0.5f, 50f, z + 0.5f) + _position;
                    if (Physics.Raycast(pos, Vector3.down, out var hit, 100f, int.MaxValue, QueryTriggerInteraction.Collide))
                    {
                        if (hit.collider.gameObject.layer != 12)
                            continue;

                        Instantiate(_prefab, hit.point, Quaternion.identity);
                        cnt++;
                    }
                }
            }
        }

        Debug.Log($"[Generator] {_prefab.name}: {cnt}");

        yield return null;
    }
}