using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWorld", menuName = "WorldData/World", order = 2)]
public class WorldData : ScriptableObject
{
    [field: SerializeField] public Material Material { get; private set; }
    [field: SerializeField] public NormalBlockType[] NormalBlockTypes { get; private set; }
    [field: SerializeField] public SlideBlockType[] SlideBlockTypes { get; private set; }
    [field: SerializeField] public int ViewChunkRange { get; private set; }

    public BlockType[] GetType(MapEditDataSource.Inherits type)
    {
        return type switch
        {
            MapEditDataSource.Inherits.Normal => NormalBlockTypes,
            MapEditDataSource.Inherits.Slide => SlideBlockTypes,
            _ => NormalBlockTypes,
        };
    }
}

[System.Serializable]
public abstract class BlockType
{
    [field: SerializeField] public string BlockName { get; protected set; }
    [field: SerializeField] public bool IsSolid { get; protected set; }

    public abstract void AddVoxelDataToChunk(Chunk chunk, Vector3Int pos, Vector3 dir);
}

[System.Serializable]
public class NormalBlockType : BlockType
{
    [field: SerializeField] public int BackFaceTexture { get; protected set; }
    [field: SerializeField] public int FrontFaceTexture { get; protected set; }
    [field: SerializeField] public int TopFaceTexture { get; protected set; }
    [field: SerializeField] public int BottomFaceTexture { get; protected set; }
    [field: SerializeField] public int LeftFaceTexture { get; protected set; }
    [field: SerializeField] public int RightFaceTexture { get; protected set; }

    public int GetTextureID(int faceIndex)
    {
        switch (faceIndex)
        {
            case 0: return BackFaceTexture;
            case 1: return FrontFaceTexture;
            case 2: return TopFaceTexture;
            case 3: return BottomFaceTexture;
            case 4: return LeftFaceTexture;
            case 5: return RightFaceTexture;
            default: Debug.LogError($"{nameof(GetTextureID)}: Invalid face index."); return 0;
        }
    }

    public override void AddVoxelDataToChunk(Chunk chunk, Vector3Int pos, Vector3 dir)
    {
        for (int i = 0; i < 6; i++)
        {
            if (chunk.World.CheckVoxel(pos + chunk.Data.faceChecks[i]))
                continue;

            for (int j = 0; j < 4; j++)
                chunk.Vertices.Add(pos + chunk.Data.voxelVerts[chunk.Data.voxelTris[i][j]]);

            AddTextureUV(GetTextureID(i), chunk);
            chunk.Triangles.Add(chunk.VertexIdx);
            chunk.Triangles.Add(chunk.VertexIdx + 1);
            chunk.Triangles.Add(chunk.VertexIdx + 2);
            chunk.Triangles.Add(chunk.VertexIdx + 2);
            chunk.Triangles.Add(chunk.VertexIdx + 1);
            chunk.Triangles.Add(chunk.VertexIdx + 3);
            chunk.VertexIdx += 4;
        }
    }

    public void AddTextureUV(int textureID, Chunk chunk)
    {
        (int w, int h) = (chunk.Data.TextureAtlasWidth, chunk.Data.TextureAtlasHeight);
        int x = textureID % w;
        int y = h - (textureID / w) - 1;

        AddTextureUV(x, y, chunk);
    }

    private void AddTextureUV(int x, int y, Chunk chunk)
    {
        if (x < 0 || y < 0 || x >= chunk.Data.TextureAtlasWidth || y >= chunk.Data.TextureAtlasHeight)
            Debug.LogError($"텍스쳐 아틀라스의 범위를 벗어났습니다 : [x = {x}, y = {y}]");

        float nw = chunk.Data.NormalizeTextureAtlasWidth;
        float nh = chunk.Data.NormalizeTextureAtlasHeight;

        float uvX = x * nw;
        float uvY = y * nh;

        chunk.Uvs.Add(new Vector2(uvX + chunk.Data.uvXBeginOffset, uvY + chunk.Data.uvYBeginOffset));
        chunk.Uvs.Add(new Vector2(uvX + chunk.Data.uvXBeginOffset, uvY + nh + chunk.Data.uvYEndOffset));
        chunk.Uvs.Add(new Vector2(uvX + nw + chunk.Data.uvXEndOffset, uvY + chunk.Data.uvYBeginOffset));
        chunk.Uvs.Add(new Vector2(uvX + nw + chunk.Data.uvXEndOffset, uvY + nh + chunk.Data.uvYEndOffset));
    }
}

[System.Serializable]
public class SlideBlockType : BlockType
{
    [field: SerializeField] public GameObject Prefab { get; protected set; }

    public override void AddVoxelDataToChunk(Chunk chunk, Vector3Int pos, Vector3 dir)
    {
        var obj = UnityEngine.Object.Instantiate(Prefab, pos, Quaternion.identity);
        var slide = obj.GetComponent<InstanceBlock>();
        slide.Forward = dir;
        slide.transform.SetParent(chunk.InstanceBlocksParent);
        slide.name = $"{BlockName} ({pos})";
        chunk.InstanceBlocks.Add(slide);
    }
}// 2024-02-