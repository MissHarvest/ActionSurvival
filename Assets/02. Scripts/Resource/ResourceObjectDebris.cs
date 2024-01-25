using UnityEngine;

// 2024-01-24 WJY
public class ResourceObjectDebris : ResourceObjectBase
{
    private float _remainingTime;
    [SerializeField] private float _respawnTime;

    private void Update()
    {
        TimeLapse();
    }

    public void Respawn()
    {
        _remainingTime = _respawnTime;
        _parent.SwitchObject(_toObjectID);
    }

    private void TimeLapse()
    {
        _remainingTime -= Time.deltaTime;
        if (_remainingTime < 0f)
            Respawn();
    }

    public override void Initialize()
    {
        base.Initialize();
        _remainingTime = _respawnTime;
    }
}