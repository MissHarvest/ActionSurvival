using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

public class Disaster : IAttack
{
    private GameObject _meteorPrefab;
    private GameObject _blizardPrefab;
    private GameObject _indicatorPrefab;

    private Transform _target;
    private float _range = 10.0f;
    private float _meteorTemp = 10.0f;
    private float _blizardTemp = -10.0f;
    private float _damage = 10.0f;
    private System.Action _fallDisaster;
    private bool _active = false;
    private Vector3[] _points;

    public void Init(Player player)
    {
        _target = player.transform;
        _meteorPrefab = Managers.Resource.GetCache<GameObject>("TerrorBringerMeteor.prefab");

        _blizardPrefab = Managers.Resource.GetCache<GameObject>("Blizard.prefab");

        _indicatorPrefab = Managers.Resource.GetCache<GameObject>("CircleAttackIndicator.prefab");
        Managers.Game.DayCycle.OnTimeUpdated += Fall;

        _points = new Vector3[10];
        _active = true;
        OnEnterSummer();
    }

    private void Fall()
    {
        if (!_active) return;
        _fallDisaster?.Invoke();
    }

    private void OnEnterSummer()
    {
        _fallDisaster = FallMeteor;
    }

    private void OnEnterWinter()
    {
        _fallDisaster = FallBlizard;
    }

    private void FallMeteor()
    {
        GetPoints();
        var point = _points.Where(x => CheckMeteorPoint(x)).ToArray();

        for (int i = 0; i < point.Length; ++i)
        {
            if (_meteorPrefab != null)
            {
                var disaster = UnityEngine.Object.Instantiate(_meteorPrefab, point[i] + Vector3.up * 30.0f, Quaternion.identity);
                disaster.GetComponent<MonsterWeapon>().Owner = this;
                var projectile = disaster.GetComponent<Projectile>();

                var indicatorGo = Object.Instantiate(_indicatorPrefab, point[i] + Vector3.up * 0.1f, _indicatorPrefab.transform.rotation);
                var indicator = indicatorGo.GetComponent<AttackIndicatorCircle>();
                var sphereCollider = projectile.GetComponent<SphereCollider>();

                projectile.Fire(point[i], 60.0f);
                indicator.Set(30.0f, projectile.Speed, sphereCollider.radius);
            }
        }
    }

    private void FallBlizard()
    {
        GetPoints();
        var point = _points.Where(x => CheckMeteorPoint(x)).ToArray();

        for (int i = 0; i < point.Length; ++i)
        {
            if (_blizardPrefab != null)
            {
                var disaster = UnityEngine.Object.Instantiate(_blizardPrefab, point[i] + Vector3.up * 50.0f, Quaternion.identity);
                var projectile = disaster.GetComponent<Projectile>();
                projectile.Fire(point[i], 60.0f);
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
}
