using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    private GridMap _gridMap;
    private Tower _currentTower;

    [SerializeField] private SingleTargetTower _sniperTower;
    [SerializeField] private SlowingTower _slowingTower;
    [SerializeField] private AOE_Tower _aoeTower;

    private List<Tower> _placedTowers;
    private GameManager _gameManager;
    private EventManager _eventManager;

    private void Start()
    {
        _placedTowers = new List<Tower>();
        _gridMap = FindObjectOfType<GridMap>();
        _gameManager = FindObjectOfType<GameManager>();
        _eventManager = FindObjectOfType<EventManager>();
    }

    public void BuildPhaseManager()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mousePos.z = 0;

        mousePos = _gridMap.GetPosAtGridCenter(mousePos);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (_currentTower != null) _currentTower = null;
            _currentTower = Instantiate(_sniperTower, mousePos, Quaternion.identity).GetComponent<Tower>();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_currentTower != null) _currentTower = null;
            _currentTower = Instantiate(_slowingTower, mousePos, Quaternion.identity).GetComponent<Tower>();
        }
        else if (Input.GetKeyDown(KeyCode.Delete))
        {
            Tower tower = GetTowerOnPos(mousePos);
            if (_currentTower != null)
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
                int moneyAmount = _gameManager.GetMoneyAmount();
                if (price <= moneyAmount && price != -1)
                {
                    tower.LvlUp();
                    moneyAmount -= price;
                    _eventManager.OnMoneyChange?.Invoke(moneyAmount);
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
    public List<Tower> GetPlacedTowers() => _placedTowers;

}

[Serializable]
class TowerContainer
{
    public GameObject _towerPrefab;
    public Button _selectButton;
    public TextMeshProUGUI _priceText; 
}
