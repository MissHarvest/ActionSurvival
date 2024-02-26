using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonsterSpawner))]
public class MonsterSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MonsterSpawner spawner = (MonsterSpawner)target;
        if (GUILayout.Button("Spawn Monster"))
        {
            spawner.Spawn();
        }

        if (GUILayout.Button("Destroy Monster"))
        {
            spawner.DestroyMonster();
        }
    }
}
