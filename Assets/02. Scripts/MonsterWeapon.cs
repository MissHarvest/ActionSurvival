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

    // Start is called before the first frame update
    void Start()
    {
        Collider.isTrigger = true;        
        Collider.enabled = _projectile != null;
        Debug.Log($"{Collider.enabled} /{_projectile != null}");
        gameObject.layer = 10;
    }

    public void ActivateWeapon()
    {
        Collider.enabled = true;
    }

    public void ActivateWeapon(float time)
    {
        Debug.Log("[Melee Attack On]");
        Collider.enabled = true;
        Invoke("InactivateWeapon", time);
    }

    public void InactivateWeapon()
    {
        Collider.enabled = false;
        Debug.Log("[Melee Attack Off]");
    }

    private void OnTriggerStay(Collider other)
    {
        var hittable = other.GetComponent<IHit>();
        if (hittable != null)
        {
            Owner.Attack(new AttackInfo(hittable, 0));
            Collider.enabled = false;
        }

        if (_projectile)
        {
            _projectile.Destroy();
        }
    }
}
