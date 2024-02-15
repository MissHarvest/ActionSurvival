using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactCreator
{
    [System.Serializable]
    public struct ArtifactSaveData
    {
        public Vector3 pos;
        public bool isActive;
        public string islandName;
    }
    public struct ArtifactSaveDataList
    {
        public List<ArtifactSaveData> list;
    }
    public ArtifactSaveDataList saveData;

    private Transform _root;
    private GameManager _manager;
    public HashSet<Artifact> Artifacts { get; private set; }

    public ArtifactCreator(GameManager manager)
    {
        _root = new GameObject("Artifact Root").transform;
        _manager = manager;
        _manager.DayCycle.OnEveningCame += TryCreate;
        Artifacts = new();
        manager.OnSaveCallback += Save;
        Load();
    }

    public void TryCreate()
    {
        if (_manager.Season.IsIceIslandActive)
            CoroutineManagement.Instance.StartCoroutine(TryCreate(_manager.IceIsland));
        else if (_manager.Season.IsFireIslandActive)
            CoroutineManagement.Instance.StartCoroutine(TryCreate(_manager.FireIsland));
    }

    private IEnumerator TryCreate(Island island)
    {
        Vector3 pos;
        var property = island.Property;
        int minX = (int)property.center.x - property.diameter / 2;
        int maxX = (int)property.center.x + property.diameter / 2;
        int minZ = (int)property.center.z - property.diameter / 2;
        int maxZ = (int)property.center.z + property.diameter / 2;

        while (true)
        {
            int x = Random.Range(minX, maxX);
            int z = Random.Range(minZ, maxZ);

            pos = new Vector3(x, z);
            if (IsValidPosition(ref pos))
                break;

            yield return null;
        }

        Create(pos, island);
    }

    public bool IsValidPosition(ref Vector3 pos)
    {
        pos += Vector3.up * 50f;
        if (Physics.Raycast(pos, Vector3.down, out var hit, 100f, int.MaxValue, QueryTriggerInteraction.Collide))
        {
            pos = hit.point;
            return hit.collider.gameObject.layer == 12;
        }
        else
            return false;
    }

    public void Create(Vector3 spawnPosition, Island island)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<Artifact>();
        obj.SetInfo(island, false, spawnPosition, _root);
        obj.SetActive(true);
        Artifacts.Add(obj);
    }

    public void Create(ArtifactSaveData loadData)
    {
        Island island;
        if (loadData.islandName == "IceIsland")
            island = _manager.IceIsland;
        else if (loadData.islandName == "FireIsland")
            island = _manager.FireIsland;
        else
            return;

        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<Artifact>();
        obj.SetInfo(island, loadData.isActive, loadData.pos, _root);
        obj.SetState();
        obj.SetLayer();
        Artifacts.Add(obj);
    }

    public void Save()
    {
        saveData.list = new();
        foreach (var artifact in Artifacts)
        {
            ArtifactSaveData data = new()
            {
                pos = artifact.OriginPos,
                isActive = artifact.IsActive,
                islandName = artifact.IslandName,
            };
            saveData.list.Add(data);
        }

        string json = JsonUtility.ToJson(saveData);
        SaveGame.CreateJsonFile("Artifacts", json, SaveGame.SaveType.Runtime);
        saveData.list = null;
    }

    public void Load()
    {
        SaveGame.TryLoadJsonFile(SaveGame.SaveType.Runtime, "Artifacts", out saveData);
        foreach (var data in saveData.list)
            Create(data);
    }
}