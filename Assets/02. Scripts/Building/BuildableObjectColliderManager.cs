using System;
using UnityEngine;

// 2024. 01. 24 Byun Jeongmin
public class BuildableObjectColliderManager : MonoBehaviour
{
    //public event Action OnRedMatAction;
    //public event Action OnBluePrintMatAction;

    //[SerializeField] private Material _redMat;
    //[SerializeField] private Material _bluePrintMat;
    //[SerializeField] private Renderer _thisObjectRenderer;
    //[SerializeField] private Collider _thisObjectAnotherCollider;

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other != _thisObjectAnotherCollider || !Managers.Game.Player.Building.CanCreateObject)
    //    {
    //        _thisObjectRenderer.material = _redMat;
    //        OnRedMatAction?.Invoke();
    //    }
    //    else
    //    {
    //        _thisObjectRenderer.material = _bluePrintMat;
    //        OnBluePrintMatAction?.Invoke();
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    _thisObjectRenderer.material = _bluePrintMat;
    //    OnBluePrintMatAction?.Invoke();
    //}
}