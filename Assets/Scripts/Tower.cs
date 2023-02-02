using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(BoxCollider2D))]
public class Tower : MonoBehaviour
{
    [SerializeField] protected int _shootRadius;
    [SerializeField] protected float _shootSpeed;
    private float _timeToShoot;

    [SerializeField] private BoxCollider2D _boxCollider;
    [SerializeField] private Transform _radiusCircle;
    private bool _isColliding;
    public bool canShoot { get; private set; }

    private void Start()
    {
        _radiusCircle.localScale = new Vector3(_shootRadius * 2, _shootRadius * 2, 0);
        _timeToShoot = _shootSpeed;
    }

    protected virtual void Update()
    {
        if (_timeToShoot <= 0)
        {
            canShoot = true;
            return;
        }
        print("time gets decreased");
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
    public int GetShootRadius() => _shootRadius;
    public bool IsColliding() => _isColliding;
}