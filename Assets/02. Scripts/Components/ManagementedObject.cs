using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

// 2024-01-29 WJY
public class ManagementedObject : MonoBehaviour
{
    private ChunkCoord _coord;
    private World _world;
    private Transform _tranform;
    private ObjectManager _manager;

    public List<ObjectManagingProtocol> managedTargets = new();
    
    private void Start()
    {
        _world = Managers.Game.World;
        _manager = Managers.Game.ObjectManager;
        _tranform = transform;

        _coord = _world.ConvertChunkCoord(_tranform.position);
        _world.OnWorldUpdated += SwitchEnabled;
        SwitchEnabled();
    }

    public void SwitchEnabled()
    {
        _coord = _world.ConvertChunkCoord(_tranform.position);
        if (_world.ChunkMap.TryGetValue(_coord, out var chunk))
        {
            bool enabled = chunk.IsActive;
            foreach (var target in managedTargets)
                _manager.ManageObject(target, enabled);
        }
    }
}