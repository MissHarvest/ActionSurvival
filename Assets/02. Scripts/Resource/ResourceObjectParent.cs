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

    public int CurrentState { get; private set; }
    public float RemainingTime { get; private set; }

    private bool _isInitialized = false;

    private void Awake()
    {
        Initialize();        
    }

    private void Start()
    {
        SetInfo(CurrentState, RemainingTime);
    }

    public void Update()
    {
        if (_debris.TryGetValue(CurrentState, out var debris))
        {
            RemainingTime -= Time.deltaTime;
            if (RemainingTime <= 0)
                debris.Respawn();
        }
    }

    private void Initialize()
    {
        if (_isInitialized) return;

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

        _isInitialized = true;
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
            obj.SetActive(false);

        CurrentState = stateID;
        _objects[CurrentState].SetActive(true);
        RemainingTime = GetCurrentDebrisRespawnTime();
    }

    public float GetCurrentDebrisRespawnTime()
    {
        if (_debris.TryGetValue(CurrentState, out var debris))
            return debris.RespawnTime;
        else return 0;
    }

    public void SetInfo(int stateID, float remainingTime)
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
            gathering.Interact(Managers.Game.Player);
    }

    public void TestRespawn()
    {
        if (!_isInitialized) return;

        if (_debris.TryGetValue(CurrentState, out var debris))
            debris.Respawn();
    }
    #endregion
}