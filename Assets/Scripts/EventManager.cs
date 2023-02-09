using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public UnityEvent<int> OnMoneyChange; //passed value is current money count
    public UnityEvent<GameManager.GamePhase> OnPhaseChange;
    public UnityEvent<EnemyBase> OnEnemyDeath; //value is money dropped
}
