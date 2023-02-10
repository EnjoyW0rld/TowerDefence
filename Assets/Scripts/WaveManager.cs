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

    [SerializeField] private Wave[] _waves;
    private Queue<Wave> _waveQueue;
    private Wave _currentWave;
    private int _enemiesLeft;

    private float _timeTillSpawn;
    bool _gameStarted = false;

    void Start()
    {
        _waveQueue = new Queue<Wave>(_waves);

        _eventManager = FindObjectOfType<EventManager>();
        _eventManager.OnEnemyDeath.AddListener(DestroyEnemy);
        _eventManager.OnPhaseChange.AddListener(OnPhaseChange);

        _eventManager.OnEnemyReached?.Invoke(_enemiesToLose);

        _spawnPoint = FindObjectOfType<Path>().GetFirstPoint();
    }


    void Update()
    {
        if (_gameStarted)
        {
            _timeTillSpawn -= Time.deltaTime;
            if (_timeTillSpawn <= 0 && _enemiesLeft > 0)
            {
                SpawnEnemy();
                _timeTillSpawn = _currentWave.SpawnDelay;
                _enemiesLeft--;
            }
            if (_enemiesLeft == 0 && _enemies.Count == 0)
            {
                print("changed mode");
                _eventManager.OnPhaseChange?.Invoke(GameManager.GamePhase.Build);
                _eventManager.OnWaveEnd?.Invoke(_waveQueue.Count);
            }
        }
    }

    public bool TryAttack(Tower tower)
    {
        return tower.TryAttackEnemy(_enemies.ToArray());
        /*
        for (int i = 0; i < _enemies.Count; i++)
        {
            float distanceToTower = (_enemies[i].transform.position - tower.transform.position).magnitude;
            if (distanceToTower <= tower.GetShootRadius())
            {
                if (_enemies[i].DamageThis(tower.GetDamage()))
                {
                    //Destroy(_enemies[i].gameObject);
                    //_enemies.Remove(_enemies[i]);
                    //_eventManager.OnEnemyDeath?.Invoke(_enemies[i]);
                }
                return true;
            }
        }
        return false;
         */
    }

    private void SpawnEnemy()
    {
        EnemyBase recentEnemy = Instantiate(enemy, _spawnPoint, Quaternion.identity);
        recentEnemy.SetValues(_currentWave.GetRandomEnemy()); //Set scriptable object
        recentEnemy.OnReachTheEnd.AddListener(OnEnemyReached); //add event when enemy reaches end
        _enemies.Add(recentEnemy);

    }
    private void OnEnemyReached(EnemyBase enemy)
    {
        _enemiesToLose--;
        if (_enemiesToLose > 0)
        {
            _enemies.Remove(enemy);
            _eventManager.OnEnemyReached?.Invoke(_enemiesToLose);
        }
        else
        {
            _eventManager.OnGameEnd?.Invoke(false);
        }
    }
    private void DestroyEnemy(EnemyBase enemy)
    {
        enemy.SpawnMoneyUI();
        Destroy(enemy.gameObject);
        _enemies.Remove(enemy);
    }
    private void SetNextWave()
    {
        print("tried to deque");
        _currentWave = _waveQueue.Dequeue();
        _enemiesLeft = _currentWave.EnemyCount;

    }
    /// <summary>
    /// Starts next wave
    /// </summary>
    /// <param name="phase">Current game phase</param>
    private void OnPhaseChange(GameManager.GamePhase phase)
    {
        if (phase == GameManager.GamePhase.Attack)
        {
            SetNextWave();
            _gameStarted = true;
        }
        else if (phase == GameManager.GamePhase.Build)
        {
            _gameStarted = false;
        }
        else if (phase == GameManager.GamePhase.GameOver)
        {
            DestroyAllEnemies();
            _gameStarted = false;
        }
    }
    private void DestroyAllEnemies()
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            Destroy(_enemies[i].gameObject);
        }
        _enemies.Clear();
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