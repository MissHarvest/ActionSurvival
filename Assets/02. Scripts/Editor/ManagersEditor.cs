// 작성 날짜 : 2024. 01. 11
// 작성자 : Park Jun Uk

using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Managers))]
public class ManagersEditor : Editor
{
    public override void OnInspectorGUI() 
    { 
        base.OnInspectorGUI();

        GUILayout.Label("DayCycle");
        if (GUILayout.Button("Skip 10sec")) 
            Managers.Game.DayCycle.SkipTime();
        if (GUILayout.Button("Skip To front Morning."))
            Managers.Game.DayCycle.PassToMorning();
        if (GUILayout.Button("Skip To front Evening."))
            Managers.Game.DayCycle.PassToEvening();
        if (GUILayout.Button("Skip To front Night."))
            Managers.Game.DayCycle.PassToNight();

        GUILayout.Label("");
        GUILayout.Label("Temperature Manager");
        if (GUILayout.Button("Fire Island Influence += 0.1f"))
            Managers.Game.FireIsland.Influence += 0.1f;
        if (GUILayout.Button("Fire Island Influence -= 0.1f"))
            Managers.Game.FireIsland.Influence -= 0.1f;
        if (GUILayout.Button("Ice Island Influence += 0.1f"))
            Managers.Game.IceIsland.Influence += 0.1f;
        if (GUILayout.Button("Ice Island Influence -= 0.1f"))
            Managers.Game.IceIsland.Influence -= 0.1f;
        if (GUILayout.Button("Fire Island Temperature += 0.1f"))
            Managers.Game.FireIsland.Temperature += 0.1f;
        if (GUILayout.Button("Fire Island Temperature -= 0.1f"))
            Managers.Game.FireIsland.Temperature -= 0.1f;
        if (GUILayout.Button("Ice Island Temperature += 0.1f"))
            Managers.Game.IceIsland.Temperature += 0.1f;
        if (GUILayout.Button("Ice Island Temperature -= 0.1f"))
            Managers.Game.IceIsland.Temperature -= 0.1f;
    }
}
