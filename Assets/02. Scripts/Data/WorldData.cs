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
}

[System.Serializable]
public class BlockType
{
    [field: SerializeField] public string BlockName { get; protected set; }
    [field: SerializeField] public bool IsSolid { get; protected set; }
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
}

[System.Serializable]
public class SlideBlockType : BlockType
{
    [field: SerializeField] public Material FrontMaterial { get; protected set; }
    [field: SerializeField] public Material SideMaterial { get; protected set; }
    [field: SerializeField] public Vector3 Forward { get; protected set; } = Vector3.forward;

    public SlideBlockType() { }

    public SlideBlockType(SlideBlockType data, Vector3 dir)
    {
        BlockName = data.BlockName;
        IsSolid = data.IsSolid;
        FrontMaterial = data.FrontMaterial;
        SideMaterial = data.SideMaterial;
        Forward = dir;
    }
}