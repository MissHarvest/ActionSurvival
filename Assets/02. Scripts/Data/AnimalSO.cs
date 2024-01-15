using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Animal", menuName = "Animal")]
public class AnimalSO : ScriptableObject
{
    [field: SerializeField] public int MaxHP { get; private set; }
    [field: SerializeField] public AnimalMovementData MovementData { get; private set; }

}
