using UnityEngine;

// 2024. 02. 22 Park Jun Uk
[System.Serializable]
public class IslandMonsterDistribution
{
    [SerializeField] private int _lowerMonsterRatio;
    [SerializeField] private int _middleMonsterRatio;
    [SerializeField] private int _upperMonsterRatio;
    private int _total => _lowerMonsterRatio + _middleMonsterRatio + _upperMonsterRatio;

    public float LowerMonsterRatio => (float)_lowerMonsterRatio / _total;
    public float MiddleMonsterRatio => (float)_middleMonsterRatio / _total;
    public float UpperMonsterRatio => (float)_upperMonsterRatio / _total;

    public GameObject[] LowerMonsters;
    public GameObject[] MiddleMonsters;
    public GameObject[] UpperMonsters;
}
