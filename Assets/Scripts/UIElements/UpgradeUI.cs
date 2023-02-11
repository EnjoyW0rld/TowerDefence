using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Class used to show if tower can be upgraded using current money
/// Uses world UI element
/// </summary>
public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    private TextMeshProUGUI _moneyText;

    private EventManager _eventManager;
    private Tower _parentTower;

    private bool _canUpgrade;
    void Start()
    {
        _parentTower = GetComponentInParent<Tower>();
        if (_parentTower == null) Debug.LogError("No tower element in parent found");

        _moneyText = GetComponentInChildren<TextMeshProUGUI>();
        _moneyText.text = "" + _parentTower.LvlUpPrice();

        _eventManager = FindObjectOfType<EventManager>();
        _eventManager.OnPhaseChange.AddListener(OnPhaseChange);
        _eventManager.OnMoneyChange.AddListener(OnMoneyChange);
    }

    /// <summary>
    /// When money changed checks if tower can be upgraded
    /// </summary>
    /// <param name="money"></param>
    private void OnMoneyChange(int money)
    {
        int price = _parentTower.LvlUpPrice();
        _moneyText.text = "" + _parentTower.LvlUpPrice();
        if (money >= price && price != -1)
        {
            _canUpgrade = true;
        }
        else
        {
            _canUpgrade = false;
        }
        _canvas.enabled = _canUpgrade;
    }
    /// <summary>
    /// Shows UI element based on current phase
    /// </summary>
    /// <param name="phase"></param>
    private void OnPhaseChange(GameManager.GamePhase phase)
    {
        if (phase == GameManager.GamePhase.Build)
        {
            _canvas.enabled = _canUpgrade;
        }
        else if(phase == GameManager.GamePhase.Attack)
        {
            _canvas.enabled = false;
        }
    }
    /// <summary>
    /// Unsubscribes from all events
    /// </summary>
    private void OnDestroy()
    {
        _eventManager.OnPhaseChange.RemoveListener(OnPhaseChange);
        _eventManager.OnMoneyChange.RemoveListener(OnMoneyChange);
    }
}
