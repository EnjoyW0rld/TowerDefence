using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    //Managers
    private GameManager _gameManager;
    private EventManager _eventManager;
    private GridMap _gridMap;
    //Tower variables
    [SerializeField] private TowerContainer[] _towerContainers;
    private Tower[] _towers;
    private Tower _currentTower;
    private List<Tower> _placedTowers;

    //Time variables
    [SerializeField,Tooltip("Amount of seconds for build phase")] private float _buildPhaseTime;
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

    /// <summary>
    /// Timer for build phase, ends build phase and changes game phase
    /// </summary>
    /// <returns></returns>
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
        UpdateSelectedTower(mousePos);

        if (Input.GetMouseButtonDown(0)) UpgradeTower(mousePos);
        if (Input.GetMouseButtonDown(1))
        {
            if(_currentTower != null)
            {
                Destroy(_currentTower.gameObject);
                _currentTower = null;
                return;
            }
            Tower tower = GetTowerOnPos(mousePos);
            if (tower != null)
            {
                _placedTowers.Remove(tower);
                _gridMap.FreeGridCell(mousePos);
                Destroy(tower.gameObject);
                return;
            }
        }


        if (Input.GetKeyDown(KeyCode.Space)) _timeLeft = 0;
        if (_currentTower == null) return;

        _currentTower.transform.position = mousePos;
        if (Input.GetMouseButtonDown(0))
        {
            PlaceTower(mousePos);
        }
    }
    /// <summary>
    /// Updates value for _currentTower variable
    /// </summary>
    /// <param name="mousePos"></param>
    private void UpdateSelectedTower(Vector3 mousePos)
    {
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

    }
    /// <summary>
    /// Destroys tower that was planned to place
    /// </summary>
    public void EndBuildPhase()
    {
        if (_currentTower != null)
        {
            Destroy(_currentTower.gameObject);
            _currentTower = null;
        }
    }

    /// <summary>
    /// Places tower on grid map, occupies grid cell and updates money amount
    /// </summary>
    /// <param name="mousePos"></param>
    private void PlaceTower(Vector3 mousePos)
    {
        if (_gridMap.IsCellEmpty(mousePos) && _gameManager.GetMoneyAmount() >= _currentTower.BuildPrice())
        {
            _gridMap.OccupyGridCell(mousePos);
            _placedTowers.Add(_currentTower);
            _gameManager.UpdateMoneyAmount(-_currentTower.BuildPrice());
            _currentTower = null;
        }
    }
    /// <summary>
    /// Upgrades tower if applicable
    /// </summary>
    /// <param name="mousePos"></param>
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
    /// <summary>
    /// Returns tower on provided position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns>Returns placed tower if find, and null if not</returns>
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
    /// <summary>
    /// Returns List of placed towers
    /// </summary>
    /// <returns></returns>
    public List<Tower> GetPlacedTowers() => _placedTowers;
    /// <summary>
    /// Rounds float value to some amount of digits after comma
    /// </summary>
    /// <param name="value">Value to be rounded</param>
    /// <param name="digits">Amount of digits after comma</param>
    /// <returns></returns>
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
    /// <summary>
    /// Updates icons of towers that can be build using current money
    /// </summary>
    /// <param name="money"></param>
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
