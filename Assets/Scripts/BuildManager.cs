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

    [SerializeField] private TowerContainer[] _towerContainers;
    private Tower[] _towers;

    private List<Tower> _placedTowers;
    private GameManager _gameManager;
    private EventManager _eventManager;

    private void Start()
    {
        _placedTowers = new List<Tower>();
        _gridMap = FindObjectOfType<GridMap>();
        _gameManager = FindObjectOfType<GameManager>();
        _eventManager = FindObjectOfType<EventManager>();
        _eventManager.OnMoneyChange.AddListener(OnMoneyChanged);

        _towers = new Tower[_towerContainers.Length];
        for (int i = 0; i < _towerContainers.Length; i++)
        {
            _towers[i] = _towerContainers[i].TowerPrefab.GetComponent<Tower>();
            _towerContainers[i].PriceText.text = "" + _towers[i].BuildPrice();
        }
    }

    public void BuildPhaseManager()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mousePos.z = 0;

        mousePos = _gridMap.GetPosAtGridCenter(mousePos);
        for (int i = 0; i < _towerContainers.Length; i++)
        {
            if (_towerContainers[i].SellectButton.IsPressed())
            {
                if (_currentTower != null)
                {
                    Destroy(_currentTower.gameObject);
                    _currentTower = null;
                }
                _currentTower = Instantiate(_towerContainers[i].TowerPrefab, mousePos, Quaternion.identity).GetComponent<Tower>();
                return;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
        }

        if (_currentTower == null) return;

        _currentTower.transform.position = mousePos;
        if (Input.GetMouseButtonDown(0))
        {
            if (_gridMap.isCellEmpty(mousePos))
            {
                _gridMap.OccupyGridCell(mousePos);
                _placedTowers.Add(_currentTower);
                _eventManager.OnMoneyChange?.Invoke(_gameManager.GetMoneyAmount() - _currentTower.BuildPrice());
                _currentTower = null;
                return;
            }

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

    void OnMoneyChanged(int money)
    {
        for (int i = 0; i < _towers.Length; i++)
        {
            Color color = _towerContainers[i].ShopImage.color;
            color.a = _towers[i].BuildPrice() > money ? .5f : 1f;
            _towerContainers[i].ShopImage.color = color;
        }
    }
}

[Serializable]
class TowerContainer
{
    public GameObject TowerPrefab;
    public ButtonOnClick SellectButton;
    public TextMeshProUGUI PriceText;
    public Image ShopImage;
}
