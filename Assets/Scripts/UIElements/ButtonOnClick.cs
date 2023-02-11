using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class used to check if button was clicked from code without connecting
/// any external function to OnClick event
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonOnClick : MonoBehaviour
{
    private Button _button;
    private bool _pressed;

    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonPress);
    }
    private void OnButtonPress() => _pressed = true;
    public bool IsPressed()
    {
        if (_pressed)
        {
            _pressed = false;
            return true;
        }
        return false;
    }
}
