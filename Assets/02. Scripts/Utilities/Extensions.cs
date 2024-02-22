using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static string DictionaryToJson<TKey, TValue>(this Dictionary<TKey, TValue> dict)
    {
        var serializableDict = new SerializableDictionary<TKey, TValue>(dict);
        return JsonUtility.ToJson(serializableDict);
    }

    public static string DictionaryToJson<TKey, TValue>(this Dictionary<TKey, TValue> dict, bool pretty)
    {
        var serializableDict = new SerializableDictionary<TKey, TValue>(dict);
        return JsonUtility.ToJson(serializableDict, pretty);
    }

    public static Dictionary<TKey, TValue> DictionaryFromJson<TKey, TValue>(this string jsonStr)
    {
        var serializableDict = JsonUtility.FromJson<SerializableDictionary<TKey, TValue>>(jsonStr);
        return serializableDict.ToDictionary();
    }

    public static bool GetCurrentChunkActive(this Component obj)
    {
        if (!GameManager.Instance.IsRunning)
            return false;
        var cc = GameManager.Instance.World.ConvertChunkCoord(obj.transform.position);
        GameManager.Instance.World.ChunkMap.TryGetValue(cc, out var chunk);
        return chunk.IsActive;
    }

    public static bool GetCurrentChunkActive(this GameObject obj)
    {
        if (!GameManager.Instance.IsRunning)
            return false;
        var cc = GameManager.Instance.World.ConvertChunkCoord(obj.transform.position);
        GameManager.Instance.World.ChunkMap.TryGetValue(cc, out var chunk);
        return chunk.IsActive;
    }
}