// 작성 날짜 : 2024. 01. 11
// 작성자 : Park Jun Uk

using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Managers))]
public class SkipTimeButton : Editor
{
    public override void OnInspectorGUI() 
    { 
        base.OnInspectorGUI();

        if (GUILayout.Button("Skip 10sec")) 
        {
            Managers.Game.DayCycle.SkipTime();
        }

        if (GUILayout.Button("Skip To front Morning."))
        {
            Managers.Game.DayCycle.PassToMorning();
        }

        if (GUILayout.Button("Skip To front Evening."))
        {
            Managers.Game.DayCycle.PassToEvening();
        }

        if (GUILayout.Button("Skip To front Night."))
        {
            Managers.Game.DayCycle.PassToNight();
        }
    }    
}
