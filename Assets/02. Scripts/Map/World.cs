using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024-01-12 WJY
public class World : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private BlockType[] blockTypes;
    [SerializeField] private int worldSizeByChunk;

    private Dictionary<(int x, int z), Chunk> chunkMap;
    private Dictionary<Vector3Int, byte> voxelMap = new();


    public BlockType[] BlockTypes => blockTypes;
    public Material Material => material;
    public Dictionary<Vector3Int, byte> VoxelMap => voxelMap;

    private void Start()
    {
        chunkMap = new();
        PopulateVoxelMap();
        GenerateChunk();
    }

    private void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
            for (int x = 0; x < VoxelData.ChunkWidth * worldSizeByChunk; x++)
                for (int z = 0; z < VoxelData.ChunkWidth * worldSizeByChunk; z++)
                    voxelMap.TryAdd(new(x, y, z), 0);
        voxelMap.Remove(new(0, 0, 0));
    }

    private void GenerateChunk()
    {
        for (int i = 0; i < worldSizeByChunk; i++)
            for (int j = 0; j < worldSizeByChunk; j++)
                chunkMap.Add(new(i, j), new(new(i, j), this));
    }
}


//TODO: Scriptable Object로 빼도 될듯 ?
[System.Serializable]
public class BlockType
{
    public string blockName;
    public bool isSolid;

    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;

    public int GetTextureID (int faceIndex)
    {
        switch (faceIndex)
        {
            case 0: return backFaceTexture;
            case 1: return frontFaceTexture;
            case 2: return topFaceTexture;
            case 3: return bottomFaceTexture;
            case 4: return leftFaceTexture;
            case 5: return rightFaceTexture;
            default: Debug.LogError($"{nameof(GetTextureID)}: Invalid face index."); return 0;
        }
    }
}