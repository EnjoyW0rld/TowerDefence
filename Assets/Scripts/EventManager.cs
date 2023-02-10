using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public UnityEvent<int> OnMoneyChange; //passed value is current money count
    public UnityEvent<GameManager.GamePhase> OnPhaseChange;
    public UnityEvent<EnemyBase> OnEnemyDeath; //value is money dropped
    public UnityEvent<int> OnWaveEnd; //value is remaining amount of waves
    public UnityEvent<int> OnEnemyReached;//value is how much enemies left to lose
    public UnityEvent<bool> OnGameEnd;//true = won, false = lost
}
