using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;

public class VoxelMapDataGenerator : MonoBehaviour
{
    [SerializeField] private TextAsset _mapData;
    [SerializeField] private string _dataPath = @"Assets/04. Resources/MapData.json";
    [SerializeField] private IslandGernerator[] _islandGenerators;

    private MapEditDataSource[] _dataSources;
    private List<List<(Vector3Int pos, MapData data)>> _islandDataList;
    private Dictionary<Vector3Int, MapData> _voxelMap = new();
    private int _blockCount = 0;


    public IEnumerator Start()
    {
        float t = Time.realtimeSinceStartup;
        yield return StartCoroutine(GetDataSources());
        yield return StartCoroutine(GetIslandData());
        yield return StartCoroutine(GenerateMapData());
        yield return StartCoroutine(SaveDataFile());
        Debug.Log($"Progress Time : {Time.realtimeSinceStartup - t}");
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
        yield return null;
    }

    private IEnumerator GetDataSources()
    {
        _dataSources = FindObjectsOfType<MapEditDataSource>();
        Debug.Log($"Get Sources Count : {_dataSources.Length}");
        yield return null;
    }

    private IEnumerator GenerateMapData()
    {
        foreach (var dataList in _islandDataList)
        {
            foreach ((var pos, var data) in dataList)
            {
                if (_voxelMap.TryAdd(pos, data))
                    _blockCount++;
            }
        }

        if (_dataSources != null)
        {
            foreach (var source in _dataSources)
            {
                foreach (var (pos, info) in source.GetSourceData())
                {
                    if (_voxelMap.TryAdd(pos, info))
                        _blockCount++;
                }
            }
        }

        Debug.Log($"Create Blocks data Count : {_blockCount}");
        yield return null;
    }

    private IEnumerator SaveDataFile()
    {
        File.WriteAllText(_dataPath, _voxelMap.DictionaryToJson());
        FileInfo fileInfo = new(_dataPath);
        Debug.Log($"Save File Bytes : {fileInfo.Length} byte");
        yield return null;
    }

    private IEnumerator GetIslandData()
    {
        _islandDataList = new();
        foreach (var island in _islandGenerators)
        {
            yield return StartCoroutine(island.Generate());
            _islandDataList.Add(island.Data);
        }
        Debug.Log($"Get Island Data : {_islandDataList.Count}");
        yield return null;
    }
}