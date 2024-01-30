using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct IslandProperty
{
    public Vector3 center;
    public string name;
    public int diameter;
    public int boundary;
    public int monsterCount;
    public int monsterInterval;

    public IslandProperty(Vector3 center, string name, int diameter = 300, int boundary = 3, int monsterCount = 50, int monsterInterval = 5)
    {
        this.center = center;
        this.name = name;
        this.diameter = diameter;            
        this.boundary = boundary;
        this.monsterCount = monsterCount;
        this.monsterInterval = monsterInterval;
    }
}
