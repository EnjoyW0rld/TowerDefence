using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    private Vector3 _spawnPoint;
    [SerializeField] private EnemyBase enemy;
    [SerializeField] private int _enemiesToLose;
    private List<EnemyBase> _enemies = new List<EnemyBase>();
    private int _enemiesReached;
    [SerializeField] private Wave wave;
    // Start is called before the first frame update
    void Start()
    {
        _spawnPoint = FindObjectOfType<Path>().GetFirstPoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            EnemyBase recentEnemy = Instantiate(enemy, _spawnPoint, Quaternion.identity);
            recentEnemy.OnReachTheEnd.AddListener(OnEnemyReached);
            _enemies.Add(recentEnemy);
        }
    }
    List<EnemyBase> GetEnemyList() => _enemies;

    public bool TryAttack(Tower tower)
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            float distanceToTower = (_enemies[i].transform.position - tower.transform.position).magnitude;
            if (distanceToTower <= tower.GetShootRadius())
            {
                Destroy(_enemies[i].gameObject);
                _enemies.Remove(_enemies[i]);
                return true;
            }
        }
        return false;
    }

    void OnEnemyReached(EnemyBase enemy)
    {
        _enemiesReached++;
        if (_enemiesReached >= _enemiesToLose)
        {
            _enemies.Remove(enemy);
        }
    }
}

[Serializable]
class Wave
{
    public int EnemyCount;
    public float SpawnDelay;
    public EnemyType[] enemyTypes;
}
[Serializable]
class EnemyType
{
    public EnemyBase Type;
    [Range(0, 100)] public float SpawnChance;
}
