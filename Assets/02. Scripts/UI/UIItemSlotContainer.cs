using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemSlotContainer : UIBase
{
    private List<UIItemSlot> _uiItemSlots = new List<UIItemSlot>();
    private GridLayoutGroup _group;
    public override void Initialize()
    {
        
    }

    private void Awake()
    {
        _group = GetComponent<GridLayoutGroup>();
    }

    public virtual void CreateItemSlots<T>(GameObject slotPrefab, int count) where T : UIItemSlot
    {
        for (int i = 0; i < count; ++i)
        {
            var itemSlotUI = Instantiate(slotPrefab, this.transform);
            var inventoryslotUI = itemSlotUI.GetComponent<T>();
            _uiItemSlots.Add(inventoryslotUI);
        }
    }

    public virtual void Init<T>(InventorySystem inventory, Action<UIItemSlot> action) where T : UIItemSlot
    {
        for(int i = 0; i < _uiItemSlots.Count; ++i)
        {
            var slot = _uiItemSlots[i] as T;
            if (slot != null)
            {
                slot.BindGroup(this, i);
                slot.Set(inventory.Get(i));
                slot.OnClicked(action);
            }
        }
    }

    public void OnItemSlotUpdated(int index, ItemSlot itemSlot)
    {
        _uiItemSlots[index].Set(itemSlot);
    }

    public void HighLight(int index)
    {
        Debug.Log("[index] " + index);
        var go = _uiItemSlots[index].gameObject;

        int xOffset = 40;
        int yOffset = 30;

        var row = 4 - index / 6;
        var col = index % 6;

        var xSpace = _group.spacing.x;
        var ySpace = _group.spacing.y;

        var xWidth = _group.cellSize.x;
        var yHeight = _group.cellSize.y;

        var xPosition = xOffset + xWidth * 0.5f + (xSpace + xWidth) * col;
        var yPosition = yOffset + (row + 1) * (yHeight + ySpace);

        var arrowUI = Managers.UI.ShowPopupUI<UITutorialArrow>();

        arrowUI.ActivateArrow(gameObject.transform.position, new Vector2(xPosition, yPosition));
        go.BindEvent((x) =>
        {
            Managers.UI.ClosePopupUI(arrowUI);

            var evtHandler = Utility.GetOrAddComponent<UIEventHandler>(go);
            evtHandler.OnPointerDownEvent = null;
        }, UIEvents.PointerDown);
    }
}
