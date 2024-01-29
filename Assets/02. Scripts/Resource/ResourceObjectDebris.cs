using UnityEngine;

// 2024-01-24 WJY
public class ResourceObjectDebris : ResourceObjectBase
{
    [SerializeField] private float _respawnTime;
    public float RespawnTime => _respawnTime;


    public void Respawn()
    {
        _parent.SwitchState(_toObjectID);
    }
}