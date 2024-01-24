using System.Collections.Generic;
using UnityEngine;

// 2024-01-23 WJY
public class ResourceObjectSpawner
{
    [SerializeField] private ResourceObjectSpawnData _spawnData;

    private World _world;

    public void Initialize()
    {
        _spawnData = Managers.Resource.GetCache<ResourceObjectSpawnData>("ResourceObjectSpawnData.data");

        _world = Managers.Game.World;
        SpawnObject();
    }

    private void SpawnObject()
    {
        // TEST SCENE CODE
        if (_world == null)
        {
            foreach (var data in _spawnData.SpawnList)
                UnityEngine.Object.Instantiate(data.Prefab, data.spawnPosition, Quaternion.identity);
            return;
        }

        foreach (var data in _spawnData.SpawnList)
            _world.SpawnObjectInWorld(data.Prefab, data.spawnPosition);

        
    }
}