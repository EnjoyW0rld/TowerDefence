using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    private void OnPhaseChange(GameManager.GamePhase phase)
    {
        if (phase == GameManager.GamePhase.Build)
        {
            _canvas.enabled = _canUpgrade;
        }
        else
        {
            _canvas.enabled = false;
        }
    }
    private void OnDestroy()
    {
        _eventManager.OnPhaseChange.RemoveListener(OnPhaseChange);
        _eventManager.OnMoneyChange.RemoveListener(OnMoneyChange);
    }
}
