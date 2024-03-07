using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 27
[System.Serializable]
public struct ResourceObjectState
{
    public ResourceObjectState(ResourceObjectParent resourceObject)
    {
        this.state = resourceObject.CurrentState;
        this.remainingTime = resourceObject.RemainingTime;
    }

    public int state;
    public int remainingTime;
}

[System.Serializable]
public class ResourceObjectSaveData
{
    [field: SerializeField] public List<ResourceObjectState> resourceObjectsState { get; set; } = new();
}
