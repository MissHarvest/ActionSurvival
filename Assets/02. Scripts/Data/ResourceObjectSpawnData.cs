using System.Collections.Generic;
using UnityEngine;

// 2024-01-23 WJY
[CreateAssetMenu(fileName = nameof(ResourceObjectSpawnData), menuName = "WorldData/ResourceObjectSpawnData", order = 3)]
public class ResourceObjectSpawnData : ScriptableObject
{
    [SerializeField] private List<ResourceObjectParent> _resourceObjectList = new();
    private Dictionary<string, ResourceObjectParent> _dict = new();

    public Dictionary<string, ResourceObjectParent> Dict => _dict;

    [System.Serializable]
    public struct DataTuple
    {
        public ResourceObjectParent _object;
        public Vector3 spawnPosition;

        public readonly GameObject Prefab => _object.gameObject;
    }

    [SerializeField] private List<DataTuple> _spawnList;
    public List<DataTuple> SpawnList => _spawnList;

    public void Initialize()
    {
        foreach (var item in _resourceObjectList)
            _dict.TryAdd(item.name, item);
    }
}