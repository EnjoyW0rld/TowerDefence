using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies")]
public class EnemyScriptBase : ScriptableObject
{
    public int Health;
    public float Speed;
    public int Money;
    public Sprite EnemySprite;
}
