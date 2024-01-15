using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWorld", menuName = "WorldData", order = 2)]
public class WorldData : ScriptableObject
{
    [field: SerializeField] public Material Material { get; private set; }
    [field: SerializeField] public BlockType[] BlockTypes { get; private set; }
    [field: SerializeField] public int WorldSize { get; private set; }
    [field: SerializeField] public int ViewRange { get; private set; }
}


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

    public int GetTextureID(int faceIndex)
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