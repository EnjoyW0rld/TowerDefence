using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIText : MonoBehaviour
{
    [SerializeField] private string _textToShow;
    [SerializeField, ContextMenuItem("Find text box", "FindTextInstance")] private TextMeshProUGUI _textIntance;

    public void UpdateText(GameManager.GamePhase phase)
    {
        _textIntance.text = _textToShow + phase;
    }
    public void UpdateText(int data)
    {
        _textIntance.text = _textToShow + data;
    }
    
    private void FindTextInstance()
    {
        _textIntance = GetComponent<TextMeshProUGUI>();
        if (_textIntance == null) Debug.LogError("Could not find text box");
    }

}
