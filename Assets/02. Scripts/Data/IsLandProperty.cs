using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct IslandProperty
{
    public Vector3 center;
    public string name;
    public int diameter;
    public int boundary;
    public int monsterCount;
    public int monsterInterval;
    [field: SerializeField] public float Temperature { get; set; }
    [field: SerializeField] public float Influence { get; set; }

    public IslandProperty(Vector3 center, string name, float temperature, float influence, int diameter = 300, int boundary = 3, int monsterCount = 50, int monsterInterval = 5)
    {
        this.center = center;
        this.name = name;
        this.diameter = diameter;            
        this.boundary = boundary;
        this.monsterCount = monsterCount;
        this.monsterInterval = monsterInterval;
        Temperature = temperature;
        Influence = influence;
    }
}

[System.Serializable]
public struct IslandPropertySaveData
{
    public List<IslandProperty> dataList;
}