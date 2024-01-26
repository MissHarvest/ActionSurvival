using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 16 Byun Jeongmin
public class BonFire : MonoBehaviour, IInteractable
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = true;
        var clip = Managers.Resource.GetCache<AudioClip>("BonFire.wav");
        _audioSource.clip = clip;
        _audioSource.Play();
    }

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
            // 플레이어의 손에서 모닥불 비활성화
            gameObject.SetActive(false);

            // 장착 해제
            Managers.Game.Player.Inventory.FindItem(itemInUse.itemData, out int index);
            var item = new QuickSlot();
            item.Set(index, Managers.Game.Player.Inventory.slots[index]);
            Managers.Game.Player.QuickSlot.UnRegist(item);

            // 인벤토리에서 제거
            Managers.Game.Player.Inventory.DestroyItemByIndex(item);

            GameObject droppedTool = Instantiate(gameObject);
            droppedTool.transform.position = new Vector3(transform.position.x, 2f, transform.position.z + 1f);
            droppedTool.SetActive(true);
        }
    }
}
