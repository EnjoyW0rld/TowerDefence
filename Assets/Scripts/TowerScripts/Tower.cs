using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(BoxCollider2D))]
public class Tower : MonoBehaviour
{
    [SerializeField] protected TowerProperties _properties;
    private int _towerLevel = 0;

    protected float _shootRadius;

    protected float _shootSpeed;

    protected int _damage;
    private float _timeToShoot;

    [SerializeField] private Transform _radiusCircle;
    public bool CanShoot { get; private set; }



    private void Start()
    {
        UpdateValues();
    }

    protected virtual void Update()
    {
        if (_timeToShoot <= 0)
        {
            CanShoot = true;
            return;
        }
        _timeToShoot -= Time.deltaTime;
    }
    
    public void UpdateCooldown()
    {
        CanShoot = false;
        _timeToShoot = _shootSpeed;
    }
    public float GetShootRadius() => _shootRadius;
    public int GetDamage() => _damage;
    /// <summary>
    /// Sets new radius, attack and damage based on current tower lvl
    /// </summary>
    protected virtual void UpdateValues()
    {

        _shootRadius = _properties.TowerLevels[_towerLevel].ShootRadius;
        _shootSpeed = _properties.TowerLevels[_towerLevel].ShootSpeed;
        _damage = _properties.TowerLevels[_towerLevel].Damage;

        _radiusCircle.localScale = new Vector3(_shootRadius * 2 / transform.localScale.x, _shootRadius * 2 / transform.localScale.y, 0);
        _timeToShoot = _shootSpeed;
    }

    public int BuildPrice() => _properties.TowerLevels[0].Price;
    /// <summary>
    /// Get the price to level up current target
    /// </summary>
    /// <returns>if value is -1, means tower is max level</returns>
    public int LvlUpPrice()
    {
        if (_towerLevel + 1 == _properties.TowerLevels.Length) return -1;
        return _properties.TowerLevels[_towerLevel + 1].Price;
    }
    public void LvlUp()
    {
        _towerLevel++;
        UpdateValues();
    }
    /// <summary>
    /// Attacks single enemy in range
    /// Override to create different behaviour
    /// </summary>
    /// <param name="enemies">All enemies currently on the scene</param>
    /// <returns>true if succeded attack</returns>
    public virtual bool TryAttackEnemy(EnemyBase[] enemies)
    {
        if (enemies.Length == 0 || enemies == null)
        {
            return false;
        }
        for (int i = 0; i < enemies.Length; i++)
        {
            float distanceToEnemy = (enemies[i].transform.position - transform.position).magnitude;
            if (distanceToEnemy <= _shootRadius)
            {
                enemies[i].DamageThis(_damage);
                return true;
            }
        }
        return false;
    }
}