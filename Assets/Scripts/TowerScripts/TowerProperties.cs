using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tower data", menuName = "Tower property")]
public class TowerProperties : ScriptableObject
{
    public TowerLevel[] TowerLevels;
}
[Serializable]
public class TowerLevel
{
    [Min(0)] public float ShootRadius;
    [Min(0)] public float ShootSpeed;
    [Min(0)] public int Damage;
    [Min(0)] public int Price;
}
