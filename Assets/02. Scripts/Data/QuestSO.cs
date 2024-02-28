using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 29 Byun Jeongmin
[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest/New Quest")]
public class QuestSO : ScriptableObject
{
    public enum QuestType
    {
        Finding,
        Crafting,
        Using,
    }

    public string questUIName;
    public bool canBeAddedWithoutPreQuests;
    public List<QuestSO> preQuests; // 선행 퀘스트 리스트
    public List<RequiredItem> requiredItems = new List<RequiredItem>();

    [System.Serializable]
    public class RequiredItem
    {
        public ItemData item;
        public int quantity;
    }

    [Header("Tutorial")]
    public LayerMask targetLayer;
    public string targetName;
    public QuestType type;
}
