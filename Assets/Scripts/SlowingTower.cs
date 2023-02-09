using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowingTower : Tower
{
    [SerializeField] private Effect _effect;

    public override bool TryAttackEnemy(EnemyBase[] enemies)
    {
        if (enemies == null || enemies.Length == 0) return false;
        for (int i = 0; i < enemies.Length; i++)
        {
            float distanceToEnemy = (enemies[i].transform.position - transform.position).magnitude;
            if (distanceToEnemy <= _shootRadius)
            {
                if (enemies[i].SetEffect(_effect))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
