using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public struct ArchitectureKeyValue
{
    public ArchitectureKeyValue(string tag, int num)
    {
        this.tag = tag;
        this.num = num;
    }

    public string tag;
    public int num;
}

[Serializable]
public struct ArchitecutreTransform
{
    public ArchitecutreTransform(string name, Vector3 position, Quaternion rotation)
    {
        this.name = name;
        this.position = position;
        this.rotation = rotation;
    }

    public string name;
    public Vector3 position;
    public Quaternion rotation;
}


[System.Serializable]
public class ArchitectureSaveData
{
    [field: SerializeField] public List<ArchitectureKeyValue> architectureDicData { get; set; } = new();
    [field: SerializeField] public List<ArchitecutreTransform> transformData { get; set; } = new();
    [field: SerializeField] public List<ResourceObjectState> farmData { get; set; } = new();
}
