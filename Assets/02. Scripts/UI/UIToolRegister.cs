using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToolRegister : UIPopup
{
    enum GameObjects
    {
        QuickSlots,
    }

    public int sourceIndex { get; private set; } = -1;

    [SerializeField] private List<UIToolRegistSlot> _slots = new List<UIToolRegistSlot>();

    public override void Initialize()
    {
        base.Initialize();
        Bind<GameObject>(typeof(GameObjects));
    }

    private void Awake()
    {
        Initialize();
        var quickslotSystem = Managers.Game.Player.QuickSlot;
        quickslotSystem.OnUpdated += OnQuickSlotUpdate;
        CreateSlots(quickslotSystem);
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void CreateSlots(QuickSlotSystem quickSlotSystem)
    {        
        var parent = Get<GameObject>((int)GameObjects.QuickSlots).transform;

        for (int i = 0; i < QuickSlotSystem.capacity; ++i)
        {
            var slotUI = Managers.Resource.Instantiate("UIToolRegistSlot", Literals.PATH_UI, parent).GetOrAddComponent<UIToolRegistSlot>();
            slotUI.Init(this, i, quickSlotSystem.slots[i].itemSlot);
            _slots.Add(slotUI);
        }
    }

    public void Set(int index) // 일단 index 값이 필요함.
    {
        sourceIndex = index;
    }

    public void OnQuickSlotUpdate(int index, ItemSlot itemSlot)
    {
        _slots[index].Set(itemSlot);
    }
}
