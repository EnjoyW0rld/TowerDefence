using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AOE_Tower : Tower
{
    /// <summary>
    /// Tries to attack all enemies in range
    /// </summary>
    /// <param name="enemies"></param>
    /// <returns></returns>
    public override bool TryAttackEnemy(EnemyBase[] enemies)
    {
        if (enemies == null || enemies.Length == 0) return false;

        List<EnemyBase> damagedEnemies = new List<EnemyBase>();
        for (int i = 0; i < enemies.Length; i++)
        {
            float distanceToEnemy = (enemies[i].transform.position - transform.position).magnitude;
            if (distanceToEnemy <= _shootRadius)
            {
                damagedEnemies.Add(enemies[i]);
            }
        }
        if (damagedEnemies.Count > 0)
        {
            foreach (EnemyBase enemy in damagedEnemies)
            {
                enemy.DamageThis(_damage);
            }
            return true;
        }
        return false;
    }
}
