using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class MeteorObject : MonoBehaviour
{
    // monsterweapon
    public IAttack Owner { get; set; }
    private IObjectPool<MeteorObject> _managedPool;

    [field: SerializeField] public LayerMask hitLayer { get; private set; }
    public Collider Collider { get; private set; }
    public Vector3 Destination { get; private set; }
    public float MaxDistance =>_projectile.MaxDistance;
    public float Speed => _projectile.Speed;

    // projectile
    private Projectile _projectile;

    private void Awake()
    {
        var indicatorPrefab = Managers.Resource.GetCache<GameObject>("CircleAttackIndicator.prefab");
        var indicator = Instantiate(indicatorPrefab, transform, true);
        indicator.SetActive(false);

        Collider = GetComponent<Collider>();
        Collider.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("MonsterWeapon");

        _projectile = GetComponent<Projectile>();
        _projectile.OnDestroy += Destroy;
    }

    public void SetManagedPool(IObjectPool<MeteorObject> managedPool)
    {
        _managedPool = managedPool;
    }

    public void Fall(Vector3 start, float speed)
    {
        if (Physics.Raycast(start, Vector3.down, out RaycastHit hit, 100.0f, hitLayer))
        {
            Collider.enabled = true;
            _projectile.Fire(start, Vector3.down, speed, hit.distance);
            Destination = hit.point;
        }
    }

    public void Fall(Vector3 start, float speed, RaycastHit hit)
    {
        Collider.enabled = true;
        _projectile.Fire(start, Vector3.down, speed, hit.distance);
        Destination = hit.point;
    }

    public void Fire(Vector3 start, Vector3 direction, float speed, float maxDistance)
    {
        Collider.enabled = true;
        _projectile.Fire(start, direction, speed, maxDistance);
        Destination = start + direction.normalized * maxDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        var hittable = other.GetComponent<IHit>();
        if (hittable != null)
        {
            Owner.Attack(new AttackInfo(hittable, 0));
        }
        Destroy();        
    }

    private void Destroy()
    {        
        Managers.Sound.PlayEffectSound(transform.position, "Explosion", 1.0f, false);
        Collider.enabled = false;
        _managedPool.Release(this);
    }
}
