using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct SaveMonster
{
    public SaveMonster(string name, Vector3 position)
    {
        this.name = name;
        this.position = position;
    }

    public string name;
    public Vector3 position;
}

[System.Serializable]
public class MonsterContainer
{
    [SerializeField] public List<SaveMonster> monsters = new();
}

// 2024. 01. 20 Park Jun Uk
public class MonsterWave
{
    public Stack<GameObject> waveMonsters = new Stack<GameObject>();
    public Stack<GameObject> overflowMonsters = new Stack<GameObject>();
    private int _minDistance = 30;
    private int _maxDistance = 37;
    public Stack<Vector3> wavePoints = new Stack<Vector3>();
    private MonsterGroup _defaultMonsters = new MonsterGroup(); // GameManager 가 가지고 있는 일반섬 목록.
    public List<Monster> monsters = new();

    public MonsterWave()
    {
        _defaultMonsters.AddMonsterType(new string[] { "Skeleton", "Bat" });
        Load();

        Managers.Game.OnSaveCallback += Save;
    }

    public void AddOverFlowedMonster(GameObject monster)
    {
        overflowMonsters.Push(monster);
    }

    public void Start()
    {
        monsters = monsters.Where(x => x != null).ToList();

        var maxCount = CalculateMonsterCountForWave();
        Debug.Log($"[ Monster Wave ] {maxCount}");
        
        var number = CalculateNumberOfOverFlowMonsterToUse();
        
        for(int i = 0; i < number; ++i)
        {
            if (overflowMonsters.Count == 0) break;
            var monster = overflowMonsters.Pop();
            if(monster)
            {
                waveMonsters.Push(monster);
                --maxCount;
            }
        }
        
        for(int i = 0; i < maxCount; ++i)
        {
            var monster = Object.Instantiate(_defaultMonsters.GetRandomMonster());
            monster.name += "[Wave]";
            waveMonsters.Push(monster);
        }
        
        CalcalateWavePoint(waveMonsters.Count);
        
        while(waveMonsters.Count != 0)
        {
            var point = wavePoints.Pop();
            var monster = waveMonsters.Pop().GetComponent<Monster>();
            monster.NavMeshAgent.Warp(point);
            monster.SetBerserkMode();
            monster.SetIsland(null);
            monsters.Add(monster);
        }
    }

    private int CalculateMonsterCountForWave()
    {
        return UnityEngine.Random.Range(2, 5);
    }

    private int CalculateNumberOfOverFlowMonsterToUse()
    {
        var date = Managers.Game.DayCycle.Date;
        if (date % 8 == 7) return overflowMonsters.Count;
        if (date % 8 % 3 == 0) return 1;
        return 0;
    }

    private void CalcalateWavePoint(int count)
    {
        while(wavePoints.Count != count)
        {
            var direction = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
            direction.Normalize();
            direction *= Random.Range(_minDistance, _maxDistance);            
            direction.y = 50;
            var playerPos = Managers.Game.Player.transform.position;
            RaycastHit hit;
            if (Physics.BoxCast(playerPos + direction, Vector3.one * 0.5f, Vector3.down, out hit, Quaternion.identity, 70.0f, 1 << 12))
            {
                wavePoints.Push(hit.point);
            }
        }
    }

    private void Load()
    {
        MonsterContainer container = new();
        if (SaveGame.TryLoadJsonToObject(container, SaveGame.SaveType.Runtime, "MonsterWave"))
        {
            for(int i = 0; i < container.monsters.Count; ++i)
            {
                var path = $"{container.monsters[i].name}.prefab";
                var prefab = Managers.Resource.GetCache<GameObject>(path);
                var monster = Object.Instantiate(prefab, container.monsters[i].position, Quaternion.identity).GetComponent<Monster>();
                monster.SetBerserkMode();
                monsters.Add(monster);
            }
        }
    }

    private void Save()
    {
        MonsterContainer container = new();

        for(int i = 0; i < monsters.Count; ++i)
        {
            var name = monsters[i].Data.name;
            var pos = monsters[i].transform.position;

            container.monsters.Add(new SaveMonster(name, pos));
        }

        var json = JsonUtility.ToJson(container);
        SaveGame.CreateJsonFile("MonsterWave", json, SaveGame.SaveType.Runtime);
    }
}
