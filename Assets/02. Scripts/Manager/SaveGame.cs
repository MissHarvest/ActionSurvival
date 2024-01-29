using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
//using UnityEditor.ShaderGraph.Serialization; lgs 24.01.29
using UnityEngine;

public class SaveGame
{
    public enum SaveType
    {
        Compile,
        Runtime,
    }

    public static void CreateJsonFile(string name, string json, SaveType type)
    {
        string path = "";
        switch(type)
        {
            case SaveType.Compile:
                path = $"{Application.dataPath}/Resources";
                break;

            case SaveType.Runtime:
                path = $"{Application.persistentDataPath}";                
                break;
        }

        CreateJsonFile(path, name, json);
    }

    private static void CreateJsonFile(string path, string name, string json)
    {
        FileStream fs = new FileStream($"{path}/{name}.json", FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(json);
        fs.Write(data, 0, data.Length);
        fs.Close();
    }

    public static void SaveToJson(string name, string json)
    {
        FileStream fs = new FileStream($"{Application.persistentDataPath}/{name}.json", FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(json);
        fs.Write(data, 0, data.Length);
        fs.Close();
    }

    public static bool TryLoadJsonFile<T>(SaveType type, string name, out T json)
    {
        string path = "";
        switch (type)
        {
            case SaveType.Compile:
                path = $"{Application.dataPath}/Resources";
                break;

            case SaveType.Runtime:
                path = $"{Application.persistentDataPath}";
                break;
        }

        if (File.Exists($"{path}/{name}.json") == false)
        {
            json = default(T);
            return false;
        }
        json = LoadJsonFile<T>(path, name);
        return true;
    }

    private static T LoadJsonFile<T>(string path, string name)
    {
        FileStream fs = new FileStream($"{path}/{name}.json", FileMode.Open);
        byte[] data = new byte[fs.Length];
        fs.Read(data, 0, data.Length);
        string json = Encoding.UTF8.GetString(data);
        fs.Close();
        return JsonUtility.FromJson<T>(json);
    }

    public static bool TryLoadJsonToObject(object obj, SaveType type, string name)
    {
        var path = GetPathByType(type);
        
        if (File.Exists($"{path}{name}.json") == false) return false;

        LoadJsonToObject(path, name, obj);
        return true;
    }

    private static string GetPathByType(SaveType type)
    {
        string path = "";
        switch (type)
        {
            case SaveType.Compile:
                path = $"{Application.dataPath}/Resources/";
                break;

            case SaveType.Runtime:
                path = $"{Application.persistentDataPath}/";
                break;
        }
        return path;
    }

    public static void LoadJsonToObject(string path, string name, object obj)
    {
        FileStream fs = new FileStream($"{path}/{name}.json", FileMode.Open);
        byte[] data = new byte[fs.Length];
        fs.Read(data, 0, data.Length);
        string json = Encoding.UTF8.GetString(data);
        JsonUtility.FromJsonOverwrite(json, obj);
        fs.Close();
    }
}
