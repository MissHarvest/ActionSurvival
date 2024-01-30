using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class VoxelMapDataGenerator : MonoBehaviour
{
    private Dictionary<Vector3Int, MapData> _voxelMap = new();
    private MapEditDataSource[] _dataSources;
    [SerializeField] private TextAsset _mapData;
    [SerializeField] private string _dataPath = @"Assets/04. Resources/MapData.json";
    private int _blockCount = 0;

    public void Start()
    {
        float t = Time.realtimeSinceStartup;
        GetDataSources();
        GenerateMapData();
        SaveDataFile();
        Debug.Log($"Progress Time : {Time.realtimeSinceStartup - t}");
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    private void GetDataSources()
    {
        _dataSources = FindObjectsOfType<MapEditDataSource>();
        Debug.Log($"Get Sources Count : {_dataSources.Length}");
    }

    private void GenerateMapData()
    {
        foreach (var source in _dataSources)
        {
            foreach (var (pos, info) in source.GetSourceData())
            {
                if (_voxelMap.TryAdd(pos, info))
                    _blockCount++;
            }
        }

        Debug.Log($"Create Blocks data Count : {_blockCount}");
    }

    private void SaveDataFile()
    {
        File.WriteAllText(_dataPath, _voxelMap.DictionaryToJson());
        FileInfo fileInfo = new(_dataPath);
        Debug.Log($"Save File Bytes : {fileInfo.Length} byte");
    }
}