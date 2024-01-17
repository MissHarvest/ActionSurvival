using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

// 2024-01-12 WJY
public class World : MonoBehaviour
{
    private Transform _player;
    private bool _isDone = false;
    private WorldNavMeshBuilder _navMeshBuilder;

    private Dictionary<ChunkCoord, Chunk> _chunkMap = new(comparer: new ChunkCoord());
    private Dictionary<Vector3Int, BlockType> _voxelMap = new();

    private List<Chunk> _currentActiveChunks = new();
    private List<Chunk> _prevActiveChunks = new();

    private ChunkCoord _currentPlayerCoord;
    private ChunkCoord _prevPlayerCoord;

    public WorldData WorldData { get; private set; }
    public VoxelData VoxelData { get; private set; }

    public Dictionary<Vector3Int, BlockType> VoxelMap => _voxelMap;
    public WorldNavMeshBuilder NavMeshBuilder => _navMeshBuilder;

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

                    // NavMesh Source 전달
                    _navMeshBuilder.UpdateChunkSources(_currentActiveChunks);
                }
            }
        }

        foreach (var chunk in _prevActiveChunks)
            chunk.IsActive = false;

        _navMeshBuilder.UpdateNavMesh();
    }

    // TEST: 일단 Grass 블록으로 평지 만들기
    private IEnumerator PopulateVoxelMap()
    {
        int SizeX = WorldData.WorldSize / 2;
        int SizeZ = WorldData.WorldSize / 2;

        for (int y = 0; y < 1; y++)
        {
            for (int x = -SizeX; x < SizeX; x++)
            {
                for (int z = -SizeZ; z < SizeZ; z++)
                {
                    _voxelMap.TryAdd(new(x, y, z), WorldData.BlockTypes[0]);
                }
            }
        }

        // TEST: 볼록한 지형 만들어보기
        for (int x = 10; x < 20; x++)
        {
            for (int z = 10; z < 20; z++)
            {
                int maxHeight = UnityEngine.Random.Range(2, 5);
                for (int y = 1; y < maxHeight; y++)
                {
                    if (y == maxHeight - 1)
                        _voxelMap.TryAdd(new(x, y, z), WorldData.BlockTypes[0]);
                    else
                        _voxelMap.TryAdd(new(x, y, z), WorldData.BlockTypes[1]);
                }
            }
        }

        yield return null;
    }

    private Chunk CreateChunk(ChunkCoord pos)
    {
        Chunk chunk = new(pos, this);
        _chunkMap.Add(pos, chunk);
        return chunk;
    }

    private IEnumerator GenerateChunk()
    {
        int SizeX = WorldData.WorldSize / VoxelData.ChunkSizeX / 2;
        int SizeZ = WorldData.WorldSize / VoxelData.ChunkSizeZ / 2;

        for (int x = -SizeX; x <= SizeX; x++)
        {
            for (int z = -SizeZ; z <= SizeZ; z++)
            {
                var chunk = CreateChunk(new(x, z));
                chunk.IsActive = false;
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
            return VoxelMap[intPos].isSolid;
    }

    public void GenerateWorldAsync(Action<float, string> progressCallback = null, Action completedCallback = null)
    {
        WorldData = Managers.Resource.GetCache<WorldData>("WorldData.data");
        VoxelData = Managers.Resource.GetCache<VoxelData>("VoxelData.data");
        StartCoroutine(GenerateCoroutine(progressCallback, completedCallback));
    }

    private IEnumerator GenerateCoroutine(Action<float, string> progressCallback = null, Action completedCallback = null)
    {
        progressCallback?.Invoke(0.25f, "복셀 맵 생성 중...");
        yield return StartCoroutine(PopulateVoxelMap());
        progressCallback?.Invoke(0.5f, "청크 생성 중...");
        yield return StartCoroutine(GenerateChunk());
        progressCallback?.Invoke(1f, "완료");
        completedCallback?.Invoke();
        _isDone = true;
    }

    public void InitializeWorldNavMeshBuilder()
    {
        StartCoroutine(InitializeWorldNavMeshBuilderCoroutine());
    }

    private IEnumerator InitializeWorldNavMeshBuilderCoroutine()
    {
        _navMeshBuilder = new GameObject(nameof(WorldNavMeshBuilder)).AddComponent<WorldNavMeshBuilder>();
        _navMeshBuilder.IsActive = false; // 주기적으로 업데이트 X. 일단은 청크 정보가 바뀔 때마다 업데이트합니다.
        UpdateChunksInViewRange();
        yield return null;
        yield return _navMeshBuilder.UpdateNavMesh();
    }
}