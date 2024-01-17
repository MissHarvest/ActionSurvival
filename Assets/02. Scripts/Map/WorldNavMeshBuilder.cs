using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System;

// 2024-01-16 WJY
public class WorldNavMeshBuilder : MonoBehaviour
{
    private List<NavMeshBuildSource> _sources;
    private NavMeshDataInstance _instance;
    private NavMeshData _data;
    private Transform _player;
    private World _world;

    public bool IsActive { get; set; }
    public NavMeshDataInstance NavMeshDataInstance => _instance;

    private IEnumerator Start()
    {
        _player = Managers.Game.Player.transform;
        _world = Managers.Game.World;
        _data = new NavMeshData();
        _instance = NavMesh.AddNavMeshData(_data);

        while (IsActive)
        {
            yield return UpdateNavMesh();
        }
    }

    public void UpdateChunkSources(List<Chunk> activeChunks)
    {
        if (activeChunks.Count == 0)
            return;

        if (_sources == null)
        {
            _sources = new();
            return;
        }

        _sources.Clear();
        foreach (var chunk in activeChunks)
        {
            NavMeshBuildSource source = new()
            {
                shape = NavMeshBuildSourceShape.Mesh,
                sourceObject = chunk.Mesh,
                transform = chunk.TransformMatrix,
                area = 0,
            };
            _sources.Add(source);
        }
    }

    public AsyncOperation UpdateNavMesh(Action<AsyncOperation> callback = null)
    {
        if (_data == null || _sources == null)
            return null;

        var buildSettings = NavMesh.GetSettingsByID(0);
        var bounds = CalculateViewBounds();
        var op = NavMeshBuilder.UpdateNavMeshDataAsync(_data, buildSettings, _sources, bounds);
        op.completed += callback;
        return op;
    }

    private Bounds CalculateViewBounds()
    {
        float sizeX = _world.VoxelData.ChunkSizeX * _world.WorldData.ViewChunkRange * 2f;
        float sizeY = _world.VoxelData.ChunkSizeY + 30f;
        float sizeZ = _world.VoxelData.ChunkSizeZ * _world.WorldData.ViewChunkRange * 2f;

        Bounds bounds = new()
        {
            center = _player.position,
            size = Vector3.one,
            extents = new Vector3(sizeX, sizeY, sizeZ),
        };

        return bounds;
    }

    void OnDrawGizmosSelected()
    {
        if (_data)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_data.sourceBounds.center, _data.sourceBounds.size);
        }

        Gizmos.color = Color.yellow;
        var bounds = CalculateViewBounds();
        Gizmos.DrawWireCube(bounds.center, bounds.extents);

        Gizmos.color = Color.green;
        var center = _player ? _player.position : transform.position;
        Gizmos.DrawWireCube(center, bounds.size);
    }
}