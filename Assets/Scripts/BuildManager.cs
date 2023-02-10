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

    [SerializeField] private float _buildPhaseTime;
    private float _timeLeft;
    [SerializeField] private TextMeshProUGUI _timeText;
    private string _originalText;

    private void Start()
    {
        if (_timeText != null) _originalText = _timeText.text;
        else Debug.LogError("No text box assigned for build phase time");

        _placedTowers = new List<Tower>();
        _gridMap = FindObjectOfType<GridMap>();
        _gameManager = FindObjectOfType<GameManager>();
        _eventManager = FindObjectOfType<EventManager>();
        _eventManager.OnMoneyChange.AddListener(OnMoneyChanged);
        _eventManager.OnPhaseChange.AddListener(OnPhaseChange);

        _towers = new Tower[_towerContainers.Length];
        for (int i = 0; i < _towerContainers.Length; i++)
        {
            _towers[i] = _towerContainers[i].TowerPrefab.GetComponent<Tower>();
            _towerContainers[i].PriceText.text = "" + _towers[i].BuildPrice();
        }
    }


    IEnumerator BuildPhaseTime()
    {
        while (_timeLeft > 0)
        {
            _timeLeft -= Time.deltaTime;
            _timeText.text = _originalText + RoundFloat(_timeLeft, 2);
            yield return null;
        }


        EndBuildPhase();
        _timeText.gameObject.SetActive(false);
        _eventManager.OnPhaseChange?.Invoke(GameManager.GamePhase.Attack);

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

        if (Input.GetMouseButtonDown(0)) UpgradeTower(mousePos);

        if (Input.GetKeyDown(KeyCode.Space)) _timeLeft = 0;
        if (_currentTower == null) return;

        _currentTower.transform.position = mousePos;
        if (Input.GetMouseButtonDown(0))
        {
            PlaceTower(mousePos);
        }
    }

    public void EndBuildPhase()
    {
        if (_currentTower != null)
        {
            Destroy(_currentTower.gameObject);
            _currentTower = null;
        }
    }

    private void PlaceTower(Vector3 mousePos)
    {
        if (_gridMap.isCellEmpty(mousePos))
        {
            _gridMap.OccupyGridCell(mousePos);
            _placedTowers.Add(_currentTower);
            _eventManager.OnMoneyChange?.Invoke(_gameManager.GetMoneyAmount() - _currentTower.BuildPrice());
            _currentTower = null;
        }
    }
    private void UpgradeTower(Vector3 mousePos)
    {
        Tower tower = GetTowerOnPos(mousePos);
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

    private float RoundFloat(float value, int digits)
    {
        float multiplier = Mathf.Pow(10, digits);
        int val = (int)(value * multiplier);
        return val / multiplier;
    }

    private void OnPhaseChange(GameManager.GamePhase phase)
    {
        if (phase == GameManager.GamePhase.Build)
        {
            _timeText.gameObject.SetActive(true);
            _timeLeft = _buildPhaseTime;
            StartCoroutine(BuildPhaseTime());

        }
    }

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
