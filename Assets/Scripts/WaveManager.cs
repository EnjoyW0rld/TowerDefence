using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    private Vector3 _spawnPoint;
    [SerializeField] private EnemyBase enemy;
    // Start is called before the first frame update
    void Start()
    {
        _spawnPoint = FindObjectOfType<Path>().GetFirstPoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Instantiate(enemy, _spawnPoint, Quaternion.identity);
        }   
    }
}
