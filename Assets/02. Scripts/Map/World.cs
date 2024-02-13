using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// 2024-01-12 WJY
public class World : MonoBehaviour
{
    private Transform _player;
    private bool _isDone = false;

    private Dictionary<ChunkCoord, Chunk> _chunkMap = new(comparer: new ChunkCoord());
    private Dictionary<Vector3Int, WorldMapData> _voxelMap = new(comparer: new Vector3IntEqualityComparer());

    private HashSet<Chunk> _currentActiveChunks = new();
    private HashSet<Chunk> _prevActiveChunks = new();

    private ChunkCoord _currentPlayerCoord;
    private ChunkCoord _prevPlayerCoord;

    public WorldData WorldData { get; private set; }
    public VoxelData VoxelData { get; private set; }

    public Dictionary<Vector3Int, WorldMapData> VoxelMap => _voxelMap;
    public HashSet<Chunk> CurrentActiveChunks => _currentActiveChunks;
    public Dictionary<ChunkCoord, Chunk> ChunkMap => _chunkMap;
    public event Action OnWorldUpdated;

    private void Update()
    {
        if (!_isDone) return;

        if (!_player)
            _player = Managers.Game.Player.transform;

        _currentPlayerCoord = ConvertChunkCoord(_player.position);

        if (_prevPlayerCoord != _currentPlayerCoord)
            UpdateChunksInViewRange();

        _prevPlayerCoord = _currentPlayerCoord;
    }

    public ChunkCoord ConvertChunkCoord(Vector3 pos)
    {
        ChunkCoord res = pos;
        res.x /= VoxelData.ChunkSizeX;
        res.z /= VoxelData.ChunkSizeZ;
        return res;
    }

    public ChunkCoord ConvertChunkCoord(Vector3Int pos)
    {
        ChunkCoord res = pos;
        res.x /= VoxelData.ChunkSizeX;
        res.z /= VoxelData.ChunkSizeZ;
        return res;
    }

    /// <summary> 시야범위 내의 청크 생성 </summary>
    public void UpdateChunksInViewRange()
    {
        _prevActiveChunks = _currentActiveChunks;
        _currentActiveChunks = new();

        for (int x = -WorldData.ViewChunkRange; x <= WorldData.ViewChunkRange; x++)
        {
            for (int z = -WorldData.ViewChunkRange; z <= WorldData.ViewChunkRange; z++)
            {
                if (_chunkMap.TryGetValue(_currentPlayerCoord + new ChunkCoord(x, z), out var currentChunk))
                {
                    _currentActiveChunks.Add(currentChunk);
                    currentChunk.SetActive(true);
                }
            }
        }

        _prevActiveChunks.ExceptWith(_currentActiveChunks);
        foreach (var chunk in _prevActiveChunks)
            chunk.IsActive = false;

        //_navMeshBuilder.UpdateNavMesh();
        OnWorldUpdated?.Invoke();
    }

    private void InitializeChunk(ChunkCoord pos, WorldMapData blockData)
    {
        if (!_chunkMap.ContainsKey(pos))
            _chunkMap.Add(pos, new(pos, this));
        _chunkMap[pos].AddVoxel(blockData);
    }

    private IEnumerator GenerateChunk(Action<float, string> progressCallback, float uiUpdateInterval)
    {
        float t = 0f;
        int totalChunkCount = _chunkMap.Count;
        int createdChunkCount = 0;

        foreach (var e in _chunkMap.Values)
        {
            e.GenerateChunk();
            e.IsActive = false;
            createdChunkCount++;

            // 일정 시간마다 UI 업데이트
            if (Time.realtimeSinceStartup - t > uiUpdateInterval)
            {
                t = Time.realtimeSinceStartup;
                progressCallback?.Invoke((float)createdChunkCount / totalChunkCount, "Generate Chunks ...");
                yield return null;
            }
        }

        UpdateChunksInViewRange();

        yield return null;
    }

    public bool CheckVoxel(Vector3 pos)
    {
        Vector3Int intPos = Vector3Int.FloorToInt(pos);

        if (!VoxelMap.ContainsKey(intPos))
            return false;
        else
            return VoxelMap[intPos].type.IsSolid;
    }

