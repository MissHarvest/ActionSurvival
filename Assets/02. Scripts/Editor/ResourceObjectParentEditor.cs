using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResourceObjectParent))]
public class ResourceObjectParentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ResourceObjectParent parent = (ResourceObjectParent)target;
        if (GUILayout.Button("Gather"))
            parent.TestInteract();
        if (GUILayout.Button("Respawn"))
            parent.TestRespawn();
    }
}