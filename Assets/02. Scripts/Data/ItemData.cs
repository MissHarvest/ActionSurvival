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

    protected int _maxStackCount = 20;
    public int MaxStackCount => _maxStackCount;

    public float MaxDurability { get; private set; } = 0.0f;

    public bool stackable => MaxStackCount != 1;
    public bool registable { get; protected set; } = false;

    public float lootingRatio { get; private set; } = 0.0f;
}
