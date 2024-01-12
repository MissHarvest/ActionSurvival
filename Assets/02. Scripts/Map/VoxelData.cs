using UnityEngine;

// 2024-01-12 WJY
public static class VoxelData
{
    // Voxel�� ���õ� LookUp Table�� �����صӴϴ�.

    /// <summary> ������ü 8�� ���� ��ġ </summary>
    public static readonly Vector3[] voxelVerts = new Vector3[]
    {
        new(0f, 0f, 0f),
        new(1f, 0f, 0f),
        new(1f, 1f, 0f),
        new(0f, 1f, 0f),
        new(0f, 0f, 1f),
        new(1f, 0f, 1f),
        new(1f, 1f, 1f),
        new(0f, 1f, 1f),
    };

    /// <summary> ������ü�� �� ���� �̷�� �� �ﰢ���� ���� �ε��� ������ </summary>
    public static readonly int[][] voxelTris = new int[][]
    {
        new int[]{ 0, 3, 1, 2 },
        new int[]{ 5, 6, 4, 7 },
        new int[]{ 3, 7, 2, 6 },
        new int[]{ 1, 5, 0, 4 },
        new int[]{ 4, 7, 0, 3 },
        new int[]{ 1, 2, 5, 6 },
    };

    /// <summary> �ﰢ�� ���� �ε����� ���� ���ǵ� UV ������ </summary>
    public static readonly Vector2[] voxelUVs = new Vector2[]
    {
        new(0f, 0f),
        new(0f, 1f),
        new(1f, 0f),
        new(1f, 1f),
    };

    /// <summary> ûũ�� �ٱ� �鸸 �׷��ֱ� ���� LookUp Table </summary>
    public static readonly Vector3[] faceChecks = new Vector3[]
    {
        new( 0.0f,  0.0f, -1.0f),
        new( 0.0f,  0.0f, +1.0f),
        new( 0.0f, +1.0f,  0.0f),
        new( 0.0f, -1.0f,  0.0f),
        new(-1.0f,  0.0f,  0.0f),
        new(+1.0f,  0.0f,  0.0f),
    };

    public static readonly int ChunkWidth = 10;
    public static readonly int ChunkHeight = 10;

    public static readonly int TextureAtlasWidth = 9;
    public static readonly int TextureAtlasHeight = 10;

    public static float NormalizeTextureAtlasWidth => 1f / TextureAtlasWidth;
    public static float NormalizeTextureAtlasHeight => 1f / TextureAtlasHeight;
}