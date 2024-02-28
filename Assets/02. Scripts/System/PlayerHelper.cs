using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class TipData
{
    public int id;
    public string displayTitle;
    public Sprite sprite;
    [TextArea]
    public string summary;

    [TextArea]
    public string information;    
    public bool check;

    public TipData(TipData tipData)
    {
        this.id = tipData.id;
        this.summary = tipData.summary;
        this.check = tipData.check;
        this.information = tipData.information;
    }
}

public class PlayerHelper
{
    private string _savePath = "Tips";
    private Dictionary<int, TipData> _tipDic = new();
    private Dictionary<(int, int), Tip> _datePairKeyword = new();
    private static int Morning = 0;
    private static int Evening = 37;
    private static int Night = 49;

    public void Init()
    {
        _datePairKeyword.TryAdd((1, 1), Tip.Tutorial);
        _datePairKeyword.TryAdd((1, Evening), Tip.MonsterWave);
        _datePairKeyword.TryAdd((4, Morning), Tip.Summer);
        _datePairKeyword.TryAdd((4, Evening), Tip.LavaArtifact);
        // 여름 끝나는 것
        // 겨울 끝나는 것
        // 겨울 시작하는 것
    }

    public void BindEvent()
    {        
        GameManager.DayCycle.OnUpdated += OnTimeUpdated;
        GameManager.Instance.Player.ConditionHandler.Hunger.OnBelowedToZero += OnHungerZero;
        GameManager.Instance.Player.ConditionHandler.Hunger.OnRecovered += OnHungerRecovered;
        GameManager.Instance.OnSaveCallback += Save;

        Load();
    }

    private void OnTimeUpdated(int date, int time)
    {
        if(_datePairKeyword.TryGetValue((date, time), out Tip tip))
        {
            ShowTip(tip);
        }
    }

    private void OnHungerZero()
    {
        ShowTip(Tip.Hunger);
    }

    private void OnHungerRecovered(float percent)
    {
        if(percent >= 0.7f)
            ShowTip(Tip.Fullness);
    }

    private void ShowTip(Tip tip)
    {
        Debug.Log($"[Tip] {tip.ToString()}");
        if(_tipDic.TryGetValue((int)tip, out TipData tipData))
        {
            if (tipData.check) return;
            tipData.check = true;
            Managers.UI.ShowPopupUI<UITip>().Set(tipData, ()=> { ShowHelper(tipData.id); });
        }        
    }

    private void ShowHelper(int index)
    {
        Managers.UI.ShowPopupUI<UITipInformation>(pause:true).ShowInformation(index);
    }

    private void Load()
    {
        var so = Managers.Resource.GetCache<TipsSO>("Tips.data");
        foreach (var tip in so.tips)
        {
            TipData tipData = new(tip);
            if(_tipDic.TryAdd(tip.id, tipData) == false)
            {
                Debug.LogWarning($"Tip ID[{tip.id}] is Exist");
            }
        }

        if(SaveGame.TryLoadJsonFile(SaveGame.SaveType.Runtime, _savePath, out SerializableList<bool> list))
        {
            for(int i = 0; i < list.Count; ++i)
            {
                if(_tipDic.TryGetValue(i, out TipData tipData))
                {
                    tipData.check = list.Get(i);
                }
            }
        }
    }

    private void Save()
    {
        SerializableList<bool> list = new();
        List<(int, bool)> temp = new();

        foreach(var tip in _tipDic)
        {
            temp.Add((tip.Key, tip.Value.check));
        }

        list.Set(temp.OrderBy(x => x.Item1).Select(x => x.Item2).ToList());

        var json = JsonUtility.ToJson(list);
        SaveGame.CreateJsonFile(_savePath, json, SaveGame.SaveType.Runtime);
    }
}
