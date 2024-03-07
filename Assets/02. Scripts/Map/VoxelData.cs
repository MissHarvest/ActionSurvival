using UnityEngine;

// 2024-01-12 WJY
[CreateAssetMenu(fileName = "VoxelData", menuName = "WorldData/VoxelData", order = 2)]
public class VoxelData : ScriptableObject
{
    [field: Header("Chunk Size")]
    [field: SerializeField] public int ChunkSizeX { get; private set; } = 10;
    [field: SerializeField] public int ChunkSizeY { get; private set; } = 1;
    [field: SerializeField] public int ChunkSizeZ { get; private set; } = 10;

    [field: Header("Texture Atlas Size")]
    [field: SerializeField] public int TextureAtlasWidth { get; private set; } = 9;
    [field: SerializeField] public int TextureAtlasHeight { get; private set; } = 10;

    public float NormalizeTextureAtlasWidth => 1f / TextureAtlasWidth;
    public float NormalizeTextureAtlasHeight => 1f / TextureAtlasHeight;

    [field: Header("Texture Atlas Offset")]
    // 텍스쳐 내에서 의도치 않게 들어가는 부분 잘라내기
    [field: SerializeField] public float uvXBeginOffset { get; private set; } = 0.003f;
    [field: SerializeField] public float uvXEndOffset { get; private set; } = -0.003f;
    [field: SerializeField] public float uvYBeginOffset { get; private set; } = 0.003f;
    [field: SerializeField] public float uvYEndOffset { get; private set; } = -0.003f;
}

public readonly struct VoxelLookUpTable
{
    // Voxel과 관련된 LookUp Table을 정의해둡니다.

    #region LookUp Table
    /// <summary> 정육면체 8개 정점 위치 </summary>
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

    /// <summary> 정육면체의 한 면을 이루는 두 삼각형의 정점 인덱스 데이터 </summary>
    public static readonly int[][] voxelTris = new int[][]
    {
        new int[]{ 0, 3, 1, 2 },
        new int[]{ 5, 6, 4, 7 },
        new int[]{ 3, 7, 2, 6 },
        new int[]{ 1, 5, 0, 4 },
        new int[]{ 4, 7, 0, 3 },
        new int[]{ 1, 2, 5, 6 },
    };

    /// <summary> 삼각형 정점 인덱스에 따라 정의된 UV 데이터 </summary>
    public static readonly Vector2[] voxelUVs = new Vector2[]
    {
        new(0f, 0f),
        new(0f, 1f),
        new(1f, 0f),
        new(1f, 1f),
    };

    /// <summary> 청크의 바깥 면만 그려주기 위한 LookUp Table </summary>
    public static readonly Vector3[] faceChecks = new Vector3[]
    {
        new( 0.0f,  0.0f, -1.0f),
        new( 0.0f,  0.0f, +1.0f),
        new( 0.0f, +1.0f,  0.0f),
        new( 0.0f, -1.0f,  0.0f),
        new(-1.0f,  0.0f,  0.0f),
        new(+1.0f,  0.0f,  0.0f),
    };
    #endregion
}