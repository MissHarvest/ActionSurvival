using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 20 Park Jun Uk
public class MonsterWave
{
    public Stack<GameObject> waveMonsters = new Stack<GameObject>();
    public Stack<GameObject> overflowMonsters = new Stack<GameObject>();
    private int _minDistance = 50;
    private int _maxDistance = 60;
    public Stack<Vector3> wavePoints = new Stack<Vector3>();
    private MonsterGroup _defaultMonsters = new MonsterGroup(); // GameManager 가 가지고 있는 일반섬 목록.
    
    public MonsterWave()
    {
        _defaultMonsters.AddMonsterType(new string[] { "Skeleton", "Bat" });
    }

    public void AddOverFlowedMonster(GameObject monster)
    {
        overflowMonsters.Push(monster);
    }

    public void Start()
    {
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
            var monster = waveMonsters.Pop();
            monster.transform.position = point;
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
            if (Physics.BoxCast(playerPos + direction, Vector3.one * 0.5f, Vector3.down, out hit, Quaternion.identity, 70.0f, 1))
            {
                wavePoints.Push(hit.point);
            }
        }
    }
}
