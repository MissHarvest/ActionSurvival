using UnityEngine;

// 2024. 01. 24 Park jun Uk
public class Storage : MonoBehaviour, IInteractable
{
    public InventorySystem InventorySystem { get; private set; }
    public BuildableObject BuildableObject { get; private set; }

    public int maxCapacity = 20;
    private void Awake()
    {
        InventorySystem = GetComponent<InventorySystem>();
        InventorySystem.SetCapacity(maxCapacity);

        BuildableObject = GetComponent<BuildableObject>();
        BuildableObject.OnRenamed += InventorySystem.Load;
    }

    public void Interact(Player player)
    {
        var ui = Managers.UI.ShowPopupUI<UIStorage>();
        ui.SetStorage(this);
    }
}
