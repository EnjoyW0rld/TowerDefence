using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class MoneyDropUI : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private TextMeshProUGUI _moneyText;
    [SerializeField] private float _fadeRatio;

    public void SetValue(int money)
    {
        _canvasGroup = GetComponentInChildren<CanvasGroup>();
        _moneyText = GetComponentInChildren<TextMeshProUGUI>();
        if (_moneyText == null) Debug.LogError("No text box was found");
        
        _moneyText.text = "+" + money;
    }

    private void Update()
    {
        if (_canvasGroup.alpha == 0)
        {
            Destroy(this.gameObject);
        }
        _canvasGroup.alpha -= _fadeRatio;
    }
}
