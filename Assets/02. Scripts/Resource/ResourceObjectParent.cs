using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceObjectParent : MonoBehaviour
{
    private ResourceObjectGathering[] _gatherings;
    private ResourceObjectDebris[] _debris;

    private GameObject _currentObject;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _gatherings = GetComponentsInChildren<ResourceObjectGathering>(true);
        _debris = GetComponentsInChildren<ResourceObjectDebris>(true);
        _currentObject = transform.GetChild(0).gameObject;

        foreach (var e in _gatherings) e.Initialize();
        foreach (var e in _debris) e.Initialize();
    }

    public void SwitchObject(int ID)
    {
        // 자기 자신의 ID와 같다면 모든 Object가 꺼지게 해놨습니다. (버섯의 경우 때문. 임시)
        var toObject = transform.GetChild(ID).gameObject;
        toObject.SetActive(true);
        _currentObject.SetActive(false);
        _currentObject = toObject;
    }

    #region Test Code ...
    public void TestInteract()
    {
        if (_currentObject == null) return;

        if (_currentObject.activeSelf)
            _currentObject.GetComponent<ResourceObjectGathering>()?.Interact(Managers.Game.Player);
    }

    public void TestRespawn()
    {
        if (_currentObject == null) return;

        if (_currentObject.activeSelf)
            _currentObject.GetComponent<ResourceObjectDebris>()?.Respawn();
    }
    #endregion
}