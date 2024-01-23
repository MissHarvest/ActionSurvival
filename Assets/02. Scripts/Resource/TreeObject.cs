using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObject : ResourceObject
{
    public override void Initialize()
    {
        InitLootingItem("LogItemData.data");
    }

    public override void Interact(Player player)
    {
        player.Inventory.AddItem(_lootingItem, 1);
        Instantiate(Managers.Resource.GetCache<GameObject>("TreeAStump.prefab"), transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public override void Respawn()
    {

    }
}
