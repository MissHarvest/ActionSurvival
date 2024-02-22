using System.Collections.Generic;
using UnityEngine;

// 2024-01-23 WJY
public class ResourceObjectSpawner
{
    [SerializeField] private ResourceObjectSpawnData _spawnData;
    private Transform _resourceObjectRoot;

    private World _world;

    [field: SerializeField] public List<ResourceObjectParent> _resourceObjects { get; set; } = new();

    public void Initialize()
    {
        _spawnData = Managers.Resource.GetCache<ResourceObjectSpawnData>("ResourceObjectSpawnData.data");
        _resourceObjectRoot = new GameObject("ResourceObjectRoot").transform;

        _world = GameManager.Instance.World;
        SpawnObjects();

        GameManager.Instance.OnSaveCallback += Save;
    }

    private void SpawnObjects()
    {
        //Load//
        SaveGame.TryLoadJsonFile<ResourceObjectSaveData>(SaveGame.SaveType.Runtime, "ResourceObjectsState", out ResourceObjectSaveData json);
        
        //foreach (var data in _spawnData.SpawnList)
        for(int i = 0; i < _spawnData.SpawnList.Count; ++i)
        {
            var data = _spawnData.SpawnList[i];
            //var go = _world.SpawnObjectInWorld(data.Prefab, data.spawnPosition);
            var go = UnityEngine.Object.Instantiate(data.Prefab, data.spawnPosition, Quaternion.identity);
            go.transform.parent = _resourceObjectRoot;
            var resourceObjectParent = go.GetComponent<ResourceObjectParent>();
            if(json!=null)
            {
                resourceObjectParent.SetInfo(json.resourceObjectsState[i].state, json.resourceObjectsState[i].remainingTime);
            }
            _resourceObjects.Add(resourceObjectParent);
        }
    }

    public GameObject SpawnObject(GameObject prefab, Vector3 pos)
    {
        var go = UnityEngine.Object.Instantiate(prefab, pos, Quaternion.identity);
        go.transform.parent = _resourceObjectRoot;
        return go;
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
