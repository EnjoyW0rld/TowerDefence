using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    enum GamePhase { attack, build }

    private GamePhase _gamePhase = GamePhase.build;

    private List<Tower> _placedTowers;
    private Tower _currentTower;
    private WaveManager _waveManager;

    [SerializeField] GameObject sniperTower;

    private void Start()
    {
        _placedTowers = new List<Tower>();
        _waveManager = FindObjectOfType<WaveManager>();
    }

    private void Update()
    {
        switch (_gamePhase)
        {
            case GamePhase.attack:
                AttackPhaseManager();
                break;
            case GamePhase.build:
                BuildPhaseManager();
                break;

        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            _gamePhase = GamePhase.attack;
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

        _currentTower.transform.position = mousePos;
        if (Input.GetMouseButtonDown(0))
        {
            if (!_currentTower.IsColliding())
            {
                _placedTowers.Add(_currentTower);
                _currentTower = null;
            }
        }
    }

}
