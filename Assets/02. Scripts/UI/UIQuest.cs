using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 2024. 01. 29 Byun Jeongmin
public class UIQuest : UIBase
{
    enum Texts
    {
        QuestName,
    }

    private TextMeshProUGUI QuestUIName => Get<TextMeshProUGUI>((int)Texts.QuestName);

    public override void Initialize()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    private void Awake()
    {
        Initialize();
    }

    //public virtual void Set(Quest quest)
    //{
    //    Get<TextMeshProUGUI>((int)Texts.QuestName).text = quest.questUIName.ToString();
    //}
}
