using System;
using System.Collections.Generic;
using UnityEngine;

// 2024-01-29 WJY
public class ObjectManager
{
    private Dictionary<Type, Action<object, bool>> _process = new();

    public ObjectManager()
    {
        _process.Add(typeof(Renderer), (x, enabled) => 
        {
            (x as Renderer).enabled = enabled;
        });

        _process.Add(typeof(GameObject), (x, enabled) =>
        {
            (x as GameObject).SetActive(enabled); 
        });

        _process.Add(typeof(Behaviour), (x, enabled) => 
        {
            (x as Behaviour).enabled = enabled; 
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

public class ObjectManagementProtocol : IEqualityComparer<ObjectManagementProtocol>
{
    public object target;
    public Type key;

    public ObjectManagementProtocol() { }

    public ObjectManagementProtocol(object target, Type key)
    {
        this.target = target;
        this.key = key;
    }

    public bool Equals(ObjectManagementProtocol x, ObjectManagementProtocol y)
    {
        return x.target == y.target;
    }

    public int GetHashCode(ObjectManagementProtocol obj)
    {
        return obj.target.GetHashCode();
    }
}