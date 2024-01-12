using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObject : MonoBehaviour, IInteractable
{
    public void Interact(Player player)
    {
        var inventory = player.GetComponentInChildren<InventorySystem>();
        //var itemData = Resources.Load<ScriptableObject>("SO/LogItemData");
        //inventory.AddItem((ItemData)itemData, 1);
        //Instantiate(Resources.Load<GameObject>("Prefabs/TreeAStump"), transform.position, transform.rotation);
        var itemData = Managers.Resource.GetCache<ItemData>("LogItemData.data");
        inventory.AddItem(itemData, 1);
        Instantiate(Managers.Resource.GetCache<GameObject>("TreeAStump.prefab"), transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
