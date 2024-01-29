using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

// 2024-01-12 WJY
public class Chunk
{
    private GameObject _chunkObject;
    private Transform _instanceBlockParent;
    private Transform _instanceObjectParent;
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private ChunkCoord _coord;

    private int _vertexIdx = 0;
    private List<Vector3> _vertices = new();
    private List<int> _triangles = new();
    private List<Vector2> _uvs = new();

    private World _world;
    private VoxelData _data;

    public ChunkCoord ChunkCoord => _coord;
    public bool IsActive
    {
        get => _meshRenderer.enabled;
        set { if (_meshRenderer.enabled != value) _meshRenderer.enabled = value; }
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
    public Transform InstanceObjectParent => _instanceObjectParent;

    private List<WorldMapData> _localMap = new();

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
        _instanceObjectParent = new GameObject("Instance Object Parent").transform;
        _instanceObjectParent.SetParent(_chunkObject.transform);
    }

    public void AddVoxel(WorldMapData data)
    {
        _localMap.Add(data);
    }

    public void GenerateChunk()
    {
        CreateMeshData();
        CreateMesh();
    }

    private void CreateMeshData()
    {
        foreach(var e in _localMap)
            e.type.AddVoxelDataToChunk(this, e.position, e.forward);
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

    public void AddTextureUV(int textureID)
    {
        (int w, int h) = (_data.TextureAtlasWidth, _data.TextureAtlasHeight);

        int x = textureID % w;
        int y = h - (textureID / w) - 1;

        AddTextureUV(x, y);
    }

    private void AddTextureUV(int x, int y)
    {
        if (x < 0 || y < 0 || x >= _data.TextureAtlasWidth || y >= _data.TextureAtlasHeight)
            Debug.LogError($"텍스쳐 아틀라스의 범위를 벗어났습니다 : [x = {x}, y = {y}]");

        float nw = _data.NormalizeTextureAtlasWidth;
        float nh = _data.NormalizeTextureAtlasHeight;

        float uvX = x * nw;
        float uvY = y * nh;

        _uvs.Add(new Vector2(uvX + _data.uvXBeginOffset, uvY + _data.uvYBeginOffset));
        _uvs.Add(new Vector2(uvX + _data.uvXBeginOffset, uvY + nh + _data.uvYEndOffset));
        _uvs.Add(new Vector2(uvX + nw + _data.uvXEndOffset, uvY + _data.uvYBeginOffset));
        _uvs.Add(new Vector2(uvX + nw + _data.uvXEndOffset, uvY + nh + _data.uvYEndOffset));
    }

    public void SetActive(bool active) => IsActive = active;

    public (Mesh, Matrix4x4)[] GetAllBlocksNavMeshSourceData()
    {
        (Mesh, Matrix4x4)[] result = new (Mesh, Matrix4x4)[_instanceBlockParent.childCount + 1];
        result[0] = (Mesh, TransformMatrix);
        for (int i = 1; i < result.Length; i++)
        {
            var block = _instanceBlockParent.GetChild(i - 1).GetComponent<SlideBlock>();
            result[i] = (block.Mesh, block.TransformMatrix);
        }
        return result;
    }

    public void AddInstanceObject(Transform obj)
    {
        obj.parent = _instanceObjectParent;
    }

    public void AddInstanceObject(GameObject obj)
    {
        AddInstanceObject(obj.transform);
    }

    public void AddInstanceObject<T>(T obj) where T : MonoBehaviour
    {
        AddInstanceObject(obj.transform);
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