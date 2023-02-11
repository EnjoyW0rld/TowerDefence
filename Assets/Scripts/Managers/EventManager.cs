using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public UnityEvent<int> OnMoneyChange; //passed value is current money count
    public UnityEvent<GameManager.GamePhase> OnPhaseChange;//passed value is current game state
    public UnityEvent<EnemyBase> OnEnemyDeath; //value is current enemy stats
    public UnityEvent<int> OnWaveEnd; //value is remaining amount of waves
    public UnityEvent<int> OnEnemyReached;//value is how much enemies left to lose
    public UnityEvent<bool> OnGameEnd;//true = won, false = lost

    [SerializeField] private EventContainer<int>[] _intContainer;
    [SerializeField] private EventContainer<float>[] _floatContainer;
    [SerializeField] private EventContainer<bool>[] _boolContainer;

    public void InvokeEvent(string name, bool value)
    {
        if (_boolContainer.Length == 0 || _boolContainer == null)
        {
            Debug.LogError("There are no bool Events!");
            return;
        }
        for (int i = 0; i < _intContainer.Length; i++)
        {
            if (name == _boolContainer[i].EventName)
            {
                _boolContainer[i].Event?.Invoke(value);
                return;
            }
        }
        Debug.LogError("No event found, check spelling and/or provided value.\n Looked for bool event");
    }
    public void InvokeEvent(string name, float value)
    {
        if (_floatContainer.Length == 0 || _floatContainer == null)
        {
            Debug.LogError("There are no float Events!");
            return;
        }
        for (int i = 0; i < _intContainer.Length; i++)
        {
            if (name == _floatContainer[i].EventName)
            {
                _floatContainer[i].Event?.Invoke(value);
                return;
            }
        }
        Debug.LogError("No event found, check spelling and/or provided value.\n Looked for float event");
    }
    public void InvokeEvent(string name, int value)
    {
        if (_intContainer.Length == 0 || _intContainer == null)
        {
            Debug.LogError("There are no int Events!");
            return;
        }
        for (int i = 0; i < _intContainer.Length; i++)
        {
            if (name == _intContainer[i].EventName)
            {
                _intContainer[i].Event?.Invoke(value);
                return;
            }
        }
        Debug.LogError("No event found, check spelling and/or provided value.\n Looked for int event");
    }

    public void SubscribeToEvent(string name, UnityAction<int> act)
    {
        if (_intContainer.Length == 0 || _intContainer == null)
        {
            Debug.LogError("No possible int events to subscribe");
            return;
        }
        for (int i = 0; i < _intContainer.Length; i++)
        {
            if (_intContainer[i].EventName == name)
            {
                _intContainer[i].Event.AddListener(act);
                return;
            }
        }
        Debug.LogError("No int event to subscribe was found!");
    }
    public void SubscribeToEvent(string name, UnityAction<float> act)
    {
        if (_floatContainer.Length == 0 || _floatContainer == null)
        {
            Debug.LogError("No possible float events to subscribe");
            return;
        }
        for (int i = 0; i < _floatContainer.Length; i++)
        {
            if (_floatContainer[i].EventName == name)
            {
                _floatContainer[i].Event.AddListener(act);
                return;
            }
        }
        Debug.LogError("No float event to subscribe was found!");
    }
    public void SubscribeToEvent(string name, UnityAction<bool> act)
    {
        if (_boolContainer.Length == 0 || _boolContainer == null)
        {
            Debug.LogError("No possible bool events to subscribe");
            return;
        }
        for (int i = 0; i < _boolContainer.Length; i++)
        {
            if (_boolContainer[i].EventName == name)
            {
                _boolContainer[i].Event.AddListener(act);
                return;
            }
        }
        Debug.LogError("No bool event to subscribe was found!");
    }

}

[Serializable]
class EventContainer<T>
{
    public string EventName;
    public UnityEvent<T> Event;
}