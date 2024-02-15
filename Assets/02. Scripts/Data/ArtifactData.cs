using UnityEngine;

// 2024-02-15 WJY
[CreateAssetMenu(fileName = "ArtifactData", menuName = "ScriptableObjects/Artifact")]
public class ArtifactData : ScriptableObject
{
    [field: SerializeField] public Mesh[] Model { get; private set; }
    [field: SerializeField] public float HPMax { get; private set; }
    [field: SerializeField] public float HpRegen { get; private set; }
    [field: SerializeField] public float InfluenceAmount { get; private set; }
    [field: SerializeField] public ItemDropTable[] LootingData { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public int CreateCount { get; private set; }

    private Artifact _artifact;
    public Artifact Artifact
    {
        get 
        {
            if (_artifact == null)
                _artifact = Prefab.GetComponent<Artifact>();
            return _artifact;
        }
    }
}