using UnityEngine;

// 2024-02-14 WJY
[CreateAssetMenu(fileName = "SeasonData", menuName = "ScriptableObjects/SeasonData")]
public class SeasonData : ScriptableObject
{
    [field: SerializeField] [field: Range(-1f, 1f)] public float FireIslandActivateThreshold { get; private set; }
    [field: SerializeField] [field: Range(-1f, 1f)] public float IceIslandActivateThreshold { get; private set; }
    [field: SerializeField] public AnimationCurve SeasonCurve { get; private set; }
    [field: SerializeField] public int CycleValue { get; private set; }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (FireIslandActivateThreshold < IceIslandActivateThreshold)
            (FireIslandActivateThreshold, IceIslandActivateThreshold) = (IceIslandActivateThreshold, FireIslandActivateThreshold);
    }
#endif
}