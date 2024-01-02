using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Default")]
    public string displayName;
    public string description;
    public Sprite iconSprite;

    public static int maxStackCount = 20;

    public bool stackable { get; protected set; } = true;
    public bool registable { get; protected set; } = false;
}
