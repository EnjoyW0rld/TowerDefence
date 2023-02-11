using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIText : MonoBehaviour
{

    [SerializeField] private string _textToShow;
    [SerializeField, ContextMenuItem("Find text box", "FindTextInstance")] private TextMeshProUGUI _textIntance;
    /// <summary>
    /// Subscribe this function to update UI text based on game phase
    /// </summary>
    /// <param name="phase"></param>
    public void UpdateText(GameManager.GamePhase phase)
    {
        _textIntance.text = _textToShow + phase;
    }
    /// <summary>
    /// Subscribe this function to update UI text based on some int
    /// </summary>
    /// <param name="data"></param>
    public void UpdateText(int data)
    {
        _textIntance.text = _textToShow + data;
    }
    /// <summary>
    /// Find text box on current game object
    /// </summary>
    private void FindTextInstance()
    {
        _textIntance = GetComponent<TextMeshProUGUI>();
        if (_textIntance == null) Debug.LogError("Could not find text box");
    }

}
