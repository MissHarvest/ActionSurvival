using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWorld", menuName = "WorldData/World", order = 2)]
public class WorldData : ScriptableObject
{
    [field: SerializeField] public Material Material { get; private set; }
    [field: SerializeField] public NormalBlockType[] NormalBlockTypes { get; private set; }
    [field: SerializeField] public SlideBlockType[] SlideBlockTypes { get; private set; }
    [field: SerializeField] public int WorldSize { get; private set; }
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

            chunk.AddTextureUV(GetTextureID(i));
            chunk.Triangles.Add(chunk.VertexIdx);
            chunk.Triangles.Add(chunk.VertexIdx + 1);
            chunk.Triangles.Add(chunk.VertexIdx + 2);
            chunk.Triangles.Add(chunk.VertexIdx + 2);
            chunk.Triangles.Add(chunk.VertexIdx + 1);
            chunk.Triangles.Add(chunk.VertexIdx + 3);
            chunk.VertexIdx += 4;
        }
    }
}

[System.Serializable]
public class SlideBlockType : BlockType
{
    [field: SerializeField] public Material FrontMaterial { get; protected set; }
    [field: SerializeField] public Material SideMaterial { get; protected set; }

    public override void AddVoxelDataToChunk(Chunk chunk, Vector3Int pos, Vector3 dir)
    {
        var obj = Managers.Resource.GetCache<GameObject>("Slide Block.prefab");
        obj = UnityEngine.Object.Instantiate(obj, pos, Quaternion.identity);
        var slide = obj.GetComponent<SlideBlock>();
        slide.Forward = dir;
        slide.FrontMaterial = FrontMaterial;
        slide.SideMaterial = SideMaterial;
        slide.transform.SetParent(chunk.InstanceBlocksParent);
        slide.name = $"{obj.name} ({pos})";
    }
}