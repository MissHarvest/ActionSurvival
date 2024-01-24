using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ResourceObjectSpawnData), menuName = "WorldData/ResourceObjectSpawnData", order = 3)]
public class ResourceObjectSpawnData : ScriptableObject
{
    [SerializeField] private List<ResourceObjectParent> _resourceObjectList = new();
    private Dictionary<string, ResourceObjectParent> _dict = new();

    private bool _isInitialized = false;

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
        if (_isInitialized) return;
        foreach (var item in _resourceObjectList)
        {
            _dict.TryAdd(item.name, item);
            Debug.Log(item.name);
        }
        _isInitialized = true;
    }

    public ResourceObjectParent this[string key]
    {
        get => _dict[key];
    }
}