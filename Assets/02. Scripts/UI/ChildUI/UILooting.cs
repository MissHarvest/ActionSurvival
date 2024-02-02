using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 21 Park Jun Uk
public class UILooting : UIBase
{
    enum GameObjects
    {
        Contents,
        HideContents,
    }

    private Stack<UILootingItemSlot> _usableSlots = new Stack<UILootingItemSlot>();
    private Stack<UILootingItemSlot> _usedSlots = new Stack<UILootingItemSlot>();
    private Queue<ItemSlot> _lootingItems = new Queue<ItemSlot>();

    public override void Initialize()
    {
        Bind<GameObject>(typeof(GameObjects));
    }

    private void Awake()
    {
        Initialize();
        //_contents = Get<GameObject>((int)Gameobjects.Contents).transform;

        // Create Slot
        var player = GameObject.Find("Player").GetComponent<Player>();
        player.Inventory.OnItemAdd += OnItemAdd;

        CreateSlots();
        StartCoroutine(ShowLootingItem());
    }

    IEnumerator ShowLootingItem()
    {
        while(true)
        {
            yield return null;
            if(_lootingItems.Count > 0)
            {
                var itemdata = _lootingItems.Dequeue();
                var slot = GetSlot();
                StartCoroutine(DeleteSlot(slot));
                slot.Set(itemdata);
                slot.gameObject.transform.SetParent(Get<GameObject>((int)GameObjects.Contents).transform, true);
                slot.gameObject.SetActive(true);
                Managers.Sound.PlayEffectSound(Managers.Game.Player.transform.position, "Looting3");
                _usedSlots.Push(slot);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private void CreateSlots()
    {
        var go = Managers.Resource.GetCache<GameObject>("UILootingItemSlot.prefab");
        for(int i = 0; i < 10; ++i)
        {
            var slot = Instantiate(go, Get<GameObject>((int)GameObjects.HideContents).transform);
            slot.SetActive(false);
            _usableSlots.Push(slot.GetComponent<UILootingItemSlot>());
        }
    }

    private UILootingItemSlot GetSlot()
    {
        if (_usableSlots.Count > 0)
        {
            return _usableSlots.Pop();
        }
        return null;
    }

    private void OnItemAdd(ItemSlot itemslot)
    {
        var itemData = new ItemSlot();
        itemData.Set(itemslot);
        _lootingItems.Enqueue(itemData);
    }

    IEnumerator DeleteSlot(UILootingItemSlot slot)
    {
        yield return new WaitForSeconds(1.0f);
        slot.gameObject.transform.SetParent(Get<GameObject>((int)GameObjects.HideContents).transform, true);
        slot.gameObject.SetActive(false);
        _usableSlots.Push(slot);
    }
}
