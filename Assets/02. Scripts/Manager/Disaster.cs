using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class Disaster : IAttack
{
    private GameObject _meteorPrefab;
    private GameObject _blizardPrefab;
    private GameObject _indicatorPrefab;

    public GameObject ObjectPoolContainer;

    private IObjectPool<MeteorObject> _meteores;
    private IObjectPool<MeteorObject> _blizards;
    private IObjectPool<AttackIndicatorCircle> _indicatores;

    private Transform _target;
    private float _range = 10.0f;
    private float _meteorTemp = 20.0f;
    private float _blizardTemp = -10.0f;
    private float _damage = 10.0f;

    private System.Action _fallDisaster;
    private bool _active = false;
    private Vector3[] _points;
    private Season _season;

    public void Init(Player player)
    {
        ObjectPoolContainer = new GameObject("@ObjectPoolContainer");
        _target = player.transform;
        _meteorPrefab = Managers.Resource.GetCache<GameObject>("TerrorBringerMeteor.prefab");
        _meteores = new ObjectPool<MeteorObject>(CreateMeteor, OnGetMeteor, OnReleaseMeteor, OnDestroyMeteor, maxSize: 30);

        _blizardPrefab = Managers.Resource.GetCache<GameObject>("Blizard.prefab");
        _blizards = new ObjectPool<MeteorObject>(CreateBlizard, OnGetBlizard, OnReleaseBlizard, OnDestroyBlizard, maxSize: 30);

        _indicatorPrefab = Managers.Resource.GetCache<GameObject>("CircleAttackIndicator.prefab");
        _indicatores = new ObjectPool<AttackIndicatorCircle>(CreateIndicator, OnGetIndicator, OnReleaseIndicator, OnDestroyIndicator, maxSize: 30);
        
        Managers.Game.DayCycle.OnTimeUpdated += Fall;
        Managers.Game.Season.OnSeasonChanged += OnSeasonChanged;

        _points = new Vector3[10];
    }

    private void OnSeasonChanged(Season.State state)
    {
        switch(state)
        {
            case Season.State.Summer:
                OnEnterSummer();
                break;

            case Season.State.Winter:
                OnEnterWinter();
                break;

            default:
                _active = false;
                break;
        }
    }

    private void Fall()
    {
        if (!_active) return;
        _fallDisaster?.Invoke();
    }

    private void OnEnterSummer()
    {
        _active = true;
        _fallDisaster = FallMeteor;
    }

    private void OnEnterWinter()
    {
        _active = true;
        _fallDisaster = FallBlizard;
    }

    private void FallMeteor()
    {
        CoroutineManagement.Instance.StartCoroutine(FallwithDelay(_meteores, CheckMeteorPoint));
    }
    private void FallBlizard()
    {
        CoroutineManagement.Instance.StartCoroutine(FallwithDelay(_blizards, CheckBlizardPoint));
    }

    IEnumerator FallwithDelay(IObjectPool<MeteorObject> pool, System.Func<Vector3, bool> condition)
    {
        GetPoints();
        var point = _points.Where(x => condition(x)).ToArray();

        if (point.Length <= 0) yield break;

        Managers.Sound.PlayEffectSound(_target.transform.position, "Falling", 0.15f);

        for (int i = 0; i < point.Length; ++i)
        {
            yield return new WaitForSeconds(0.25f);
            var meteor = pool.Get();
            meteor.Fall(point[i] + Vector3.up * 40.0f, 25);

            var indicator = _indicatores.Get();
            if (meteor.Collider is SphereCollider collider)
            {
                indicator.Activate(meteor.Destination, meteor.Speed, meteor.MaxDistance, collider);
            }
        }
    }

    private bool CheckMeteorPoint(Vector3 x)
    {
        return Managers.Game.Temperature.GetTemperature(x) >= _meteorTemp;
    }

    private bool CheckBlizardPoint(Vector3 x)
    {
        return Managers.Game.Temperature.GetTemperature(x) <= _blizardTemp;
    }

    private void GetPoints()
    {
        for (int i = 0; i < _points.Length; ++i)
        {
            _points[i] = _target.transform.position + new Vector3(
                Random.Range(-_range, _range), 0, Random.Range(-_range, _range));
        }
    }

    public void Attack(IHit target)
    {
        target.Hit(this, _damage);
    }

    #region Object Pooling
    private MeteorObject CreateMeteor()
    {
        MeteorObject meteor = Object.Instantiate(_meteorPrefab, ObjectPoolContainer.transform).GetComponent<MeteorObject>();
        meteor.SetManagedPool(_meteores);
        meteor.Owner = this;        
        return meteor;
    }

    private void OnGetMeteor(MeteorObject meteor)
    {
        meteor.gameObject.SetActive(true);
    }

    private void OnReleaseMeteor(MeteorObject meteor)
    {
        meteor.gameObject.SetActive(false);
    }

    private void OnDestroyMeteor(MeteorObject meteor)
    {
        Object.Destroy(meteor.gameObject);
    }

    private AttackIndicatorCircle CreateIndicator()
    {
        AttackIndicatorCircle indicator = 
            Object.Instantiate(_indicatorPrefab, ObjectPoolContainer.transform).GetComponent<AttackIndicatorCircle>();
        indicator.SetManagedPool(_indicatores);
        return indicator;
    }

    private void OnGetIndicator(AttackIndicatorCircle indicator)
    {
        indicator.gameObject.SetActive(true);
    }

    private void OnReleaseIndicator(AttackIndicatorCircle indicator)
    {
        indicator.gameObject.SetActive(false);
    }

    private void OnDestroyIndicator(AttackIndicatorCircle indicator)
    {
        Object.Destroy(indicator.gameObject);
    }

    private MeteorObject CreateBlizard()
    {
        MeteorObject blizard = Object.Instantiate(_blizardPrefab, ObjectPoolContainer.transform).GetComponent<MeteorObject>();
        blizard.SetManagedPool(_blizards);
        blizard.Owner = this;
        return blizard;
    }

    private void OnGetBlizard(MeteorObject meteor)
    {
        meteor.gameObject.SetActive(true);
    }

    private void OnReleaseBlizard(MeteorObject meteor)
    {
        meteor.gameObject.SetActive(false);
    }

    private void OnDestroyBlizard(MeteorObject meteor)
    {
        Object.Destroy(meteor.gameObject);
    }
    #endregion
}
