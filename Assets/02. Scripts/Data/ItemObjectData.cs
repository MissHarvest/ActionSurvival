using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemObjectData : MonoBehaviour
{
    // lg
    public ToolItemData itemData;
    public bool onEquipTwoHandedTool = false;

    public void OnEquipTwoHandedTool()
    {
        // 여기서 캐릭터가 장착 중인지 검사하면 좋을 것 같은디..
        if (itemData.isTwoHandedTool == true)
        {
            Debug.Log("양 손 도구를 장착하였습니다.");
            onEquipTwoHandedTool = true;
        }
        else if (itemData.isTwoHandedTool == false)
        {
            Debug.Log("한 손 도구를 장착하였습니다.");            
        }
    }
    public void OnUnEquipTwoHandedTool()
    {
        if (itemData.isTwoHandedTool == true)
        {
            Debug.Log("양 손 도구를 해제하였습니다.");
            onEquipTwoHandedTool = false;
        }
        else if (itemData.isTwoHandedTool == false)
        {
            Debug.Log("한 손 도구를 해제하였습니다.");
        }
    }
}
