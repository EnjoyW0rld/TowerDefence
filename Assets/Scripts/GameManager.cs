using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    enum GamePhase { attack, build }

    private List<ITower> _placedTowers;
    private GamePhase _gamePhase = GamePhase.build;

    private ITower _currentTower;

    private void Start()
    {
        _placedTowers = new List<ITower>();
    }
    private void Update()
    {
        switch (_gamePhase)
        {
            case GamePhase.attack:
                break;
            case GamePhase.build:
                BuildPhaseManager();
                break;

        }
    }
    private void BuildPhaseManager()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {

        }
    }

}
