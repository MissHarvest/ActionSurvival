using System;

// 2024. 01. 29 Byun Jeongmin
[Serializable]
public class EatAppleQuest
{
    public EatAppleQuest() : base()
    {
    }

    //"EatAppleQuest", "사과 먹기", "사과 하나를 먹어 보상을 획득하세요! 힌트 : 사과는 사과나무에서 채집할 수 있습니다."

    //사과 먹음 -> 사과 획득 여부로 변경(인벤토리 - 획득 이벤트 이용)

    //public bool IsAppleEaten(PlayerInventorySystem inventorySystem)
    //{
    //    // 인벤토리에서 사과를 먹었는지 확인
    //    return inventorySystem.GetItemCount(Managers.Resource.GetCache<ItemData>("AppleItemData.data")) == 0;

    //}

    //퀘스트 클리어 보상 x
}
