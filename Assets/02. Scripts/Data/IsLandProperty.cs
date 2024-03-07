using UnityEngine;

// 2024. 02. 22 Park Jun Uk
[System.Serializable]
public class IslandProperty
{
    [field: SerializeField] public Vector3 Center { get; private set; }
    [field: SerializeField] public string name;
    [field: SerializeField] public int Diameter { get; private set; } = 300;
    public Vector3 Offset => new Vector3(Diameter / 2, 0, Diameter / 2) - Center;
    [field: SerializeField] public float Temperature { get; private set; }
    [field: SerializeField] public float Influence { get; private set; } = 1.0f;
}