using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public Player player;
    public GameObject monsterPrefab;
    public GameObject monster;
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.Player;
    }

    private void Update()
    {
        
    }

    private void DebugLog()
    {
        Debug.Log($"[DebugLog]{time}");
    }

    public void Spawn()
    {
        if (monsterPrefab == null) return;
        monster = Instantiate(monsterPrefab, transform.position, transform.rotation);
    }

    public void DestroyMonster()
    {
        Destroy(monster.gameObject);
    }
}
