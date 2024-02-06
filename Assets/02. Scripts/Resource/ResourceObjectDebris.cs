using UnityEngine;

// 2024-01-24 WJY
public class ResourceObjectDebris : ResourceObjectBase
{
    [SerializeField] private int _respawnTime;
    public int RespawnTime => _respawnTime;


    public void Respawn()
    {
        _parent.SwitchState(_toObjectID);
    }
}