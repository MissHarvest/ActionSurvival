using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 29 Byun Jeongmin
[Serializable]
public class CraftAxeQuest
{
    public CraftAxeQuest() : base()
    {
    }

    //"CraftAxe", "도끼 만들어 사용하기", "도끼를 만들고 사용하여 보상을 획득하세요! 힌트 : 도끼는 돌을 가지고 있어야 만들 수 있습니다."


    public bool IsAxeCrafted(InventorySystem inventory)
    {
        // 인벤토리에 도끼가 있는지 확인(AddItem쓰기)
        //return inventory.FindItem(Managers.Resource.GetCache<ItemData>("AxeItemData.data"), out _);
        return false;
    }
}
