using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024-01-12 WJY
public class World : MonoBehaviour
{
    private Transform _player;

    private Dictionary<ChunkCoord, Chunk> _chunkMap = new(comparer: new ChunkCoord());

    private HashSet<Chunk> _currentActiveChunks = new();
    private HashSet<Chunk> _prevActiveChunks = new();

    private ChunkCoord _currentPlayerCoord;
    private ChunkCoord _prevPlayerCoord;

    public WorldData WorldData { get; private set; }
    public VoxelData VoxelData { get; private set; }

    public HashSet<Chunk> CurrentActiveChunks => _currentActiveChunks;
    public Dictionary<ChunkCoord, Chunk> ChunkMap => _chunkMap;
    public event Action OnWorldUpdated;

    private void Update()
    {
        if (!GameManager.Instance.IsRunning) return;

        if (!_player)
            _player = GameManager.Instance.Player.transform;

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

    public WorldMapData GetBlock(Vector3Int pos)
    {
        ChunkCoord cc = ConvertChunkCoord(pos);

        if (_chunkMap.TryGetValue(cc, out var chunk))
        {
            if (chunk.LocalMap.TryGetValue(pos, out var block))
                return block;
        }
        return null;
    }

    public bool CheckVoxel(Vector3 pos)
    {
        Vector3Int intPos = Vector3Int.FloorToInt(pos);

        var block = GetBlock(intPos);

        if (block == null)
            return false;
        else
            return block.type.IsSolid;
    }

    public IEnumerator ReadMapDataFile(TextAsset json, Action<float, string> progressCallback, float uiUpdateInterval)
    {
        float t = 0;
        int currentCount = 0;
        var originData = json.text.DictionaryFromJson<Vector3Int, MapData>();

        foreach (var data in originData)
        {
            // originData를 World Map Data로 변경
            var worldMapData = new WorldMapData()
            {
                type = WorldData.GetType(data.Value.type)[data.Value.typeIndex],
                position = data.Key,
                forward = data.Value.forward,
            };

            // 현재 블럭이 속한 청크 좌표에 청크 생성
            // 생성한 청크에 블럭 정보 추가
            var coord = ConvertChunkCoord(data.Key);
            InitializeChunk(coord, worldMapData);

            currentCount++;

            // 일정 시간마다 UI 업데이트
            if (Time.realtimeSinceStartup - t > uiUpdateInterval)
            {
                t = Time.realtimeSinceStartup;
                progressCallback?.Invoke((float)currentCount / originData.Count, "Read MapData ...");
                yield return null;
            }
        }
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
    }
}

public class WorldMapData
{
    public BlockType type;
    public Vector3Int position;
    public Vector3 forward;
}