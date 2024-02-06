using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IslandObjectGenerator))]
public class IslandObjectGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
        {
            var generator = (IslandObjectGenerator)target;
            generator.StartCoroutine(generator.Start());
        }
    }
}