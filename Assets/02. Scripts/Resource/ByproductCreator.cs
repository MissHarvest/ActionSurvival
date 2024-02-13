using UnityEngine;

// 2024-02-05 WJY
public class ByproductCreator : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private GameObject _prefab;
    [Range(0f, 1f)] [SerializeField] private float _distribution;
    [SerializeField] private float _maxRange;
    [SerializeField] private float _minRange;
    [SerializeField] private int _maxCreateCount;

    private int _currentCreateCount;
    private DayCycle _manager;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_maxRange < _minRange)
            (_maxRange, _minRange) = (_minRange, _maxRange);
    }
#endif

    private void OnEnable()
    {
        if (_manager == null)
            _manager = Managers.Game.DayCycle;

        _currentCreateCount = 0;
        _manager.OnTimeUpdated += TryCreate;
    }

    private void OnDisable()
    {
        if (_manager != null)
            _manager.OnTimeUpdated -= TryCreate;
    }

    private void Start()
    {
        var managed = gameObject.GetOrAddComponent<ManagementedObject>();
        managed.Add(this, typeof(Behaviour));
    }

    public void TryCreate()
    {
        if (_distribution <= Random.value || _currentCreateCount >= _maxCreateCount)
            return;

        Vector3 spawnPosition = Random.onUnitSphere;
        spawnPosition.Set(spawnPosition.x, 0, spawnPosition.z);
        spawnPosition.Normalize();
        spawnPosition *= Random.Range(_minRange, _maxRange);
        spawnPosition += transform.position;

        Create(spawnPosition);
    }

    public void Create(Vector3 spawnPosition)
    {
        if (IsValidPosition(ref spawnPosition))
        {
            Managers.Game.ResourceObjectSpawner.SpawnObject(_prefab, spawnPosition);
            _currentCreateCount++;
        }
    }

    public bool IsValidPosition(ref Vector3 pos)
    {
        pos += Vector3.up * 50f;
        if (Physics.Raycast(pos, Vector3.down, out var hit, 100f, int.MaxValue, QueryTriggerInteraction.Collide))
        {
            pos = hit.point;
            return hit.collider.gameObject.layer == 12;
        }
        else
            return false;
    }
}