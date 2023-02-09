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

    private int _originalSpeed;

    private int _remainingHealth;
    private EnemyScriptBase _enemyProperties;

    private Queue<Vector3> _path;
    private Vector3 _targetPostition;
    private Vector3 _direction;

    private Effect _effect;

    [HideInInspector] public UnityEvent<EnemyBase> OnReachTheEnd;

    public void SetValues(EnemyScriptBase enemyProperties)
    {
        GetComponent<SpriteRenderer>().sprite = enemyProperties.Sprite;


        _enemyProperties = enemyProperties;
        _remainingHealth = _enemyProperties.Health;
        _originalSpeed = _enemyProperties.Speed;
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

    protected virtual void ApplyEffect()
    {
        if (_effect == null) return;
        if(_effect.Duration > 0)
        {
            //Speed = _originalSpeed * 
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
        if (_remainingHealth <= 0)
        {
            print("enemy died");
            FindObjectOfType<EventManager>().OnEnemyDeath?.Invoke(this);
        }
        return _remainingHealth <= 0;
    }
    public int GetRemainingHealth() => _remainingHealth;

}

[Serializable]
class Effect
{
    public float Duration;
    [HideInInspector] public float RemainingDuration;
    [Tooltip("Value which enemy speed will be multiplied with")] public float Modifier;
}