using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GamePhase { Attack, Build }

    private GamePhase _gamePhase = GamePhase.Build;

    private List<Tower> _placedTowers;
    private Tower _currentTower;
    private WaveManager _waveManager;
    private EventManager _eventManager;
    private GridMap _gridMap;
    private int _moneyAmount;

    [SerializeField] private GameObject sniperTower;

    private void Start()
    {
        _eventManager = FindObjectOfType<EventManager>();
        _eventManager.OnEnemyDeath.AddListener(UpdateMoneyAmount);

        _placedTowers = new List<Tower>();
        _waveManager = FindObjectOfType<WaveManager>();

        _gridMap = FindObjectOfType<GridMap>();
    }

    private void Update()
    {
        switch (_gamePhase)
        {
            case GamePhase.Attack:
                AttackPhaseManager();
                break;
            case GamePhase.Build:
                BuildPhaseManager();
                break;

        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            ChangePhase(GamePhase.Attack);
        }
    }

    private void AttackPhaseManager()
    {
        for (int i = 0; i < _placedTowers.Count; i++)
        {
            if (!_placedTowers[i].canShoot) continue;
            if (_waveManager.TryAttack(_placedTowers[i]))
            {
                _placedTowers[i].UpdateCooldown();
                //print("worked");
            }
        }
    }

    private void BuildPhaseManager()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mousePos.z = 0;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _currentTower = Instantiate(sniperTower, mousePos, Quaternion.identity).GetComponent<Tower>();
        }

        if (_currentTower == null) return;

        _currentTower.transform.position = _gridMap.GetPosAtGridCenter(mousePos);
        if (Input.GetMouseButtonDown(0))
        {
            if (_gridMap.isCellEmpty(mousePos))
            {
                _gridMap.OccupyGridCell(mousePos);
                _placedTowers.Add(_currentTower);
                _currentTower = null;
            }
        }
    }

    private void ChangePhase(GamePhase phase)
    {
        _gamePhase = phase;
        _eventManager?.OnPhaseChange.Invoke(_gamePhase);
    }
    private void UpdateMoneyAmount(int moneyToAdd)
    {
        _moneyAmount += moneyToAdd;
        _eventManager.OnMoneyChange?.Invoke(_moneyAmount);
    }
}
