using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Animal))]
public class AnimalButtons : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Animal animal = (Animal)target;
        if (GUILayout.Button("Respawn"))
        {
            animal.Respawn();
        }

        if (GUILayout.Button("Hit"))
        {
            //animal.Hit(Managers.Game.Player, 1.0f);
        }

        if (GUILayout.Button("Die"))
        {
            animal.Die();
        }
    }
}
