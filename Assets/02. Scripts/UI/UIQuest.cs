using System;
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

    public void Set(Quest activeQuest, Tutorial tutorial)
    {
        Get<Image>((int)Images.Icon).sprite = activeQuest.questSO.requiredItems[0].item.iconSprite;
        Get<TextMeshProUGUI>((int)Texts.QuestName).text = activeQuest.questSO.questUIName;

        var handler = gameObject.GetOrAddComponent<UIEventHandler>();
        handler.OnClickEvent = null; // Set 이 두번되서

        gameObject.BindEvent((x) =>
        {
            SetFunction(activeQuest, tutorial);
        });
    }

    private void SetFunction(Quest activeQuest,Tutorial tutorial)
    {
        switch (activeQuest.questSO.type)
        {
            case QuestSO.QuestType.Finding:
                if (activeQuest.questSO.targetName == string.Empty) return;
                tutorial.PathFinding(activeQuest.questSO.targetLayer, activeQuest.questSO.targetName);
                break;

            case QuestSO.QuestType.Crafting:
                Debug.Log("Call using Crafting");
                tutorial.GuideCraft();
                break;

            case QuestSO.QuestType.Using:
                Debug.Log("Call using tutorial");// 여기에 추가적으로 콜백 넣어서, 퀘스트 클리어도..
                tutorial.StartInvnetoryGuide(activeQuest.questSO.requiredItems[0].item);
                break;
        }
    }
}