    public IEnumerator ReadMapDataFile(TextAsset json, Action<float, string> progressCallback, float uiUpdateInterval)
    {
        float t = 0;
        int currentCount = 0;
        var originData = json.text.DictionaryFromJson<Vector3Int, MapData>();
        _voxelMap = new(originData.Count, new Vector3IntEqualityComparer());

        NativeArray<Vector3Int> coords = new NativeArray<Vector3Int>(originData.Count, Allocator.TempJob);
        NativeArray<ChunkCoord> chunks = new NativeArray<ChunkCoord>(originData.Count, Allocator.TempJob);

        var df = originData.Keys.ToArray();
        for (int i = 0; i < coords.Length; ++i)
        {
            coords[i] = df[i];
        }

        MyParrelJob job = new MyParrelJob();
        job.a = coords;
        job.result = chunks;
        job.voxelSizeX = VoxelData.ChunkSizeX;
        job.voxelSizeZ = VoxelData.ChunkSizeZ;

        var handle = job.Schedule(coords.Length, 10);

        while (!handle.IsCompleted)
            yield return null;

        handle.Complete();

        foreach (var data in originData)
        {
            // originData를 World Map Data로 변경
            var worldMapData = new WorldMapData()
            {
                type = WorldData.GetType(data.Value.type)[data.Value.typeIndex],
                position = data.Key,
                forward = data.Value.forward,
            };
            _voxelMap.TryAdd(data.Key, worldMapData);

            // 현재 블럭이 속한 청크 좌표에 청크 생성
            // 생성한 청크에 블럭 정보 추가 << 

            //var coord = ConvertChunkCoord(data.Key);

            //InitializeChunk(coord, worldMapData);

            //currentCount++;

            //// 일정 시간마다 UI 업데이트
            //if (Time.realtimeSinceStartup - t > uiUpdateInterval)
            //{
            //    t = Time.realtimeSinceStartup;
            //    progressCallback?.Invoke((float)currentCount / originData.Count, "Read MapData ...");
            //    yield return null;
            //}
        }

        for (int i = 0; i < chunks.Length; ++i)
        {
            InitializeChunk(chunks[i], _voxelMap[coords[i]]);
        }
        coords.Dispose();
        chunks.Dispose();

        yield return null;
    }

    public void GenerateWorldAsync(Action<float, string> progressCallback = null, Action completedCallback = null)
    {
        StartCoroutine(GenerateCoroutine(progressCallback, completedCallback));
    }

    private IEnumerator GenerateCoroutine(Action<float, string> progressCallback = null, Action completedCallback = null)
    {
        progressCallback?.Invoke(0f, "Read MapData ...");
        yield return null;
        WorldData = Managers.Resource.GetCache<WorldData>("WorldData.data");
        VoxelData = Managers.Resource.GetCache<VoxelData>("VoxelData.data");
        var data = Managers.Resource.GetCache<TextAsset>("MapData.data");
        yield return StartCoroutine(ReadMapDataFile(data, progressCallback, 0.035f));
        progressCallback?.Invoke(0f, "Generate Chunks ...");
        yield return null;
        yield return StartCoroutine(GenerateChunk(progressCallback, 0.035f));
        progressCallback?.Invoke(1f, "Complete");
        completedCallback?.Invoke();
        _isDone = true;
    }
}

public class Vector3IntEqualityComparer : IEqualityComparer<Vector3Int>
{
    public bool Equals(Vector3Int lhs, Vector3Int rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
    }

    public int GetHashCode(Vector3Int obj)
    {
        int hashCode = obj.y.GetHashCode();
        int hashCode2 = obj.z.GetHashCode();
        return obj.x.GetHashCode() ^ (hashCode << 4) ^ (hashCode >> 28) ^ (hashCode2 >> 4) ^ (hashCode2 << 28);
    }
}

public class WorldMapData
{
    public BlockType type;
    public Vector3Int position;
    public Vector3 forward;
}