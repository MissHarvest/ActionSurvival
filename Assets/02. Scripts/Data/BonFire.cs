using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 16 Byun Jeongmin
public class BonFire : MonoBehaviour, IInteractable
{
    public void Interact(Player player)
    {
        Managers.Game.Player.Cooking.OnCookingShowAndHide();
        //Debug.Log("모닥불에서 요리 판넬 띄우기");
    }

    public void DropFireFromPlayerHand(ItemSlot itemInUse, Dictionary<string, GameObject> tools)
    {
        // 현재 플레이어가 들고 있는 도구의 이름을 가져옴
        string currentToolName = Managers.Game.Player.ToolSystem.GetToolName(itemInUse);

        if (currentToolName == "Handable_BonFire")
        {
            //인벤토리에서 모닥불 장착 해제 및 제거
            for (int i = 0; i < Managers.Game.Player.QuickSlot.slots.Length; i++)
            {
                if (Managers.Game.Player.QuickSlot.slots[i].itemSlot.itemData.displayName == "모닥불")
                {
                    //Debug.Log(i);
                    //Debug.Log(Managers.Game.Player.QuickSlot.slots[i]);
                    Managers.Game.Player.QuickSlot.UnRegist(Managers.Game.Player.QuickSlot.slots[0]);
                    break;
                }
            }

            // 플레이어의 손에서 모닥불 비활성화
            gameObject.SetActive(false);

            GameObject droppedTool = Instantiate(gameObject);
            droppedTool.transform.position = new Vector3(transform.position.x, 2f, transform.position.z + 1f);
            droppedTool.SetActive(true);
        }
    }
}
