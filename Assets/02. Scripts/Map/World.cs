using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// 2024-01-12 WJY
public class World : MonoBehaviour
{
    private Transform _player;
    private bool _isDone = false;
    private WorldNavMeshBuilder _navMeshBuilder;

    private Dictionary<ChunkCoord, Chunk> _chunkMap = new(comparer: new ChunkCoord());
    private Dictionary<Vector3Int, (BlockType type, Vector3 forward)> _voxelMap = new();

    private List<Chunk> _currentActiveChunks = new();
    private List<Chunk> _prevActiveChunks = new();

    private ChunkCoord _currentPlayerCoord;
    private ChunkCoord _prevPlayerCoord;

    private int _totalChunkCount = 1;
    private int _createdChunkCount = 0;

    public WorldData WorldData { get; private set; }
    public VoxelData VoxelData { get; private set; }

    public Dictionary<Vector3Int, (BlockType type, Vector3 forward)> VoxelMap => _voxelMap;
    public WorldNavMeshBuilder NavMeshBuilder => _navMeshBuilder;
    public List<Chunk> CurrentActiveChunks => _currentActiveChunks;

    private void Update()
    {
        if (!_isDone) return;

        if (!_player)
            _player = Managers.Game.Player.transform;

        _currentPlayerCoord = _player.position;
        _currentPlayerCoord.x /= VoxelData.ChunkSizeX;
        _currentPlayerCoord.z /= VoxelData.ChunkSizeZ;

        if (_prevPlayerCoord != _currentPlayerCoord)
            UpdateChunksInViewRange();

        _prevPlayerCoord = _currentPlayerCoord;
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
                    _prevActiveChunks.Remove(currentChunk);
                }
            }
        }

        foreach (var chunk in _prevActiveChunks)
            chunk.IsActive = false;

        _navMeshBuilder.UpdateNavMesh();
    }

    private Chunk CreateChunk(ChunkCoord pos)
    {
        Chunk chunk = new(pos, this);
        _chunkMap.Add(pos, chunk);
        return chunk;
    }

    private IEnumerator GenerateChunk(Action<float, string> progressCallback, float uiUpdateInterval)
    {
        float t = 0f;

        // BFS로 생성
        Queue<ChunkCoord> queue = new();
        queue.Enqueue(new ChunkCoord(0, 0));

        Dictionary<ChunkCoord, bool> visited = new();
        int minX = _voxelMap.Keys.Min(pos => pos.x) / VoxelData.ChunkSizeX - 1;
        int maxX = _voxelMap.Keys.Max(pos => pos.x) / VoxelData.ChunkSizeX + 1;
        int minZ = _voxelMap.Keys.Min(pos => pos.z) / VoxelData.ChunkSizeZ - 1;
        int maxZ = _voxelMap.Keys.Max(pos => pos.z) / VoxelData.ChunkSizeZ + 1;
        for (int x = minX; x <= maxX; x++)
            for (int z = minZ; z <= maxZ; z++)
                visited.TryAdd(new(x, z), false);
        visited[new(0, 0)] = true;
        _totalChunkCount = visited.Count;

        ChunkCoord[] dxdy = { ChunkCoord.Up, ChunkCoord.Down, ChunkCoord.Left, ChunkCoord.Right };

        while (queue.Count > 0)
        {
            _createdChunkCount++;
            var current = queue.Dequeue();
            var chunk = CreateChunk(current);
            chunk.IsActive = false;

            for (int i = 0; i < dxdy.Length; i++)
            {
                ChunkCoord next = current + dxdy[i];
                if (visited.ContainsKey(next))
                {
                    if (!visited[next])
                    {
                        visited[next] = true;
                        queue.Enqueue(next);
                    }
                }
            }

            // 일정 시간마다 UI 업데이트
            t += Time.unscaledDeltaTime;
            if (uiUpdateInterval < t)
            {
                t = 0f;
                progressCallback?.Invoke((float)_createdChunkCount / _totalChunkCount, "Generate Chunks ...");
                yield return null;
            }
        }

        yield return null;
    }

    public bool CheckVoxel(Vector3 pos)
    {
        Vector3Int intPos = new(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));

        if (!VoxelMap.ContainsKey(intPos))
            return false;
        else
            return VoxelMap[intPos].type.IsSolid;
    }

    public IEnumerator ReadMapDataFile(TextAsset json)
    {
        var originData = json.text.DictionaryFromJson<Vector3Int, MapData>();
        foreach (var data in originData)
        {
            var type = WorldData.GetType(data.Value.type)[data.Value.typeIndex];
            _voxelMap.TryAdd(data.Key, (type, data.Value.forward));
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
        yield return StartCoroutine(ReadMapDataFile(data));
        progressCallback?.Invoke((float)_createdChunkCount / _totalChunkCount, "Generate Chunks ...");
        yield return null;
        yield return StartCoroutine(GenerateChunk(progressCallback, 0.5f));
        progressCallback?.Invoke(1f, "Complete");
        completedCallback?.Invoke();
        _isDone = true;
    }

    public void InitializeWorldNavMeshBuilder(bool autoUpdate = false, Action<AsyncOperation> callback = null)
    {
        StartCoroutine(InitializeWorldNavMeshBuilderCoroutine(autoUpdate, callback));
    }

    private IEnumerator InitializeWorldNavMeshBuilderCoroutine(bool autoUpdate = false, Action<AsyncOperation> callback = null)
    {
        _navMeshBuilder = new GameObject(nameof(WorldNavMeshBuilder)).AddComponent<WorldNavMeshBuilder>();
        _navMeshBuilder.IsActive = autoUpdate; // 주기적으로 업데이트 X. 일단은 청크 정보가 바뀔 때마다 업데이트합니다.
        UpdateChunksInViewRange();
        yield return new WaitForFixedUpdate();
        yield return _navMeshBuilder.UpdateNavMesh(callback);
    }
}