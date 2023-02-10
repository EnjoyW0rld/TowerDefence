using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    [SerializeField] private GameObject _gridPrefab;
    private GridMap _gridMap;
    void Start()
    {
        _gridMap = FindObjectOfType<GridMap>();
        Vector3[] gridBoundaries = _gridMap.GetGridBoundaries();
        int[] dimensions = _gridMap.GetGridDimensions();

        float step = _gridMap.GetStep();
        float xScale = gridBoundaries[1].x * 2;
        float yScale = gridBoundaries[1].y * 2;

        for (int y = 0; y < dimensions[1]; y++)
        {
            Vector3 offset = new Vector3(0, y * step, 0);
            GameObject temp = Instantiate(_gridPrefab, gridBoundaries[0] + offset,Quaternion.identity);
            Vector3 scale = temp.transform.localScale;
            scale.x = xScale;
            temp.transform.localScale = scale;
        }

        for (int x = 0; x < dimensions[0]; x++)
        {
            Vector3 offset = new Vector3(x * step, 0, 0);
            GameObject temp = Instantiate(_gridPrefab, gridBoundaries[0] + offset, Quaternion.identity);
            temp.transform.localRotation = Quaternion.Euler(0, 0, 90);
            Vector3 scale = temp.transform.localScale;
            scale.x = yScale;
            temp.transform.localScale = scale;
        }
    }

}
