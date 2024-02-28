using System.Collections.Generic;
using UnityEngine;

public class UsableMonsterSpawnPoints
{
    private List<Vector3[]> _usablePointGroup = new();
    private string _savePath = "UsableMonsterSpawnPoints";

    public UsableMonsterSpawnPoints()
    {
        Load();
    }

    public Vector3[] Get()
    {
        Debug.Log("[Get]");
        var rnd = Random.Range(0, 3);
        return _usablePointGroup[rnd];
    }

    private void Load()
    {
        if(SaveGame.TryLoadJsonFile(SaveGame.SaveType.Compile, _savePath, out SaveArray2 data2))
        {
            for (int i = 0; i < data2._serailList.Count; ++i)
            {
                _usablePointGroup.Add(data2._serailList[i]._serailList.ToArray());
            }
        }
    }
}
