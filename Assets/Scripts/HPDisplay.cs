using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPDisplay : MonoBehaviour
{

    private float _maxHP;
    private float _originalScale;
    [SerializeField] private EnemyBase _parent;

    void Start()
    {
        _maxHP = _parent.GetMaxHealth();
        _originalScale = transform.localScale.x;
    }
    private void Update()
    {
        float scaleFactor = _parent.GetRemainingHealth() / _maxHP;
        float newScale = Mathf.Lerp(0, _originalScale, scaleFactor);
        transform.localScale = new Vector3(newScale, transform.localScale.y, transform.localScale.z);
    }
}
