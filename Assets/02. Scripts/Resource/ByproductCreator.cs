using UnityEngine;

// 2024-02-05 WJY
public class ByproductCreator : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private GameObject _prefab;
    [Range(0f, 1f)] [SerializeField] private float _distribution;
    [SerializeField] private float _range;
    [SerializeField] private int _maxCreateCount;

    private int _currentCreateCount;
    private DayCycle _manager;

    private void OnEnable()
    {
        if (_manager == null)
            _manager = Managers.Game.DayCycle;

        _currentCreateCount = 0;
        _manager.OnTimeUpdated += TryCreate;
    }

    private void Start()
    {
        TryCreate();
    }

    private void OnDisable()
    {
        if (_manager != null)
            _manager.OnTimeUpdated -= TryCreate;
    }


    public void TryCreate()
    {
        if (_distribution <= Random.value)
            return;

        Create();
    }

    // TODO: 생성 위치가 유효한지 확인해야함.
    // 현재 문제점
    // 1. 나무 속에 버섯이 생성될 가능성이 있음
    // 2. 버섯 끼리 겹쳐서 생성될 가능성이 있음
    // 3. 공중, 혹은 벽 속에 생성될 가능성이 있음
    public void Create()
    {
        if (_currentCreateCount >= _maxCreateCount)
            return;

        Vector3 spawnPosition = Random.insideUnitCircle * _range;
        spawnPosition.Set(spawnPosition.x, 0, spawnPosition.y);
        spawnPosition += transform.position;

        Instantiate(_prefab, spawnPosition, Quaternion.identity);
        _currentCreateCount++;
    }
}