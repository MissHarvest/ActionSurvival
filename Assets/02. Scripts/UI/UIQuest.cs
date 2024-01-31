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

    private List<QuestSO> _quests;

    public override void Initialize()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        _quests = Managers.Game.Player.Tutorial.Quests;
    }

    public void SetText(int index)
    {
        Get<TextMeshProUGUI>((int)Texts.QuestName).text = _quests[index].questUIName;
    }
}
