using UnityEngine;

[CreateAssetMenu(fileName = "ToolItemData", menuName = "New Item/Tool", order = 0)]
public class ToolItemData : ItemData
{
    public LayerMask targetLayers;
    public float range;
    public string targetTagName;

    public ToolItemData()
    {
        _maxStackCount = 1;
        registable = true;
    }
}
