using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// 2024-02-20 WJY
[BurstCompile]
public struct ChunkJob : IJob
{
    // 입력
    [ReadOnly] public NativeArray<bool> checks;
    [ReadOnly] public NativeArray<int> textures;
    [ReadOnly] public NativeArray<Vector3> positions;
    public int faceIdx;
    public int vertextIdx;

    // 텍스처 정보
    [ReadOnly] public int textureAtlasWidth;
    [ReadOnly] public int textureAtlasHeight;
    [ReadOnly] public float normalizeTextureAtlasWidth;
    [ReadOnly] public float normalizeTextureAtlasHeight;
    [ReadOnly] public float uvXBeginOffset;
    [ReadOnly] public float uvXEndOffset;
    [ReadOnly] public float uvYBeginOffset;
    [ReadOnly] public float uvYEndOffset;

    // 출력
    [WriteOnly] public NativeList<Vector3> vertices;
    [WriteOnly] public NativeList<int> triangles;
    [WriteOnly] public NativeList<Vector2> uv;

    public void Execute()
    {
        for (int blockCount = 0; blockCount < positions.Length; blockCount++)
        {
            for (int i = 0; i < 6; i++)
            {
                if (checks[faceIdx + i])
                    continue;

                for (int j = 0; j < 4; j++)
                    vertices.Add(positions[blockCount] + VoxelLookUpTable.voxelVerts[VoxelLookUpTable.voxelTris[i][j]]);

                int textureX = textures[faceIdx + i] % textureAtlasWidth;
                int textureY = textureAtlasHeight - (textures[faceIdx + i] / textureAtlasWidth) - 1;
                float uvX = textureX * normalizeTextureAtlasWidth;
                float uvY = textureY * normalizeTextureAtlasHeight;
                uv.Add(new Vector2(uvX + uvXBeginOffset, uvY + uvYBeginOffset));
                uv.Add(new Vector2(uvX + uvXBeginOffset, uvY + normalizeTextureAtlasHeight + uvYEndOffset));
                uv.Add(new Vector2(uvX + normalizeTextureAtlasWidth + uvXEndOffset, uvY + uvYBeginOffset));
                uv.Add(new Vector2(uvX + normalizeTextureAtlasWidth + uvXEndOffset, uvY + normalizeTextureAtlasHeight + uvYEndOffset));

                triangles.Add(vertextIdx);
                triangles.Add(vertextIdx + 1);
                triangles.Add(vertextIdx + 2);
                triangles.Add(vertextIdx + 2);
                triangles.Add(vertextIdx + 1);
                triangles.Add(vertextIdx + 3);
                vertextIdx += 4;
            }
            faceIdx += 6;
        }
    }

    public (NativeList<Vector3> vertices, NativeList<int> triangles, NativeList<Vector2> uv) GetResult()
    {
        return (vertices, triangles, uv);
    }

    public void Dispose()
    {
        checks.Dispose();
        textures.Dispose();
        positions.Dispose();
        vertices.Dispose();
        triangles.Dispose();
        uv.Dispose();
    }
}