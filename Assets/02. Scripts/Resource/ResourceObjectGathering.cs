using UnityEngine;

// 2024-01-24 WJY
public class ResourceObjectGathering : ResourceObjectBase, IInteractable
{
    [SerializeField] private ItemData _lootingItem;

    public void Interact(Player player)
    {
        if (!gameObject.activeSelf)
            return;

        if (_lootingItem != null)
            player.Inventory.AddItem(_lootingItem, 1);
        _parent.SwitchObject(_toObjectID);
    }
}