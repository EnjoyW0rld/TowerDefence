using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GamePhase { Attack, Build }

    private GamePhase _gamePhase = GamePhase.Build;

    private WaveManager _waveManager;
    private EventManager _eventManager;
    private BuildManager _buildManager;
    //private GridMap _gridMap;
    private int _moneyAmount = 50;

    private void Start()
    {
        _eventManager = FindObjectOfType<EventManager>();
        _eventManager.OnEnemyDeath.AddListener(UpdateMoneyAmount);
        _eventManager.OnPhaseChange.AddListener(OnPhaseChange);

        _waveManager = FindObjectOfType<WaveManager>();
        _buildManager = FindObjectOfType<BuildManager>();

    }

    private void Update()
    {
        switch (_gamePhase)
        {
            case GamePhase.Attack:
                AttackPhaseManager();
                break;
            case GamePhase.Build:
                _buildManager.BuildPhaseManager();
                break;

        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            _eventManager.OnPhaseChange?.Invoke(GamePhase.Attack);//ChangePhase(GamePhase.Attack);
        }
    }

    private void AttackPhaseManager()
    {
        List<Tower> placedTowers = _buildManager.GetPlacedTowers();
        for (int i = 0; i < placedTowers.Count; i++)
        {
            if (!placedTowers[i].canShoot) continue;
            if (_waveManager.TryAttack(placedTowers[i]))
            {
                placedTowers[i].UpdateCooldown();
            }
        }
    }



    private void OnPhaseChange(GamePhase phase)
    {
        _gamePhase = phase;
    }
    private void UpdateMoneyAmount(EnemyBase enemy)
    {
        _moneyAmount += enemy.GetMoney();
        _eventManager.OnMoneyChange?.Invoke(_moneyAmount);
    }
    public int GetMoneyAmount() => _moneyAmount;
}
