using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

// 2024-01-12 WJY
public class Chunk : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshFilter meshFilter;

    private Dictionary<Vector3Int, byte> voxelMap = new();

    private int vertexIdx = 0;
    private List<Vector3> vertices = new();
    private List<int> triangles = new();
    private List<Vector2> uvs = new();

    [SerializeField] private World world;

    private void Start()
    {
        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }

    private void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                    voxelMap.TryAdd(new(x, y, z), 0);
    }

    private void CreateMeshData()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                    AddVoxelDataToChunk(new(x, y, z));
    }

    private void AddVoxelDataToChunk(Vector3 pos)
    {
        Vector3Int intPos = new(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));

        for (int i = 0; i < 6; i++)
        {
            if (CheckVoxel(pos + VoxelData.faceChecks[i]))
                continue;

            for (int j = 0; j < 4; j++)
            {
                vertices.Add(intPos + VoxelData.voxelVerts[VoxelData.voxelTris[i][j]]);
                //AddTextureUV(43);
            }
            triangles.Add(vertexIdx);
            triangles.Add(vertexIdx + 1);
            triangles.Add(vertexIdx + 2);
            triangles.Add(vertexIdx + 2);
            triangles.Add(vertexIdx + 1);
            triangles.Add(vertexIdx + 3);
            vertexIdx += 4;
        }
    }

    private void CreateMesh()
    {
        Mesh mesh = new()
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uvs.ToArray(),
        };
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    private bool CheckVoxel(Vector3 pos)
    {
        Vector3Int intPos = new(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));

        if (!voxelMap.ContainsKey(intPos))
            return false;
        else
            return world.BlockTypes[voxelMap[intPos]].isSolid;
    }

    private void AddTextureUV(int textureID)
    {
        (int w, int h) = (VoxelData.TextureAtlasWidth, VoxelData.TextureAtlasHeight);

        int x = textureID % w;
        int y = h - (textureID / w) - 1;

        AddTextureUV(x, y);
    }

    private void AddTextureUV(int x, int y)
    {
        if (x < 0 || y < 0 || x >= VoxelData.TextureAtlasWidth || y >= VoxelData.TextureAtlasHeight)
            throw new IndexOutOfRangeException($"텍스쳐 아틀라스의 범위를 벗어났습니다 : [x = {x}, y = {y}]");

        float nw = VoxelData.NormalizeTextureAtlasWidth;
        float nh = VoxelData.NormalizeTextureAtlasHeight;

        float uvX = x * nw;
        float uvY = y * nh;

        uvs.Add(new Vector2(uvX, uvY));
        uvs.Add(new Vector2(uvX, uvY + nh));
        uvs.Add(new Vector2(uvX + nw, uvY));
        uvs.Add(new Vector2(uvX + nw, uvY + nh));
    }
}