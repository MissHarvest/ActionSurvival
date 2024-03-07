using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Linq;
using Unity.VisualScripting;

// 2024-01-16 WJY
public class WorldNavMeshBuilder : MonoBehaviour
{
    private List<NavMeshBuildSource> _sources = new();
    private NavMeshDataInstance _instance;
    private NavMeshData _data;
    private World _world;
    private bool _isInitialized = false;

    public bool AutoUpdate { get; set; }
    public NavMeshDataInstance NavMeshDataInstance => _instance;

    private IEnumerator Start()
    {
        while (AutoUpdate)
        {
            Initialize();
            yield return GenerateNavMeshAsync();
        }
    }

    public void Initialize()
    {
        if (_isInitialized) return;

        _world = GameManager.Instance.World;
        _data = new NavMeshData();
        _instance = NavMesh.AddNavMeshData(_data);

        _isInitialized = true;
    }

    public void AddSources(NavMeshBuildSource source)
    {
        _sources.Add(source);
    }

    public void AddChunkSources(Chunk chunk)
    {
        var tuples = chunk.GetAllBlocksNavMeshSourceData();
        foreach (var data in tuples)
        {
            if (data.Item1?.vertexCount == 0) continue;

            NavMeshBuildSource source = new()
            {
                shape = NavMeshBuildSourceShape.Mesh,
                sourceObject = data.Item1,
                transform = data.Item2,
                area = 0,
            };
            _sources.Add(source);
        }
    }

    public void UpdateChunkSources(HashSet<Chunk> activeChunks)
    {
        if (activeChunks.Count == 0)
            return;

        _sources.Clear();
        foreach (var chunk in activeChunks)
            AddChunkSources(chunk);
    }

    public AsyncOperation GenerateNavMeshAsync(Action<AsyncOperation> callback = null)
    {
        Initialize();

        foreach (var chunk in _world.ChunkMap.Values)
            AddChunkSources(chunk);

        var buildSettings = NavMesh.GetSettingsByID(0);
        var bounds = CaculateWorldBounds();
        var op = NavMeshBuilder.UpdateNavMeshDataAsync(_data, buildSettings, _sources, bounds);
        op.completed += callback;
        return op;
    }

    private Bounds CaculateWorldBounds()
    {
        var minX = _world.ChunkMap.Keys.Min(x => x.x);
        var minZ = _world.ChunkMap.Keys.Min(x => x.z);
        var maxX = _world.ChunkMap.Keys.Max(x => x.x);
        var maxZ = _world.ChunkMap.Keys.Max(x => x.z);

        Bounds bounds = new()
        {
            center = Vector3.zero,
            size = Vector3.one,
            extents = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue),
        };

        return bounds;
    }
}