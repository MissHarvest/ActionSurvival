using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class QuickSlotSystem : MonoBehaviour
{
    public static int capacity = 4;
    [SerializeField] private QuickSlot[] _slots = new QuickSlot[capacity];
    private Player _player;

    public event Action<int, ItemSlot> OnUpdated;
    public event Action<QuickSlot> OnRegisted;
    public event Action<QuickSlot> OnUnRegisted;
    public event Action OnClickEmptySlot;

    [HideInInspector][SerializeField] private int _indexInUse = -1;

    private void Awake()
    {
        Debug.Log("QuickSystem Awake");
        for (int i = 0; i < capacity; ++i)
        {
            _slots[i] = new QuickSlot();
        }

        Load();
        _player = GetComponentInParent<Player>();
        Managers.Game.OnSaveCallback += Save;
    }

    private void Start()
    {
        Debug.Log("QuickSystem Start");
        Managers.Game.Player.Inventory.OnUpdated += OnInventoryUpdated;
    }

    public QuickSlot Get(int index)
    {
        return _slots[index];
    }

    public void Regist(int index, QuickSlot slot)
    {
        if (slot.itemSlot.itemData == null) return;

        UnRegist(index, true);        

        slot.itemSlot.SetRegist(true);
        _slots[index].Set(slot.targetIndex, slot.itemSlot);
        OnUpdated?.Invoke(index, slot.itemSlot);
        OnRegisted?.Invoke(slot);

        if (index == _indexInUse)
        {
            QuickUse(_indexInUse);
        }
    }

    // index 가 targetIndex 인 경우
    public void UnRegist(int index)
    {
        for (int i = 0; i < _slots.Length; ++i)
        {
            if (_slots[i].itemSlot.itemData != null && _slots[i].targetIndex == index)
            {
                _slots[i].itemSlot.SetRegist(false);
                OnUnRegisted?.Invoke(_slots[i]);
                _slots[i].Clear();
                OnUpdated?.Invoke(i, _slots[i].itemSlot);
                return;
            }
        }
    }

    // index 가 slot 의 index 인 경우
    private void UnRegist(int index, bool indexInUseStay)
    {
        if (_slots[index].itemSlot.itemData == null) return;
        _slots[index].itemSlot.SetRegist(false);
        OnUnRegisted?.Invoke(_slots[index]);
        _slots[index].Clear();
        OnUpdated?.Invoke(index, _slots[index].itemSlot);
        if (_indexInUse == index)
        {
            _indexInUse = indexInUseStay ? _indexInUse : -1;
        }
    }

    public void OnQuickUseInput(int index)
    {
        QuickUse(index);
    }

    private void QuickUse(int index)
    {
        if (_slots[index].itemSlot.itemData == null)
        {
            _indexInUse = -1;
            OnClickEmptySlot?.Invoke();
            return;
        }

        // 건설 및 음식이면 즉시 사용, index 변경 x
        // 장비 아이템이면 index 변경
        
        _indexInUse = index;
        _player.ItemUsageHelper.Use(_slots[index].targetIndex);
        if (_slots[index].itemSlot.itemData is ToolItemData)
        {
            _indexInUse = index;
        }        
    }

    private void QuickUse()
    {
        if (_indexInUse == -1) return;
        if (_slots[_indexInUse].itemSlot.itemData == null)
        {
            _indexInUse = -1;
            OnClickEmptySlot?.Invoke();
            return;
        }
        if (_slots[_indexInUse].itemSlot.itemData is ToolItemData)
        {
            _player.ItemUsageHelper.Use(_slots[_indexInUse].targetIndex);
        }
    }


    public void OnInventoryUpdated(int inventoryIndex, ItemSlot itemSlot)
    {
        for (int i = 0; i < capacity; ++i)
        {
            if (_slots[i].targetIndex == inventoryIndex)
            {
                if(itemSlot.itemData == null)
                {
                    _slots[i].Clear();
                    if (i == _indexInUse) _indexInUse = -1;
                }
                else
                {
                    _slots[i].Set(_slots[i].targetIndex, itemSlot);
                }
                
                OnUpdated?.Invoke(i, _slots[i].itemSlot);
                return;
            }
        }
    }

    private void Load()
    {
        if (SaveGame.TryLoadJsonToObject(this, SaveGame.SaveType.Runtime, "PlayerQuickSlot"))
        {
            for(int i = 0; i < _slots.Length; ++i)
            {
                _slots[i].itemSlot.LoadData();
            }

            if (_indexInUse != -1 && _slots[_indexInUse].itemSlot.itemData is ToolItemData)
            {
                QuickUse();
            }
        }
    }

    private void Save()
    {
        var json = JsonUtility.ToJson(this);
        SaveGame.CreateJsonFile("PlayerQuickSlot", json, SaveGame.SaveType.Runtime);
    }
}
