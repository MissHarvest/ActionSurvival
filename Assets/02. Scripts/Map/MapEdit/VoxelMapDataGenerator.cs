using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;

public class VoxelMapDataGenerator : MonoBehaviour
{
    [SerializeField] private TextAsset _mapData;
    [SerializeField] private string _dataPath = @"Assets/04. Resources/MapData.json";
    [SerializeField] private WorldData _worldData;
    [SerializeField] private VoxelData _voxelData;
    [SerializeField] private IslandGernerator[] _islandGenerators;

    private MapEditDataSource[] _dataSources;
    private List<Dictionary<Vector3Int, MapData>> _islandDataList;
    private Dictionary<Vector3Int, MapData> _voxelMap = new();
    private int _blockCount = 0;


    public IEnumerator Start()
    {
        float t = Time.realtimeSinceStartup;
        yield return StartCoroutine(GetDataSources());
        yield return StartCoroutine(GetIslandData());
        yield return StartCoroutine(GenerateMapData());
        //yield return StartCoroutine(RemoveUnnecessaryBlock());
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

    //// 임시... 쓸모없는 블럭들은 지우도록 했습니다.
    //private IEnumerator RemoveUnnecessaryBlock()
    //{
    //    Dictionary<Vector3Int, WorldMapData> worldMap = new();
    //    foreach (var data in _voxelMap)
    //    {
    //        var worldMapData = new WorldMapData()
    //        {
    //            type = _worldData.GetType(data.Value.type)[data.Value.typeIndex],
    //            position = data.Key,
    //            forward = data.Value.forward,
    //        };
    //        worldMap.TryAdd(data.Key, worldMapData);
    //    }

    //    Vector3Int[] check = new Vector3Int[4]
    //    {
    //        Vector3Int.forward,
    //        Vector3Int.back,
    //        Vector3Int.left,
    //        Vector3Int.right,
    //    };

    //    // TSET ==================
    //    foreach (var block in worldMap.Values)
    //    {
    //        Vector3Int? pos = null;
    //        Vector3Int? forward = null;
    //        int openCount = 0;
    //        for (int i = 0; i < 4; i++)
    //        {
    //            Vector3Int checkPos = block.position + check[i];
    //            // 바라보는 칸이 아예 비었음.
    //            // 그 아래칸은 Solid임
    //            // 그 아래칸의 바라보는 칸도 Solid임

    //            if (worldMap.ContainsKey(checkPos) || 
    //                !worldMap.TryGetValue(checkPos + Vector3Int.down, out var checkBlock) ||
    //                !worldMap.TryGetValue(checkPos + Vector3Int.down + check[i], out var checkBlock2))
    //                continue;

    //            if (checkBlock.type.IsSolid && checkBlock2.type.IsSolid)
    //            {
    //                pos = checkPos;
    //                forward = -check[i];
    //                openCount++;
    //            }
    //        }

    //        if (pos.HasValue && forward.HasValue && openCount == 1)
    //        {
    //            MapData newSlideBlcokData = new(MapEditDataSource.Inherits.Slide, 0, forward.Value);
    //            _voxelMap.TryAdd(pos.Value, newSlideBlcokData);
    //        }
    //    }
    //    //========================

    //    HashSet<Vector3Int> deleteKey = new();
    //    foreach (var data in worldMap.Values)
    //    {
    //        bool flag = true;
    //        for (int i = 0; i < 6; i++)
    //        {
    //            Vector3Int pos = Vector3Int.FloorToInt(data.position + _voxelData.faceChecks[i]);

    //            if (!worldMap.ContainsKey(pos))
    //            { 
    //                flag = false;
    //                break; 
    //            }
    //            else if (!worldMap[pos].type.IsSolid)
    //            { 
    //                flag = false;
    //                break;
    //            }
    //        }

    //        if (flag)
    //            deleteKey.Add(data.position);
    //    }

    //    Debug.Log($"delete blocks : {deleteKey.Count}");

    //    foreach (var key in deleteKey)
    //        _voxelMap.Remove(key);

    //    yield return null;
    //}

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