using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// 2024. 01. 15 Park Jun Uk
[CustomEditor(typeof(Monster), true)]
public class MonsterDieButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Monster monster = (Monster)target;
        if (GUILayout.Button("Respawn"))
        {
            monster.Respawn();
        }

        if (GUILayout.Button("Hit"))
        {
            //monster.Hit(Managers.Game.Player, 1.0f);
        }

        if (GUILayout.Button("Die"))
        {
            monster.Die();
        }
    }
}
