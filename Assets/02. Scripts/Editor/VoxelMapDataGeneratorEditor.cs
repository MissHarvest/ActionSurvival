using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VoxelMapDataGenerator))]
public class VoxelMapDataGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Map Data"))
            (target as VoxelMapDataGenerator).Start();
    }
}