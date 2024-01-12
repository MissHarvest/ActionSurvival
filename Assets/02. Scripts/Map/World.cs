using UnityEngine;

// 2024-01-12 WJY
public class World : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private BlockType[] blockTypes;

    public BlockType[] BlockTypes => blockTypes;
}


//TODO: Scriptable Object�� ���� �ɵ� ?
[System.Serializable]
public class BlockType
{
    public string blockName;
    public bool isSolid;
}