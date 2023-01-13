using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyBase : MonoBehaviour
{
    public int Health;
    public int Speed;
    public int Money;

    private Queue<Vector3> _path;
    private Vector3 _targetPostition;
    private Vector3 _direction;

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
        if((_targetPostition - transform.position).magnitude < 0.2f)
        {
            _targetPostition = _path.Dequeue();
            _direction = GetDirection();

        }
    }
    private Vector3 GetDirection() => (_targetPostition - transform.position).normalized;
}
