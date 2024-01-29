using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ArchitectureManager
{
    public Dictionary<string, int> numbering = new();
    public Dictionary<string, List<GameObject>> architectures = new();

    public void Init()
    {
        if(SaveGame.TryLoadJsonFile<ArchitectureSaveData>(SaveGame.SaveType.Runtime, "Architectures", out ArchitectureSaveData json))
        {
            foreach(var v in json.architectureDicData)
            {
                numbering.Add(v.tag, v.num);
            }

            foreach (var data in json.transformData)
            {
                var prefabName = data.name.Split("-")[0];
                var prefab = Managers.Resource.GetCache<GameObject>($"{prefabName}.prefab");
                var go = UnityEngine.Object.Instantiate(prefab, data.position, data.rotation);
                go.name = data.name;
                                
                //go.GetComponent<BuildableObject>().DestroyColliderManager();
                                
                if(architectures.TryGetValue(prefabName, out List<GameObject> list))
                {
                    list.Add(go);
                }
                else
                {
                    architectures.Add(prefabName, new List<GameObject>());
                    architectures[prefabName].Add(go);
                }
            }

            if (architectures.ContainsKey("Architecture_Farm"))
            {
                for (int i = 0; i < architectures["Architecture_Farm"].Count; ++i)
                {
                    architectures["Architecture_Farm"][i].GetComponent<Farm>()?.SetInfo(json.farmData[i].state, json.farmData[i].remainingTime);
                }
            }
        }

        Managers.Game.OnSaveCallback += Save;
    }

    public void Add(BuildableObject architecture)
    {
        var name = architecture.name.Replace("(Clone)", "");
        
        if(architectures.TryGetValue(name, out List<GameObject> list))
        {
            list.Add(architecture.gameObject);            
            architecture.gameObject.name = $"{name}-{numbering[name]}";
            numbering[name] += 1;
            return;
        }

        Debug.Log($"[{name}] Dictionary Create");
        architectures.Add(name, new List<GameObject>());
        numbering.Add(name, 0);

        architectures[name].Add(architecture.gameObject);
        architecture.gameObject.name = $"{name}-{numbering[name]}";
        numbering[name] += 1;
    }

    public void Save()
    {
        ArchitectureSaveData saveData = new ArchitectureSaveData();
        foreach(var dic in numbering)
        {
            saveData.architectureDicData.Add(new ArchitectureKeyValue(dic.Key, dic.Value));
        }

        foreach(var dic in  architectures.Values)
        {
            foreach(var go in dic)
            {
                saveData.transformData.Add(new ArchitecutreTransform(go.name, go.transform.position, go.transform.rotation));
            }
        }

        if (architectures.ContainsKey("Architecture_Farm"))
        {
            foreach (var farm in architectures["Architecture_Farm"])
            {
                saveData.farmData.Add(new FarmData(farm.GetComponent<Farm>()));
            }
        }

        var json = JsonUtility.ToJson(saveData);
        SaveGame.CreateJsonFile("Architectures", json, SaveGame.SaveType.Runtime);
    }

    public void Remove(BuildableObject architecture)
    {
        var data = architecture.gameObject.name.Split("-");
        var name = data[0];
        var number = data[1];

        architectures[name].Remove(architecture.gameObject);
    }
}
