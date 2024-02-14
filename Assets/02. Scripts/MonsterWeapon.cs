using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterWeapon : MonoBehaviour
{
    public Collider Collider { get; private set; }
    public IAttack Owner { get; set; }

    private Projectile _projectile;

    private void Awake()
    {
        Collider = GetComponent<Collider>();
        _projectile = GetComponent<Projectile>();
    }

    private void OnEnable()
    {
        Collider.enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Collider.isTrigger = true;
        Collider.enabled = _projectile != null;
        gameObject.layer = 10;
    }

    public void ActivateWeapon()
    {
        Collider.enabled = true;
    }

    public void InactivateWeapon()
    {
        Collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var hittable = other.GetComponent<IHit>();
        if (hittable != null)
        {
            Owner.Attack(hittable);
        }
        Collider.enabled = false;
        
        if(_projectile)
        {
            _projectile.DestroySelf();
        }
    }
}
