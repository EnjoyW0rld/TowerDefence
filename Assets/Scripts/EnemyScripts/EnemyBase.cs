using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class EnemyBase : MonoBehaviour
{
    private int _maxHealth;
    private int _remainingHealth;
    private int _money;

    private float _currentSpeed;
    private float _originalSpeed;

    private EnemyScriptBase _enemyProperties;

    private Queue<Vector3> _path;
    private Vector3 _targetPostition;
    private Vector3 _direction;

    private Effect _effect;
    [SerializeField] private GameObject _moneyDropUI;

    [HideInInspector] public UnityEvent<EnemyBase> OnReachTheEnd;

    public void SetValues(EnemyScriptBase enemyProperties)
    {
        GetComponent<SpriteRenderer>().sprite = enemyProperties.Sprite;


        _enemyProperties = enemyProperties;

        _currentSpeed = _enemyProperties.Speed;
        _originalSpeed = _enemyProperties.Speed;

        _money = _enemyProperties.Money;
        _maxHealth = _enemyProperties.Health;
        _remainingHealth = _enemyProperties.Health;
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
        UpdateEffect();
        transform.position += _currentSpeed * Time.deltaTime * _direction;
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

    protected virtual void UpdateEffect()
    {
        if (_effect == null) return;
        if (_effect.RemainingDuration > 0)
        {
            _currentSpeed = _originalSpeed / _effect.Modifier;
        }
        else
        {
            _currentSpeed = _originalSpeed;
            _effect = null;
        }
        _effect.RemainingDuration -= Time.deltaTime;
    }
    public bool SetEffect(Effect effect)
    {

        if (_effect == null)
        {
            _effect = effect;
            return true;
        }
        return false;
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
            FindObjectOfType<EventManager>().OnEnemyDeath?.Invoke(this);
        }
        return _remainingHealth <= 0;
    }
    public void SpawnMoneyUI()
    {
        MoneyDropUI _dropUI = Instantiate(_moneyDropUI, transform.position, Quaternion.identity).GetComponentInChildren<MoneyDropUI>();
        _dropUI.SetValue(_money);
    }
    
    public int GetRemainingHealth() => _remainingHealth;
    public int GetMaxHealth() => _maxHealth;
    public int GetMoney() => _money;
}

[Serializable]
public class Effect
{
    [HideInInspector] public float RemainingDuration = 5;
    [Tooltip("Value which enemy speed will be devided with")] public float Modifier;

    public Effect(float modifier)
    {
        Modifier = modifier;
    }
}