using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// 2024-01-25 WJY
[CustomEditor(typeof(ResourceObjectSpawnData))]
public class ResourceObjectSpawnDataEditor : Editor
{
    private readonly string _saveMessage = @"덮어쓸거임 ?";
    private readonly string _clearMessage = @"지금 씬에 올려놓은거 삭제됨 ㄱㅊ ?";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        if (GUILayout.Button("Save") && EditorUtility.DisplayDialog("ResourceObjectSpawnData", _saveMessage, "OK", "Cancel"))
        {
            SaveObjects();
        }

        if (GUILayout.Button("Load") && EditorUtility.DisplayDialog("ResourceObjectSpawnData", _clearMessage, "OK", "Cancel"))
        {
            LoadObjects();
        }

        if (GUILayout.Button("Clear Hierarchy") && EditorUtility.DisplayDialog("ResourceObjectSpawnData", _clearMessage, "OK", "Cancel"))
        {
            ClearObjects();
        }
    }

    private void SaveObjects()
    {
        var data = target as ResourceObjectSpawnData;
        data.Initialize();
        List<ResourceObjectSpawnData.DataTuple> list = data.SpawnList;
        list.Clear();
        foreach (var e in FindAllResourceObjects())
            list.Add(new() { _object = data.Dict[e.name], spawnPosition = e.transform.position });
    }

    private void LoadObjects()
    {
        ClearObjects();
        var data = target as ResourceObjectSpawnData;
        foreach (var e in data.SpawnList)
        {
            var obj = Instantiate(e.Prefab, e.spawnPosition, Quaternion.identity);
            obj.name = e.Prefab.name;
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    private void ClearObjects()
    {
        foreach (var e in FindAllResourceObjects())
            DestroyImmediate(e.gameObject, false);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    public ResourceObjectParent[] FindAllResourceObjects()
    {
        return FindObjectsOfType<ResourceObjectParent>();
    }
}