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
        handler.OnClickEvent = null; // Set �� �ι��Ǽ�

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
                tutorial.PathFinding(activeQuest);
                break;

            case QuestSO.QuestType.Crafting:
                Debug.Log("Call using Crafting");
                tutorial.GuideCraft();
                break;

            case QuestSO.QuestType.Using:
                Debug.Log("Call using tutorial");// ���⿡ �߰������� �ݹ� �־, ����Ʈ Ŭ���..
                tutorial.StartInvnetoryGuide(activeQuest.questSO.requiredItems[0].item);
                break;
        }
    }
}
