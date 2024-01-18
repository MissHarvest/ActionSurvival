using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private ChunkCoord _minChunkCoord;
    private ChunkCoord _maxChunkCoord;

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

    private IEnumerator PopulateVoxelMap()
    {
        // TEST: 일단 Grass 블록으로 평지 만들기
        int SizeX = WorldData.WorldSize / 2;
        int SizeZ = WorldData.WorldSize / 2;

        for (int y = 0; y < 1; y++)
        {
            for (int x = -SizeX; x < SizeX; x++)
            {
                for (int z = -SizeZ; z < SizeZ; z++)
                {
                    _voxelMap.TryAdd(new(x, y, z), (WorldData.NormalBlockTypes[0], Vector3.forward));
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
                        _voxelMap.TryAdd(new(x, y, z), (WorldData.NormalBlockTypes[0], Vector3.forward));
                    else
                        _voxelMap.TryAdd(new(x, y, z), (WorldData.NormalBlockTypes[1], Vector3.forward));
                }
            }
        }

        // TEST: 계단 만들어보기
        _voxelMap.TryAdd(new(-10, 1, -10), (WorldData.SlideBlockTypes[0], Vector3.forward));
        for (int i = -9; i < 0; i++)
        {
            _voxelMap.TryAdd(new(-10, 1, i), (WorldData.NormalBlockTypes[0], Vector3.forward));
            _voxelMap.TryAdd(new(-9, 1, i), (WorldData.NormalBlockTypes[0], Vector3.forward));
            _voxelMap.TryAdd(new(-8, 1, i), (WorldData.NormalBlockTypes[0], Vector3.forward));
        }

        _voxelMap.TryAdd(new(-9, 1, -11), (WorldData.SlideBlockTypes[0], Vector3.right));
        _voxelMap.TryAdd(new(-9, 1, -12), (WorldData.SlideBlockTypes[0], Vector3.right));
        _voxelMap.TryAdd(new(-8, 1, -11), (WorldData.NormalBlockTypes[0], Vector3.forward));
        _voxelMap.TryAdd(new(-8, 1, -12), (WorldData.NormalBlockTypes[0], Vector3.forward));
        _voxelMap.TryAdd(new(-9, 1, -10), (WorldData.NormalBlockTypes[0], Vector3.forward));
        _voxelMap.TryAdd(new(-8, 1, -10), (WorldData.NormalBlockTypes[0], Vector3.forward));

        // TEST: 우측에 눈 덮인 지형 만들어보기
        for (int y = 0; y < 1; y++)
        {
            for (int x = -SizeX + WorldData.WorldSize; x < SizeX + WorldData.WorldSize; x++)
            {
                for (int z = -SizeZ; z < SizeZ; z++)
                {
                    _voxelMap.TryAdd(new(x, y, z), (WorldData.NormalBlockTypes[2], Vector3.forward));
                }
            }
        }

        // TEST: 좌측에 붉은 돌 지형 만들어보기
        for (int y = 0; y < 1; y++)
        {
            for (int x = -SizeX + -WorldData.WorldSize; x < SizeX + -WorldData.WorldSize; x++)
            {
                for (int z = -SizeZ; z < SizeZ; z++)
                {
                    _voxelMap.TryAdd(new(x, y, z), (WorldData.NormalBlockTypes[3], Vector3.forward));
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

    // BFS로 개선해도 ???
    private IEnumerator GenerateChunk()
    {
        //var keys = _voxelMap.Keys;
        //_minChunkCoord.x = keys.Min(pos => pos.x) / VoxelData.ChunkSizeX - 1;
        //_minChunkCoord.z = keys.Min(pos => pos.z) / VoxelData.ChunkSizeZ - 1;
        //_maxChunkCoord.x = keys.Max(pos => pos.x) / VoxelData.ChunkSizeX + 1;
        //_maxChunkCoord.z = keys.Max(pos => pos.z) / VoxelData.ChunkSizeZ + 1;

        //for (int x = _minChunkCoord.x; x <= _maxChunkCoord.x; x++)
        //{
        //    for (int z = _minChunkCoord.z; z <= _maxChunkCoord.z; z++)
        //    {
        //        var chunk = CreateChunk(new(x, z));
        //        chunk.IsActive = false;
        //    }
        //}

        int SizeX = WorldData.WorldSize / VoxelData.ChunkSizeX / 2;
        int SizeZ = WorldData.WorldSize / VoxelData.ChunkSizeZ / 2;

        for (int x = -SizeX * 3; x <= SizeX * 3; x++)
        {
            for (int z = -SizeZ; z <= SizeZ; z++)
            {
                var chunk = CreateChunk(new(x, z));
                chunk.IsActive = false;
            }
        }

        //int SizeX = WorldData.WorldSize / VoxelData.ChunkSizeX / 2;
        //int SizeZ = WorldData.WorldSize / VoxelData.ChunkSizeZ / 2;

        //for (int x = -SizeX; x <= SizeX; x++)
        //{
        //    for (int z = -SizeZ; z <= SizeZ; z++)
        //    {
        //        var chunk = CreateChunk(new(x, z));
        //        chunk.IsActive = false;
        //    }
        //}
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
        yield return new WaitForFixedUpdate();
        yield return _navMeshBuilder.UpdateNavMesh();
    }
}