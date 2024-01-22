using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024. 01. 21 Park Jun Uk
public class UILooting : UIBase
{
    enum GameObjects
    {
        Contents,
    }

    private Stack<UILootingItemSlot> _usableSlots = new Stack<UILootingItemSlot>();
    private Stack<UILootingItemSlot> _usedSlots = new Stack<UILootingItemSlot>();

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
        //gameObject.SetActive(false);
    }

    private void CreateSlots()
    {
        var go = Managers.Resource.GetCache<GameObject>("UILootingItemSlot.prefab");
        for(int i = 0; i < 10; ++i)
        {
            var slot = Instantiate(go, Get<GameObject>((int)GameObjects.Contents).transform);
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

    private void OnItemAdd(ItemData itemdata)
    {
        var slot = GetSlot();
        StartCoroutine(DeleteSlot(slot));
        slot.Set(new ItemSlot(itemdata, 1));
        slot.gameObject.transform.parent = Get<GameObject>((int)GameObjects.Contents).transform;
        slot.gameObject.SetActive(true);
        _usedSlots.Push(slot);
    }

    IEnumerator DeleteSlot(UILootingItemSlot slot)
    {
        yield return new WaitForSeconds(1.0f);
        slot.gameObject.transform.parent = null;
        slot.gameObject.SetActive(false);
        _usableSlots.Push(slot);
    }
}
