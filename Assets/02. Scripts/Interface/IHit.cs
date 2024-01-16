using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHit
{
    void Hit(IAttack attacker, float damage);
}
