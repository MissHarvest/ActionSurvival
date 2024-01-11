using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomObject : MonoBehaviour , IInteractable
{
    public void Interact(Player player)
    {
        var inventory = player.GetComponentInChildren<InventorySystem>();
        //var itemData = Resources.Load<ScriptableObject>("SO/MushroomItemData");
        //inventory.AddItem((ItemData)itemData, 1);
        var itemData = Managers.Resource.GetCache<ItemData>("MushroomItemData.data");
        inventory.AddItem(itemData, 1);
        Destroy(gameObject);
    }
}
