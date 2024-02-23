using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Ignition), true)]
public class FirewoodEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Ignition ignition = (Ignition)target;
        if (GUILayout.Button("Start a fire"))
        {
            ignition.StartCookingButton();
        }
        if (GUILayout.Button("Stop a fire"))
        {
            ignition.StopCookingButton();
        }
    }
}
