using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    private Vector3 _spawnPoint;
    [SerializeField] private EnemyBase enemy;
    [SerializeField] private int _enemiesToLose;
    private EventManager _eventManager;
    private List<EnemyBase> _enemies = new List<EnemyBase>();
    private int _enemiesReached;
    [SerializeField] private Wave wave;
    // Start is called before the first frame update
    void Start()
    {
        _eventManager = FindObjectOfType<EventManager>();
        _spawnPoint = FindObjectOfType<Path>().GetFirstPoint();
    }

    private float _timeTillSpawn;
    bool _gameStarted = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _gameStarted = true;
            _timeTillSpawn = wave.SpawnDelay;
        }
        if (_gameStarted)
        {
            _timeTillSpawn -= Time.deltaTime;
            if (_timeTillSpawn <= 0)
            {
                SpawnEnemy();
                _timeTillSpawn = wave.SpawnDelay;
            }
        }
    }

    public bool TryAttack(Tower tower)
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            float distanceToTower = (_enemies[i].transform.position - tower.transform.position).magnitude;
            if (distanceToTower <= tower.GetShootRadius())
            {
                if (_enemies[i].DamageThis(tower.GetDamage()))
                {
                    Destroy(_enemies[i].gameObject);
                    _enemies.Remove(_enemies[i]);
                    _eventManager.OnEnemyDeath?.Invoke(_enemies[i].Money);
                }
                return true;
            }
        }
        return false;
    }
    private void SpawnEnemy()
    {
        EnemyBase recentEnemy = Instantiate(enemy, _spawnPoint, Quaternion.identity);
        recentEnemy.SetValues(wave.GetRandomEnemy()); //Set scriptable object
        recentEnemy.OnReachTheEnd.AddListener(OnEnemyReached); //add event when enemy reaches end
        _enemies.Add(recentEnemy);

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

    public EnemyScriptBase GetRandomEnemy()
    {
        int randNum = UnityEngine.Random.Range(1, 101);
        int offset = 0;
        for (int i = 0; i < enemyTypes.Length; i++)
        {
            if (randNum <= enemyTypes[i].SpawnChance + offset)
            {
                return enemyTypes[i].Type;
            }
            offset += enemyTypes[i].SpawnChance;
        }
        Debug.LogError("Spawn chances for enemy is off!");
        return null;
    }
}
[Serializable]
class EnemyType
{
    public EnemyScriptBase Type;
    [Range(1, 100)] public int SpawnChance;
}