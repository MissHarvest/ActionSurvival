using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

public class ResourceManager
{
    private Dictionary<string, Object> _resources = new();

    public void LoadAsync<T>(string key, Action<T> callback = null) where T : Object
    {
        if (_resources.TryGetValue(key, out var resource))
        {
            callback?.Invoke(resource as T);
            return;
        }

        var operation = Addressables.LoadAssetAsync<T>(key);
        operation.Completed += op =>
        {
            _resources.TryAdd(key, op.Result);
            callback?.Invoke(op.Result);
        };
    }

    public void LoadAllAsync<T>(string label, Action<string, int, int> callback = null) where T : Object
    {
        var operation = Addressables.LoadResourceLocationsAsync(label, typeof(T));
        operation.Completed += op =>
        {
            int loadCount = 0;
            int totalCount = op.Result.Count;

            foreach (var result in op.Result)
            {
                LoadAsync<T>(result.PrimaryKey, obj =>
                {
                    loadCount++;
                    callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                });
            }
        };
    }

    public T GetCache<T>(string key) where T : Object
    {
        if (!_resources.TryGetValue(key, out var resource))
        {
            Debug.LogError($"[{nameof(ResourceManager)}] {key} is not loaded.");
            return null;
        }
        return resource as T;
    }

    public void Release(string key)
    {
        if (_resources.TryGetValue(key, out var resource))
        {
            Addressables.Release(resource);
            _resources.Remove(key);
        }
    }

    public T[] GetCacheGroup<T>(string containKeyword) where T : Object
    {
        List<T> result = new();
        foreach (var element in _resources)
            if (element.Key.Contains(containKeyword))
                result.Add(element.Value as T);
        return result.ToArray();
    }


    #region legacy code
    //#region Resources Mebmer

    //// [ 리소스 형태 ]
    //// 1. string : Key => File Path
    //// 2. value : Prefab Name, Game Object
    //private readonly Dictionary<string, Dictionary<string, GameObject>> _resourcesByFolder =
    //    new Dictionary<string, Dictionary<string, GameObject>>();

    //private readonly Dictionary<string, ScriptableObject> _scriptableResources =
    //    new Dictionary<string, ScriptableObject>();

    //private readonly Dictionary<string, AudioClip> _audioResources =
    //    new Dictionary<string, AudioClip>();

    //public bool IsLoaded { get; private set; }

    //#endregion



    //#region Load Resources

    //public void LoadAllPrefabs()
    //{
    //    LoadPrefabsByFolder();
    //    LoadPrefabsByFolder(Literals.PATH_INIT);        
    //    LoadPrefabsByFolder(Literals.PATH_UI);
    //    LoadPrefabsByFolder(Literals.PATH_POPUPUI);
    //    LoadPrefabsByFolder(Literals.PATH_SCENEUI);
    //    LoadPrefabsByFolder(Literals.PATH_RESOURCEMODEL);
    //    LoadPrefabsByFolder(Literals.PATH_HANDABLE);
    //    LoadPrefabsByFolder(Literals.PATH_ITEM);
    //    LoadPrefabsByFolder(Literals.PATH_PLAYER);
    //    LoadPrefabsByFolder(Literals.PATH_STRUCTURE);
    //    LoadScriptableByFolder();
    //    LoadAuidoClipByFolder();
    //    IsLoaded = true;
    //}

    //private void LoadPrefabsByFolder(string folderPath = null)
    //{
    //    var folderKey = folderPath ?? "Prefabs";
    //    var prefabs = Resources.LoadAll<GameObject>(folderKey);

    //    if (!_resourcesByFolder.ContainsKey(folderKey))
    //    {
    //        _resourcesByFolder[folderKey] = new Dictionary<string, GameObject>();
    //    }

    //    foreach (var prefab in prefabs)
    //    {
    //        if(!_resourcesByFolder[folderKey].ContainsKey(prefab.name))
    //        {
    //            _resourcesByFolder[folderKey].Add(prefab.name, prefab);
    //        }
    //        else
    //        {
    //            Debug.LogWarning($"Prefab with name {prefab.name} already exists in the dict for folder {folderKey}");
    //        }
    //    }
    //}

    //private void LoadScriptableByFolder(string folderPath = null)
    //{
    //    var folderKey = folderPath ?? "ScriptableObjects";
    //    var scriptableObjects = Resources.LoadAll<ScriptableObject>(folderKey);

    //    foreach (var scriptableObject in scriptableObjects)
    //    {
    //        if (!_scriptableResources.TryAdd(scriptableObject.name, scriptableObject))
    //        {
    //            Debug.LogWarning($"ScriptableObject with name {scriptableObject.name} already exists in the dictionary for folder {folderKey}");
    //        }
    //    }
    //}

    //private void LoadAuidoClipByFolder(string folderPath = null)
    //{
    //    var folderKey = folderPath ?? "Sounds";
    //    var audioClips = Resources.LoadAll<AudioClip>(folderKey);

    //    foreach(var audioClip in audioClips)
    //    {
    //        if(!_audioResources.TryAdd(audioClip.name, audioClip))
    //        {
    //            Debug.LogWarning($"AudioClip with name {audioClip.name} already exists in the dictionary for folder {folderKey}");
    //        }
    //    }
    //}

    //#endregion



    //#region Get Resources

    //public GameObject GetPrefab(string prefabName, string folderPath = "Prefabs")
    //{
    //    if (_resourcesByFolder.TryGetValue(folderPath, out var folderDict) &&
    //        folderDict.TryGetValue(prefabName, out var prefab))
    //    {
    //        return prefab;
    //    }

    //    Debug.LogWarning($"Prefab with name {prefabName} not found in the folder {folderPath}.");
    //    return null;
    //}

    //public GameObject[] GetPrefabs(string folderPath)
    //{
    //    if (!string.IsNullOrEmpty(folderPath) && _resourcesByFolder.TryGetValue(folderPath, out var folderDict))
    //    {
    //        return new List<GameObject>(folderDict.Values).ToArray();
    //    }

    //    Debug.LogWarning($"Folder path is invalid or not loaded: {folderPath}");
    //    return Array.Empty<GameObject>();
    //}

    //public ScriptableObject GetScriptableObject(string scriptableName)
    //{
    //    if (_scriptableResources.TryGetValue(scriptableName, out var scriptableObject))
    //    {
    //        return scriptableObject;
    //    }

    //    Debug.LogWarning($"ScriptableObject with name {scriptableName} not found.");
    //    return null;
    //}

    //public ScriptableObject[] GetScriptableObjects()
    //{
    //    return new List<ScriptableObject>(_scriptableResources.Values).ToArray();
    //}

    //public AudioClip GetAudioClip(string audioClipName)
    //{
    //    if(_audioResources.TryGetValue(audioClipName, out var audioClip))
    //    {
    //        return audioClip;
    //    }

    //    Debug.LogWarning($"AudioClip with name {audioClipName} not found.");
    //    return null;
    //}

    //#endregion



    //#region Utils

    //public GameObject Instantiate(string prefabName, string folderPath = "Prefabs", Transform parent = null)
    //{
    //    var prefab = GetPrefab(prefabName, folderPath);

    //    if (prefab != null) return Object.Instantiate(prefab, parent);

    //    Debug.LogWarning($"Failed to load prefab: {prefabName}");
    //    return null;
    //}

    //public static void Destroy(GameObject gameObject)
    //{
    //    if (gameObject == null) return;

    //    Object.Destroy(gameObject);
    //}

    //#endregion
    #endregion
}