using System.Collections.Generic;
using Unity.VisualScripting;
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
        {
            string key = e.name.Split('(')[0];
            key = key.TrimEnd(' ');
            list.Add(new() { _object = data.Dict[key], spawnPosition = e.transform.position });
        }
    }

    private void LoadObjects()
    {
        ClearObjects();
        var data = target as ResourceObjectSpawnData;
        var root = GetRoot();
        foreach (var e in data.SpawnList)
        {
            var obj = Instantiate(e.Prefab, e.spawnPosition, Quaternion.identity);
            obj.name = e.Prefab.name;
            obj.transform.parent = root;
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    private void ClearObjects()
    {
        foreach (var e in FindAllResourceObjects())
            DestroyImmediate(e.gameObject, false);
        DestroyImmediate(GetRoot().gameObject, false);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    private Transform GetRoot()
    {
        var root = GameObject.Find("@ResourceObjectSpawnEditorRoot");
        if (root == null)
            root = new GameObject("@ResourceObjectSpawnEditorRoot");
        return root.transform;
    }

    public ResourceObjectParent[] FindAllResourceObjects()
    {
        return FindObjectsOfType<ResourceObjectParent>();
    }
}