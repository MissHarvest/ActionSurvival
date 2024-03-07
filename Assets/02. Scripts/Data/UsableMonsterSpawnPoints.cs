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
        var data = Managers.Resource.GetCache<TextAsset>("UsableMonsterSpawnPoints.data");
        if (data == null) return;
        var data2 = JsonUtility.FromJson<SaveArray2>(data.text);

        for (int i = 0; i < data2._serailList.Count; ++i)
        {
            _usablePointGroup.Add(data2._serailList[i]._serailList.ToArray());
        }
    }
}
