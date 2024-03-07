using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024-02-14 WJY
public class ArtifactCreator
{
    public struct ArtifactSaveData
    {
        [System.Serializable]
        public struct Data
        {
            public Vector3 pos;
            public string islandName;
            public float remainHP;
        }

        public List<Data> list;
    }
    public ArtifactSaveData saveData;

    private Transform _root;
    private ArtifactData _data;
    public HashSet<Artifact> Artifacts { get; private set; }

    public void Init()
    {
        _data = Managers.Resource.GetCache<ArtifactData>("ArtifactData.data");
        _root = new GameObject("Artifact Root").transform;        
        Artifacts = new();
    }

    public void BindEvent()
    {
        GameManager.DayCycle.OnStarted += OnDayStarted;
        GameManager.DayCycle.OnEveningCame += TryCreate;
        GameManager.Instance.OnSaveCallback += Save;
    }

    private void OnDayStarted(DayInfo info)
    {
        Load();
    }

    public void TryCreate()
    {
        if (GameManager.Season.IsIceIslandActive)
        {
            _data.Artifact.SetSharedMesh(_data.Model[1]);
            _data.Artifact.SetDropTable(_data.LootingData[1]);
            for (int i = 0; i < _data.CreateCount; i++)
                CoroutineManagement.Instance.StartCoroutine(TryCreate(GameManager.Instance.IceIsland));
        }
        else if (GameManager.Season.IsFireIslandActive)
        {
            _data.Artifact.SetSharedMesh(_data.Model[0]);
            _data.Artifact.SetDropTable(_data.LootingData[0]);
            for (int i = 0; i < _data.CreateCount; i++)
                CoroutineManagement.Instance.StartCoroutine(TryCreate(GameManager.Instance.FireIsland));
        }
    }

    private IEnumerator TryCreate(Island island)
    {
        Vector3 pos;
        var property = island.Property;
        int minX = (int)property.Center.x - property.Diameter / 2;
        int maxX = (int)property.Center.x + property.Diameter / 2;
        int minZ = (int)property.Center.z - property.Diameter / 2;
        int maxZ = (int)property.Center.z + property.Diameter / 2;

        while (true)
        {
            int x = Random.Range(minX, maxX);
            int z = Random.Range(minZ, maxZ);

            pos = new Vector3(x, 0, z);
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
        var obj = UnityEngine.Object.Instantiate(_data.Prefab).GetComponent<Artifact>();
        obj.SetInfo(island, spawnPosition, _root, _data);
        obj.RaiseInfluence();
        obj.SpawnOverflowedMonster();
        Artifacts.Add(obj);
        obj.OnDestroy += x => Artifacts.Remove(x);
    }

    public void Create(ArtifactSaveData.Data loadData)
    {
        Island island;
        if (loadData.islandName == GameManager.Instance.IceIsland.Name)
        {
            _data.Artifact.SetSharedMesh(_data.Model[1]);
            _data.Artifact.SetDropTable(_data.LootingData[1]);
            island = GameManager.Instance.IceIsland;
        }
        else if (loadData.islandName == GameManager.Instance.FireIsland.Name)
        {
            _data.Artifact.SetSharedMesh(_data.Model[0]);
            _data.Artifact.SetDropTable(_data.LootingData[0]);
            island = GameManager.Instance.FireIsland;
        }
        else
            return;

        var obj = UnityEngine.Object.Instantiate(_data.Prefab).GetComponent<Artifact>();
        obj.SetInfo(island, loadData.pos, _root, _data, loadData.remainHP);
        Artifacts.Add(obj);
        obj.OnDestroy += x => Artifacts.Remove(x);
    }

    public void Save()
    {
        saveData.list = new();
        foreach (var artifact in Artifacts)
        {
            ArtifactSaveData.Data data = new()
            {
                pos = artifact.transform.position,
                islandName = artifact.IslandName,
                remainHP = artifact.RemainingHP,
            };
            saveData.list.Add(data);
        }

        string json = JsonUtility.ToJson(saveData);
        SaveGame.CreateJsonFile("Artifacts", json, SaveGame.SaveType.Runtime);
        saveData.list = null;
    }

    public void Load()
    {
        if (SaveGame.TryLoadJsonFile(SaveGame.SaveType.Runtime, "Artifacts", out saveData))
        {
            foreach (var data in saveData.list)
                Create(data);
            saveData.list = null;
        }
    }
}