using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 2024. 01. 29 Byun Jeongmin
public class UIQuest : UIBase
{
    enum Images
    {
        Icon,
    }

    enum Texts
    {
        QuestName,
    }

    [SerializeField] private QuestSO[] _quests;

    public override void Initialize()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        Get<Image>((int)Images.Icon).raycastTarget = false;
        Get<TextMeshProUGUI>((int)Texts.QuestName).raycastTarget = false;
    }

    private void Awake()
    {
        Initialize();
    }

    //private void OnEnable()
    //{
    //    _quests = Managers.Resource.GetCacheGroup<QuestSO>("QuestData");
    //}

    public void Set(Quest activeQuest)
    {
        Get<Image>((int)Images.Icon).sprite = activeQuest.questSO.requiredItems[0].item.iconSprite;
        Get<TextMeshProUGUI>((int)Texts.QuestName).text = activeQuest.questSO.questUIName;
    }
}
