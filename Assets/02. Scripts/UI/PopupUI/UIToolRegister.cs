using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIToolRegister : UIPopup
{
    enum GameObjects
    {
        //QuickSlots_PC,
        QuickSlots_Mobile,
    }

    public QuickSlot SelectedSlot { get; private set; } = new QuickSlot();

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
        var parent = Get<GameObject>((int)GameObjects.QuickSlots_Mobile).transform;
        var slots = parent.GetComponentsInChildren<UIToolRegistSlot>();

        for (int i = 0; i < QuickSlotSystem.capacity; ++i)
        {
            var slotUIPrefab = Managers.Resource.GetCache<GameObject>("UIToolRegistSlot.prefab");            
            var slotUI = Instantiate(slotUIPrefab, parent).GetOrAddComponent<UIToolRegistSlot>();
            slotUI.gameObject.transform.position = slots[i].transform.position;
            slotUI.Init(this, i, quickSlotSystem.Get(i).itemSlot);
            _slots.Add(slotUI);
        }

        foreach(var slot in slots)
        {
            Destroy(slot.gameObject);
        }
    }

    public void Set(int index, ItemSlot itemSlot) // 일단 index 값이 필요함.
    {
        SelectedSlot.Set(index, itemSlot);
    }

    public void OnQuickSlotUpdate(int index, ItemSlot itemSlot)
    {
        _slots[index].Set(itemSlot);
    }
}
