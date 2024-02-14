using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Disaster : IAttack
{
    private GameObject _meteorPrefab;
    private GameObject _blizardPrefab;
    private Transform _target;
    private float _range = 10.0f;
    private float _meteorTemp;
    private float _blizardTemp;
    private float _damage = 10.0f;

    // 일정 주기 마다?
    // 넘겨줄 때 어떤 값을? 단순 (메테오 등의)갯수, or 각 섬에 있는 아티팩트의 비율? 온도비율?
    //

    public void Init(Player player)
    {
        _target = player.transform;
        _meteorPrefab = Managers.Resource.GetCache<GameObject>("Meteor.prefab");
        _meteorPrefab.GetComponent<MonsterWeapon>().Owner = this;

        _blizardPrefab = Managers.Resource.GetCache<GameObject>("Blizard.prefab");
    }

    public void StartFallDisaster(int count, int times)
    {
        var point = _target.transform.position + new Vector3(
            Random.Range(-_range, _range), Random.Range(-_range, _range));

        var temperature = Managers.Game.Temperature.GetTemperature(point);

        var prefab = temperature >= _meteorTemp ? _meteorPrefab :
            temperature <= _blizardTemp ? _blizardPrefab : null;
        
        if(prefab != null)
        {
            var disaster = Object.Instantiate(prefab, point + Vector3.up * 25.0f, Quaternion.identity);
            var projectile = disaster.GetComponent<Projectile>();
            projectile.Fire(point, 30.0f);
        }
    }

    IEnumerator FallDisaster(int count, int times)
    {
        for(int i = 0; i < times; ++i)
        {
            yield return new WaitForSeconds(0.5f);
            Vector3[] points = new Vector3[count];
            for (int j = 0; j < count; ++j)
            {
                points[j] = _target.transform.position + new Vector3(
                Random.Range(-_range, _range), Random.Range(-_range, _range));
            }
        }
    }

    public void Attack(IHit target)
    {
        target.Hit(this, _damage);
    }
}
