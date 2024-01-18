using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct IsLandProperty
{
    public Vector3 center;
    public int diameter;
    public int boundary;
    public int monsterCount;
    public int monsterInterval;

    public IsLandProperty(Vector3 center, int diameter = 300, int boundary = 3, int monsterCount = 50, int monsterInterval = 5)
    {
        this.center = center;
        this.diameter = diameter;            
        this.boundary = boundary;
        this.monsterCount = monsterCount;
        this.monsterInterval = monsterInterval;
    }
}
