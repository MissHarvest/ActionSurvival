using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

// 2024-01-23 WJY
public class ResourceObjectSpawner
{
    [SerializeField] private ResourceObjectSpawnData _spawnData;

    private World _world;

    [field: SerializeField] public List<ResourceObjectParent> _resourceObjects { get; set; } = new();

    public void Initialize()
    {
        _spawnData = Managers.Resource.GetCache<ResourceObjectSpawnData>("ResourceObjectSpawnData.data");

        _world = Managers.Game.World;
        SpawnObject();

        Managers.Game.OnSaveCallback += Save;
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

        //Load//
        SaveGame.TryLoadJsonFile<ResourceObjectSaveData>(SaveGame.SaveType.Runtime, "ResourceObjectsState", out ResourceObjectSaveData json);
        
        //foreach (var data in _spawnData.SpawnList)
        for(int i = 0; i < _spawnData.SpawnList.Count; ++i)
        {
            var data = _spawnData.SpawnList[i];
            var go = _world.SpawnObjectInWorld(data.Prefab, data.spawnPosition);
            var resourceObjectParent = go.GetComponent<ResourceObjectParent>();
            if(json.resourceObjectsState.Count > 0)
            {
                resourceObjectParent.SetInfo(json.resourceObjectsState[i].state, json.resourceObjectsState[i].remainingTime);
            }
            _resourceObjects.Add(resourceObjectParent);
        }
    }

    private void Save()
    {
        ResourceObjectSaveData ro = new();
        
        foreach (var obj in _resourceObjects)
        {
            ro.resourceObjectsState.Add(new ResourceObjectState(obj));
        }
        var json = JsonUtility.ToJson(ro);
        SaveGame.CreateJsonFile("ResourceObjectsState", json, SaveGame.SaveType.Runtime);
    }
}
