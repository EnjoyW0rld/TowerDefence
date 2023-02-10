using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GamePhase { Attack, Build, GameOver }

    private GamePhase _gamePhase;

    private WaveManager _waveManager;
    private EventManager _eventManager;
    private BuildManager _buildManager;

    private int _moneyAmount = 50;

    [SerializeField] private TextMeshProUGUI _gameEndText;

    private void Start()
    {
        _eventManager = FindObjectOfType<EventManager>();
        _eventManager.OnEnemyDeath.AddListener(UpdateMoneyAmount);
        _eventManager.OnPhaseChange.AddListener(OnPhaseChange);
        _eventManager.OnGameEnd.AddListener(OnGameEnd);

        _eventManager.OnMoneyChange?.Invoke(_moneyAmount);

        _waveManager = FindObjectOfType<WaveManager>();
        _buildManager = FindObjectOfType<BuildManager>();

        _eventManager.OnPhaseChange?.Invoke(GamePhase.Build);
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

    private void OnGameEnd(bool won)
    {
        _gameEndText.gameObject.SetActive(true);
        _gameEndText.text = won ? "You won!" : "You lost!";
        _eventManager.OnPhaseChange?.Invoke(GamePhase.GameOver);
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
