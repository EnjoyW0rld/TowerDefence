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

    [SerializeField] private BoxCollider2D _boxCollider;
    [SerializeField, Min(0)] private Transform _radiusCircle;
    private bool _isColliding;
    public bool canShoot { get; private set; }



    private void Start()
    {
        UpdateValues();
    }

    protected virtual void Update()
    {
        if (_timeToShoot <= 0)
        {
            canShoot = true;
            return;
        }

        _timeToShoot -= Time.deltaTime;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        _isColliding = true;
    }
    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        _isColliding = false;
    }
    public void UpdateCooldown()
    {
        canShoot = false;
        _timeToShoot = _shootSpeed;
    }
    public float GetShootRadius() => _shootRadius;
    public bool IsColliding() => _isColliding;
    public int GetDamage() => _damage;

    public void UpdateValues()
    {

        _shootRadius = _properties.TowerLevels[_towerLevel].ShootRadius;
        _shootSpeed = _properties.TowerLevels[_towerLevel].ShootSpeed;
        _damage = _properties.TowerLevels[_towerLevel].Damage;


        _radiusCircle.localScale = new Vector3(_shootRadius * 2, _shootRadius * 2, 0);
        _timeToShoot = _shootSpeed;
    }

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