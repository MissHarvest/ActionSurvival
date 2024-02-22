using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// 2024-01-12 WJY
public class Chunk
{
    private GameObject _chunkObject;
    private Transform _instanceBlockParent;
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private ChunkCoord _coord;
    private Dictionary<Vector3Int, WorldMapData> _localMap = new();
    private bool _isActive;

    private int _vertexIdx = 0;
    private List<Vector3> _vertices = new();
    private List<int> _triangles = new();
    private List<Vector2> _uvs = new();

    private List<InstanceBlock> _instanceBlocks = new();

    private World _world;
    private VoxelData _data;

    private List<bool> _jobChecks = new();
    private List<int> _jobTextures = new();
    private List<Vector3> _jobPositions = new();

    public ChunkCoord ChunkCoord => _coord;
    public Dictionary<Vector3Int, WorldMapData> LocalMap => _localMap;
    public bool IsActive
    {
        get => _isActive;
        set => SetActive(value);
    }
    public Mesh Mesh => _meshFilter.sharedMesh;
    public Matrix4x4 TransformMatrix => _chunkObject.transform.localToWorldMatrix;
    public int VertexIdx { get => _vertexIdx; set => _vertexIdx = value; }
    public List<Vector3> Vertices => _vertices;
    public List<int> Triangles => _triangles;
    public List<Vector2> Uvs => _uvs;
    public World World => _world;
    public VoxelData Data => _data;
    public Transform InstanceBlocksParent => _instanceBlockParent;
    public List<InstanceBlock> InstanceBlocks => _instanceBlocks;
    public List<bool> JobChecks => _jobChecks;
    public List<int> JobTextures => _jobTextures;
    public List<Vector3> JobPositions => _jobPositions;

    public Chunk(ChunkCoord coord, World world)
    {
        _world = world;
        _coord = coord;
        _data = _world.VoxelData;

        _chunkObject = new($"{nameof(Chunk)} {coord.x:D2}, {coord.z:D2}");
        _chunkObject.layer = LayerMask.NameToLayer("Ground");
        _meshRenderer = _chunkObject.AddComponent<MeshRenderer>();
        _meshFilter = _chunkObject.AddComponent<MeshFilter>();

        _meshRenderer.material = world.WorldData.Material;
        _chunkObject.transform.SetParent(world.transform);
        _instanceBlockParent = new GameObject("Instance Block Parent").transform;
        _instanceBlockParent.SetParent(_chunkObject.transform);
    }

    public void AddVoxel(WorldMapData data)
    {
        _localMap.Add(data.position, data);
    }

    public void GenerateChunk()
    {
        CoroutineManagement.Instance.StartCoroutine(GenerateChunkJobCoroutine());
    }

    private IEnumerator GenerateChunkJobCoroutine()
    {
        foreach (var block in _localMap.Values)
            block.type.AddVoxelDataToChunk(this, block.position, block.forward);

        var job = new ChunkJob()
        {
            checks = new(_jobChecks.ToArray(), Allocator.TempJob),
            textures = new(_jobTextures.ToArray(), Allocator.TempJob),
            positions = new(_jobPositions.ToArray(), Allocator.TempJob),
            faceIdx = 0,
            vertextIdx = 0,
            textureAtlasWidth = _data.TextureAtlasWidth,
            textureAtlasHeight = _data.TextureAtlasHeight,
            normalizeTextureAtlasWidth = _data.NormalizeTextureAtlasWidth,
            normalizeTextureAtlasHeight = _data.NormalizeTextureAtlasHeight,
            uvXBeginOffset = _data.uvXBeginOffset,
            uvXEndOffset = _data.uvXEndOffset,
            uvYBeginOffset = _data.uvYBeginOffset,
            uvYEndOffset = _data.uvYEndOffset,
            vertices = new(0, Allocator.TempJob),
            triangles = new(0, Allocator.TempJob),
            uv = new(0, Allocator.TempJob),
        };

        _jobChecks = null;
        _jobTextures = null;
        _jobPositions = null;

        var handle = job.Schedule();

        if (!handle.IsCompleted)
            yield return null;

        handle.Complete();

        var (vertices, triangles, uv) = job.GetResult();
        _vertices = new(vertices.AsArray());
        _triangles = new(triangles.AsArray());
        _uvs = new(uv.AsArray());
        job.Dispose();

        CreateMesh();
    }

    private void CreateMesh()
    {
        Mesh mesh = new()
        {
            vertices = _vertices.ToArray(),
            triangles = _triangles.ToArray(),
            uv = _uvs.ToArray(),
        };
        mesh.RecalculateNormals();
        _meshFilter.mesh = mesh;
        _chunkObject.transform.SetParent(_world.transform);
        _chunkObject.AddComponent<MeshCollider>().sharedMesh = mesh;
    }

    public void SetActive(bool active)
    {
        _isActive = active;
        _meshRenderer.enabled = active;
        foreach (var e in _instanceBlocks)
            e.MeshRenderer.enabled = active;
    }

    public (Mesh, Matrix4x4)[] GetAllBlocksNavMeshSourceData()
    {
        (Mesh, Matrix4x4)[] result = new (Mesh, Matrix4x4)[_instanceBlockParent.childCount + 1];
        result[0] = (Mesh, TransformMatrix);
        for (int i = 1; i < result.Length; i++)
        {
            var block = _instanceBlockParent.GetChild(i - 1).GetComponent<InstanceBlock>();
            result[i] = (block.Mesh, block.TransformMatrix);
        }
        return result;
    }
}

public struct ChunkCoord : IEqualityComparer<ChunkCoord>
{
    public int x;
    public int z;

    public ChunkCoord(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    #region Utility
    public static ChunkCoord Up => new(0, 1);
    public static ChunkCoord Down => new(0, -1);
    public static ChunkCoord Left => new(-1, 0);
    public static ChunkCoord Right => new(1, 0);

    public override string ToString()
    {
        return $"({x}, {z})";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, z);
    }

    public int GetHashCode(ChunkCoord obj)
    {
        return obj.GetHashCode();
    }

    public bool Equals(ChunkCoord l, ChunkCoord r)
    {
        return l.x == r.x && l.z == r.z;
    }

    public override bool Equals(object other)
    {
        var o = (ChunkCoord)other;
        return Equals(this, o);
    }

    public bool Equals(ChunkCoord other)
    {
        return Equals(this, other);
    }

    public static bool operator==(ChunkCoord lhs, ChunkCoord rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator!=(ChunkCoord lhs, ChunkCoord rhs)
    {
        return !lhs.Equals(rhs);
    }

    public static ChunkCoord operator+(ChunkCoord lhs, ChunkCoord rhs)
    {
        return new(lhs.x + rhs.x, lhs.z + rhs.z);
    }

    public static implicit operator ChunkCoord(Vector3 v)
    {
        return new ChunkCoord(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.z));
    }

    public static implicit operator ChunkCoord(Vector3Int v)
    {
        return new ChunkCoord(v.x, v.z);
    }
    #endregion
}