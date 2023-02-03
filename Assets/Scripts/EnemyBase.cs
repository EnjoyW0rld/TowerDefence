using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class EnemyBase : MonoBehaviour
{
    public int Health { get { return _enemyProperties.Health; } }
    public int Speed { get { return _enemyProperties.Speed; } }
    public int Money { get { return _enemyProperties.Money; } }

    private int _remainingHealth;
    private EnemyScriptBase _enemyProperties;

    private Queue<Vector3> _path;
    private Vector3 _targetPostition;
    private Vector3 _direction;

    [HideInInspector] public UnityEvent<EnemyBase> OnReachTheEnd;

    public void SetValues(EnemyScriptBase enemyProperties)
    {
        _enemyProperties = enemyProperties;
        _remainingHealth = _enemyProperties.Health;
    }
    public EnemyBase(EnemyScriptBase enemyProperties)
    {
        _enemyProperties = enemyProperties;
    }

    private void Start()
    {
        _path = FindObjectOfType<Path>().GetRoute();
        if (_path == null)
        {
            Debug.LogError("no path found!");
            return;
        }
        _targetPostition = _path.Dequeue();
        _direction = GetDirection();
    }

    protected virtual void Update()
    {
        transform.position += Speed * Time.deltaTime * _direction;
        if ((_targetPostition - transform.position).magnitude < 0.2f)
        {
            if (_path.Count > 0)
            {
                _targetPostition = _path.Dequeue();
                _direction = GetDirection();
            }
            else
            {
                OnReachTheEnd?.Invoke(this);
                Destroy(this.gameObject);
            }

        }
    }
    private Vector3 GetDirection() => (_targetPostition - transform.position).normalized;
    /// <summary>
    /// Applies damage to intance of enemy and return if it is alive
    /// </summary>
    /// <param name="damage">Damage to be given</param>
    /// <returns></returns>
    public bool DamageThis(int damage)
    {

        _remainingHealth -= damage;
        print(_remainingHealth);
        return _remainingHealth <= 0;
    }
    
}
