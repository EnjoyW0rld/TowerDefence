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
    private int _moneyAmount = 50;

    [SerializeField] private GameObject _sniperTower;
    [SerializeField] private GameObject _aoeTower;
    [SerializeField] private GameObject _slowingTower;

    private void Start()
    {
        _eventManager = FindObjectOfType<EventManager>();
        _eventManager.OnEnemyDeath.AddListener(UpdateMoneyAmount);
        _eventManager.OnPhaseChange.AddListener(OnPhaseChange);

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
            _eventManager.OnPhaseChange?.Invoke(GamePhase.Attack);//ChangePhase(GamePhase.Attack);
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
            }
        }
    }

    private void BuildPhaseManager()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mousePos.z = 0;

        mousePos = _gridMap.GetPosAtGridCenter(mousePos);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _currentTower = Instantiate(_sniperTower, mousePos, Quaternion.identity).GetComponent<Tower>();
        }else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _currentTower = Instantiate(_slowingTower, mousePos, Quaternion.identity).GetComponent<Tower>();
        }
        else if (Input.GetKeyDown(KeyCode.Delete))
        {
            Tower tower = GetTowerOnPos(mousePos);
            if(_currentTower != null)
            {
                Destroy(_currentTower.gameObject);
                _currentTower = null;
            }
            else if (tower != null)
            {
                _gridMap.FreeGridCell(tower.transform.position);
                Destroy(tower.gameObject);
                _placedTowers.Remove(tower);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Tower tower = GetTowerOnPos(mousePos);
            print(tower);
            if (tower != null)
            {
                int price = tower.LvlUpPrice();
                if (price <= _moneyAmount && price != -1)
                {
                    tower.LvlUp();
                    _moneyAmount -= price;
                    _eventManager.OnMoneyChange?.Invoke(_moneyAmount);
                }
            }
        }

        if (_currentTower == null) return;

        _currentTower.transform.position = mousePos;
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

    private Tower GetTowerOnPos(Vector3 pos)
    {
        if (_placedTowers.Count == 0) return null;
        for (int i = 0; i < _placedTowers.Count; i++)
        {
            if (_placedTowers[i].transform.position == pos)
            {
                return _placedTowers[i];
            }
        }
        return null;
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
}
