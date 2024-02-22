using UnityEngine;

// 2024-01-24 WJY
public class ResourceObjectGathering : ResourceObjectBase, IInteractable
{
    [SerializeField] private ItemDropTable _itemTable;
    [SerializeField] private float _toolDurabilityReduceAmount;

    public float GetInteractTime()
    {
        throw new System.NotImplementedException();
    }

    public void Interact(Player player)
    {
        if (!gameObject.activeSelf)
            return;

        int targetIndex = player.ToolSystem.EquippedTool.targetIndex;
        if (targetIndex != -1)
            player.Inventory.TrySubtractDurability(targetIndex, _toolDurabilityReduceAmount);

        _itemTable?.AddInventory(player.Inventory);
        Parent.SwitchState(_toObjectID);
    }
}