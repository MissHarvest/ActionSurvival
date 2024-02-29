using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2024-01-24 WJY
public class ResourceObjectParent : MonoBehaviour
{
    private Dictionary<int, ResourceObjectGathering> _gatherings = new();
    private Dictionary<int, ResourceObjectDebris> _debris = new();
    private Dictionary<int, GameObject> _objects = new();
    private DayCycle _manager;

    public int CurrentState { get; private set; }
    public int RemainingTime { get; private set; }

    private bool _isInitialized = false;

    private void Awake()
    {
        Initialize();        
    }

    private void Start()
    {
        SetInfo(CurrentState, RemainingTime);
    }

    private void OnDestroy()
    {
        if (_manager != null)
            _manager.OnTimeUpdated -= TimeLapse;
    }

    private void TimeLapse()
    {
        if (_debris.TryGetValue(CurrentState, out var debris))
        {
            --RemainingTime;
            if (RemainingTime <= 0)
                debris.Respawn();
        }
    }

    private void Initialize()
    {
        if (_isInitialized) return;

        _manager = GameManager.DayCycle;
        _manager.OnTimeUpdated += TimeLapse;
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child.TryGetComponent<ResourceObjectGathering>(out var gathering))
            {
                _gatherings.TryAdd(i, gathering);
                gathering.Initialize();
                _objects.TryAdd(i, child.gameObject);
            }
            if (child.TryGetComponent<ResourceObjectDebris>(out var debris))
            {
                _debris.TryAdd(i, debris);
                debris.Initialize();
                _objects.TryAdd(i, child.gameObject);
            }
        }
        CurrentState = 0;
        RemainingTime = GetCurrentDebrisRespawnTime();
        SetManagementedObject();

        _isInitialized = true;
    }

    public void SetManagementedObject()
    { 
        var managedObject = Utility.GetOrAddComponent<ManagementedObject>(gameObject);
        managedObject.AddRange(GetComponentsInChildren<Renderer>(true), typeof(Renderer));
        managedObject.AddRange(GetComponentsInChildren<Collider>(true), typeof(Collider));
    }

    public void SwitchState(int stateID)
    {
        // -1을 전달받을 경우 삭제
        if (stateID == -1)
        {
            Destroy(gameObject);
            return;
        }

        foreach (var obj in _objects.Values)
            obj?.SetActive(false);

        CurrentState = stateID;
        if (_objects.TryGetValue(CurrentState, out var nextObj))
            nextObj.SetActive(true);
        RemainingTime = GetCurrentDebrisRespawnTime();
    }

    public int GetCurrentDebrisRespawnTime()
    {
        if (_debris.TryGetValue(CurrentState, out var debris))
            return debris.RespawnTime;
        else return 0;
    }

    public void SetInfo(int stateID, int remainingTime)
    {
        //Initialize();
        SwitchState(stateID);
        RemainingTime = remainingTime;
    }

    #region Test Code ...
    public void TestInteract()
    {
        if (!_isInitialized) return;

        if (_gatherings.TryGetValue(CurrentState, out var gathering))
            gathering.Interact(GameManager.Instance.Player);
    }

    public void TestRespawn()
    {
        if (!_isInitialized) return;

        if (_debris.TryGetValue(CurrentState, out var debris))
            debris.Respawn();
    }
    #endregion
}