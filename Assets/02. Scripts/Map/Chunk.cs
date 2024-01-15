using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

// 2024-01-12 WJY
public class Chunk
{
    private GameObject chunkObject;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private ChunkCoord coord;

    private int vertexIdx = 0;
    private List<Vector3> vertices = new();
    private List<int> triangles = new();
    private List<Vector2> uvs = new();

    private World world;

    public ChunkCoord ChunkCoord => coord;

    public Chunk(ChunkCoord coord, World world)
    {
        this.world = world;
        this.coord = coord;

        chunkObject = new($"{nameof(Chunk)} {coord.x:D2}, {coord.z:D2}");
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshFilter = chunkObject.AddComponent<MeshFilter>();

        meshRenderer.material = world.Material;
        chunkObject.transform.SetParent(world.transform);
        //chunkObject.transform.position = new(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);

        CreateMeshData();
        CreateMesh();
    }

    private void CreateMeshData()
    {
        //foreach (var pos in world.VoxelMap.Keys)
        //    AddVoxelDataToChunk(pos);

        for (int y = 0; y < VoxelData.ChunkHeight; y++)
            for (int x = VoxelData.ChunkWidth * coord.x; x < VoxelData.ChunkWidth * (coord.x + 1); x++)
                for (int z = VoxelData.ChunkWidth * coord.z; z < VoxelData.ChunkWidth * (coord.z + 1); z++)
                    if (world.VoxelMap.ContainsKey(new(x, y, z)))
                        AddVoxelDataToChunk(new(x, y, z));
    }

    private void AddVoxelDataToChunk(Vector3Int pos)
    {
        for (int i = 0; i < 6; i++)
        {
            if (CheckVoxel(pos + VoxelData.faceChecks[i]))
                continue;

            for (int j = 0; j < 4; j++)
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[i][j]]);

            var blockID = world.VoxelMap[pos];

            AddTextureUV(world.BlockTypes[blockID].GetTextureID(i));
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

        if (!world.VoxelMap.ContainsKey(intPos))
            return false;
        else
            return world.BlockTypes[world.VoxelMap[intPos]].isSolid;
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
            Debug.LogError($"텍스쳐 아틀라스의 범위를 벗어났습니다 : [x = {x}, y = {y}]");

        float nw = VoxelData.NormalizeTextureAtlasWidth;
        float nh = VoxelData.NormalizeTextureAtlasHeight;

        float uvX = x * nw;
        float uvY = y * nh;

        uvs.Add(new Vector2(uvX + VoxelData.uvXBeginOffset, uvY + VoxelData.uvYBeginOffset));
        uvs.Add(new Vector2(uvX + VoxelData.uvXBeginOffset, uvY + nh + VoxelData.uvYEndOffset));
        uvs.Add(new Vector2(uvX + nw + VoxelData.uvXEndOffset, uvY + VoxelData.uvYBeginOffset));
        uvs.Add(new Vector2(uvX + nw + VoxelData.uvXEndOffset, uvY + nh + VoxelData.uvYEndOffset));
    }
}

public class ChunkCoord
{
    public int x;
    public int z;

    public ChunkCoord(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
}