using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 12 Byun Jeongmin
public class CraftingTable : MonoBehaviour
{
    public void DropToolFromPlayerHand(ItemSlot itemInUse, Dictionary<string, GameObject> tools)
    {
        //// 현재 플레이어가 들고 있는 도구의 이름을 가져옴
        //string currentToolName = Managers.Game.Player.ToolSystem.GetToolName(itemInUse);

        //if (currentToolName == "Handable_CraftingTable")
        //{
        //    // 플레이어의 손에서 제작대 비활성화
        //    gameObject.SetActive(false);

        //    GameObject droppedTool = Instantiate(gameObject);
        //    droppedTool.transform.position = new Vector3(transform.position.x, 2f, transform.position.z + 1f);
        //    droppedTool.SetActive(true);
        //}
    }
}

