using System;
using System.Collections.Generic;
using UnityEngine;

// 2024-01-29 WJY
public class ObjectManager
{
    private Dictionary<Type, Action<object, bool>> _process = new();

    public ObjectManager()
    {
        _process.Add(typeof(Renderer[]), (x, enabled) =>
        {
            foreach (var e in x as Renderer[]) 
                e.enabled = enabled; 
        });

        _process.Add(typeof(Renderer), (x, enabled) => 
        {
            (x as Renderer).enabled = enabled;
        });

        _process.Add(typeof(GameObject[]), (x, enabled) =>
        {
            foreach (var e in x as GameObject[])
                e.SetActive(enabled);
        });

        _process.Add(typeof(GameObject), (x, enabled) =>
        {
            (x as GameObject).SetActive(enabled); 
        });

        _process.Add(typeof(Behaviour[]), (x, enabled) => 
        {
            foreach (var e in x as Behaviour[]) 
                e.enabled = enabled; 
        });

        _process.Add(typeof(Behaviour), (x, enabled) => 
        {
            (x as Behaviour).enabled = enabled; 
        });

        _process.Add(typeof(Collider[]), (x, enabled) =>
        {
            foreach (var e in x as Collider[])
                e.enabled = enabled;
        });

        _process.Add(typeof(Collider), (x, enabled) =>
        {
            (x as Collider).enabled = enabled;
        });
    }

    public void ManageObject(ObjectManagementProtocol protocol, bool chunkEnabled)
    {
        _process[protocol.key]?.Invoke(protocol.target, chunkEnabled);
    }
}

public class ObjectManagementProtocol
{
    public object target;
    public Type key;

    public ObjectManagementProtocol(object target, Type key)
    {
        this.target = target;
        this.key = key;
    }
}