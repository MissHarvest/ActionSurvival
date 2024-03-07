using UnityEngine;

// 2024-02-15 WJY
public class ArtifactHitbox : MonoBehaviour, IHit
{
    [SerializeField] private Artifact _parent;
    public Condition HP { get; private set; }
    private IAttack _lastAttacker;

    public void SetInfo(float HPMax, float HPRegen, float? HPCurrent)
    {
        HP = new(HPMax)
        {
            regenRate = HPRegen,
            currentValue = HPCurrent ?? HPMax,
        };
        HP.OnBelowedToZero += () => { _parent.DestroyByAttack(_lastAttacker); };
    }

    public void Hit(IAttack attacker, float damage)
    {
        _lastAttacker = attacker;
        HP.Subtract(damage);
    }
}