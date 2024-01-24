using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceObjectDebris : ResourceObjectBase
{
    private float _remainingTime;
    [SerializeField] private float _respawnTime;

    private void Update()
    {
        Respawn();
    }

    public void Respawn()
    {
        _remainingTime -= Time.deltaTime;
        if (_remainingTime < 0f)
        {
            _parent.SwitchObject(_toObjectID);
            _remainingTime = _respawnTime;
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        _remainingTime = _respawnTime;
    }
}