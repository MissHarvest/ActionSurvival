using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BossMonster), true)]
public class BossMonsterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BossMonster boss = (BossMonster)target;
        if (GUILayout.Button("Respawn"))
        {
            //boss.Respawn();
        }

        if (GUILayout.Button("Hit"))
        {
            boss.Hit(boss, 8.0f);
        }

        if (GUILayout.Button("Die"))
        {
            boss.Die();
        }
    }
}
