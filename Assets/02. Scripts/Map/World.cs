using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

// 2024-01-12 WJY
public class World : MonoBehaviour
{
    private Transform _player;
    private bool _isDone = false;

    private Dictionary<ChunkCoord, Chunk> _chunkMap = new();
    private Dictionary<Vector3Int, BlockType> _voxelMap = new();

    private List<Chunk> _currentActiveChunks = new();
    private List<Chunk> _prevActiveChunks = new();

    private ChunkCoord _currentPlayerCoord;
    private ChunkCoord _prevPlayerCoord;

    private List<Vector3> _vertices = new();
    private List<int> _triangles = new();
    private int _vertexIdx = 0;
    private MeshCollider _meshCollider;

    public WorldData WorldData { get; private set; }
    public Dictionary<Vector3Int, BlockType> VoxelMap => _voxelMap;

    //private void Start()
    //{
    //    GenerateWorldAsync();
    //    PopulateVoxelMap();
    //    GenerateChunk();
    //    CreateMeshData();
    //    CreateMesh();
    //}

    private void Update()
    {
        if (!_isDone) return;

        if (!_player)
            _player = Managers.Game.Player.transform;

        _currentPlayerCoord = _player.position;

        if (_prevPlayerCoord != _currentPlayerCoord)
            UpdateChunksInViewRange();

        _prevPlayerCoord = _currentPlayerCoord;
    }

    /// <summary> 시야범위 내의 청크 생성 </summary>
    private void UpdateChunksInViewRange()
    {
        _prevActiveChunks = _currentActiveChunks;
        _currentActiveChunks = new();

        for (int x = -WorldData.ViewRange; x < WorldData.ViewRange; x++)
        {
            for (int z = -WorldData.ViewRange; z < WorldData.ViewRange; z++)
            {
                if (_chunkMap.TryGetValue(_currentPlayerCoord + new ChunkCoord(x, z), out var currentChunk))
                {
                    _currentActiveChunks.Add(currentChunk);
                    currentChunk?.SetActive(true);
                    _prevActiveChunks.Remove(currentChunk);
                }
            }
        }

        foreach (var chunk in _prevActiveChunks)
            chunk.IsActive = false;
    }

    // TEST: 일단 Grass 블록으로 평지 만들기
    private IEnumerator PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.ChunkSizeY; y++)
        {
            for (int x = -WorldData.WorldSize / 2; x < WorldData.WorldSize / 2; x++)
            {
                for (int z = -WorldData.WorldSize / 2; z < WorldData.WorldSize / 2; z++)
                {
                    _voxelMap.TryAdd(new(x, y, z), WorldData.BlockTypes[0]);
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
        for (int x = -WorldData.WorldSize / 2; x < WorldData.WorldSize / 2; x++)
        {
            for (int z = -WorldData.WorldSize / 2; z < WorldData.WorldSize / 2; z++)
            {
                if (x % VoxelData.ChunkSizeX == 0 && z % VoxelData.ChunkSizeZ == 0)
                {
                    var chunk = CreateChunk(new(x, z));
                    chunk.IsActive = false;
                    yield return null;
                }
            }
        }
    }

    private IEnumerator CreateMeshData()
    {
        foreach (var pos in _voxelMap.Keys)
            AddVoxelData(pos);
        yield return null;
    }

    private void AddVoxelData(Vector3Int pos)
    {
        for (int i = 0; i < 6; i++)
        {
            if (CheckVoxel(pos + VoxelData.faceChecks[i]))
                continue;

            for (int j = 0; j < 4; j++)
                _vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[i][j]]);

            _triangles.Add(_vertexIdx);
            _triangles.Add(_vertexIdx + 1);
            _triangles.Add(_vertexIdx + 2);
            _triangles.Add(_vertexIdx + 2);
            _triangles.Add(_vertexIdx + 1);
            _triangles.Add(_vertexIdx + 3);
            _vertexIdx += 4;
        }
    }

    public bool CheckVoxel(Vector3 pos)
    {
        Vector3Int intPos = new(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));

        if (!VoxelMap.ContainsKey(intPos))
            return false;
        else
            return VoxelMap[intPos].isSolid;
    }

    private void CreateMesh()
    {
        Mesh mesh = new()
        {
            vertices = _vertices.ToArray(),
            triangles = _triangles.ToArray(),
        };
        mesh.RecalculateNormals();
        Debug.Log(_vertices.Count);
        Debug.Log(_triangles.Count);
        _meshCollider = gameObject.AddComponent<MeshCollider>();
        _meshCollider.sharedMesh = mesh;
    }

    public void GenerateWorldAsync(Action<float, string> progressCallback = null, Action completedCallback = null)
    {
        WorldData = Managers.Resource.GetCache<WorldData>("WorldData.data");
        StartCoroutine(GenerateCoroutine(progressCallback, completedCallback));
    }

    private IEnumerator GenerateCoroutine(Action<float, string> progressCallback = null, Action completedCallback = null)
    {
        progressCallback?.Invoke(0.25f, "복셀 맵 생성 중...");
        yield return StartCoroutine(PopulateVoxelMap());
        progressCallback?.Invoke(0.5f, "청크 생성 중...");
        yield return StartCoroutine(GenerateChunk());
        //progressCallback?.Invoke(0.75f, "메시 콜라이더 생성 중...");
        //yield return StartCoroutine(CreateMeshData());
        //CreateMesh();
        progressCallback?.Invoke(1f, "완료");
        completedCallback?.Invoke();
        _isDone = true;
    }
}