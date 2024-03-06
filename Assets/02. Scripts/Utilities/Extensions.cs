using System;
using System.Collections.Generic;
using Unity.Jobs;
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

    public static Vector3[] CalcaultateSmoothNormal(this Mesh mesh)
    {
        Dictionary<Vector3, List<int>> indicesDict = new(comparer: new Vector3EqualityComparer());

        // 프로퍼티로 접근하면 접근할때마다 복사하기 때문에 로컬변수로 빼놔야함
        var originVertices = mesh.vertices;
        var originNormals = mesh.normals;

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            if (!indicesDict.ContainsKey(originVertices[i]))
                indicesDict.Add(originVertices[i], new());

            indicesDict[originVertices[i]].Add(i);
        }

        Vector3[] normals = new Vector3[originNormals.Length];

        foreach (var indices in indicesDict.Values)
        {
            Vector3 normal = Vector3.zero;

            foreach (var index in indices)
                normal += originNormals[index];

            normal /= indices.Count;

            foreach (var index in indices)
                normals[index] = normal;
        }

        return normals;
    }
}