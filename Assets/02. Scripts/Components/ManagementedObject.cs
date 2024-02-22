using System;
using System.Collections.Generic;
using UnityEngine;

// 2024-01-29 WJY
public class ManagementedObject : MonoBehaviour
{
    private ChunkCoord _coord;
    private World _world;
    private Transform _tranform;
    private ObjectManager _manager;

    private HashSet<ObjectManagementProtocol> _managedTargets = new(comparer: new ObjectManagementProtocol());

    private void Start()
    {
        _world = GameManager.Instance.World;
        _manager = GameManager.ObjectManager;
        _tranform = transform;

        _world.OnWorldUpdated += SwitchEnabled;
        SwitchEnabled();
    }

    private void OnDestroy()
    {
        if (_world)
            _world.OnWorldUpdated -= SwitchEnabled;
    }

    public void Add(object target, Type type)
    {
        _managedTargets.Add(new(target, type));
    }

    public void AddRange(IEnumerable<object> targets, Type type)
    {
        foreach (var e in targets)
            Add(e, type);
    }

    public void Remove(object target)
    {
        _managedTargets.Remove(new(target, null));
    }

    public void RemoveRange(IEnumerable<object> targets)
    {
        foreach (var e in targets)
            Remove(e);
    }

    public void SwitchEnabled()
    {
        _coord = _world.ConvertChunkCoord(_tranform.position);
        if (!_world.ChunkMap.TryGetValue(_coord, out var chunk))
            return;

        bool enabled = chunk.IsActive;
        foreach (var target in _managedTargets)
            _manager.ManageObject(target, enabled);
    }
}