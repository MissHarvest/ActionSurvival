using UnityEngine;

// 2024-01-24 WJY
public class ResourceObjectGathering : ResourceObjectBase, IInteractable
{
    [SerializeField] private ItemDropTable _itemTable;

    public void Interact(Player player)
    {
        if (!gameObject.activeSelf)
            return;

        _itemTable?.AddInventory(player.Inventory);
        _parent.SwitchState(_toObjectID);
    }
}