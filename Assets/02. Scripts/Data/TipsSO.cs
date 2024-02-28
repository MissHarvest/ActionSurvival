using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tips", menuName = "Tips")]
public class TipsSO : ScriptableObject
{
    [SerializeField] public List<TipData> tips = new();
}
