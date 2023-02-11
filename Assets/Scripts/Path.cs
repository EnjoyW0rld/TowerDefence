using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    private Vector3[] _pathPoints;
    [SerializeField] private PathPoint[] _pathPointsGrid;
    [SerializeField] private Sprite _finishSprite;

    private LineRenderer _lineRenderer;

    private GridMap _gridMap;
    // Start is called before the first frame update
    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        _lineRenderer.positionCount = _pathPointsGrid.Length;
        _pathPoints = PathPoints2vec3();
        _lineRenderer.SetPositions(_pathPoints);

        GameObject obj = new GameObject("finishSprite");
        obj.transform.position = _pathPoints[_pathPoints.Length - 1];
        SpriteRenderer sprRend = obj.AddComponent<SpriteRenderer>();
        sprRend.sprite = _finishSprite;
        sprRend.sortingOrder = 1;
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < _pathPointsGrid.Length; i++)
        {
            Vector3 pos = new Vector3(_pathPointsGrid[i].Pos.x, _pathPointsGrid[i].Pos.y, 0);
            Gizmos.DrawWireSphere(pos, .3f);
            UnityEditor.Handles.Label(pos + new Vector3(0, 0, -1), i.ToString());
        }
    }

    public Queue<Vector3> GetRoute() => new Queue<Vector3>(_pathPoints);
    public Vector3 GetFirstPoint() => _pathPoints[0];
    private Vector3[] PathPoints2vec3()
    {
        Vector3[] positions = new Vector3[_pathPointsGrid.Length];
        for (int i = 0; i < _pathPointsGrid.Length; i++)
        {
            positions[i] = _pathPointsGrid[i].Pos;
        }
        return positions;
    }
    public PathPoint[] GetPathPoints() => _pathPointsGrid;
    private void OnValidate()
    {

        //null check for grid map
        if (_gridMap == null)
        {
            _gridMap = FindObjectOfType<GridMap>();
            if (_gridMap == null)
            {
                Debug.LogError("no grid map found, create it");
                return;
            }
        }

        for (int i = 0; i < _pathPointsGrid.Length; i++)
        {
            Vector3 newPos = _gridMap.GetPosAtGridCenter(_pathPointsGrid[i].ToArray());

            _pathPointsGrid[i].Pos = newPos;
        }
    }
}
[Serializable]
public class PathPoint
{
    public int x;
    public int y;
    [HideInInspector] public Vector3 Pos;
    public int[] ToArray() => new int[] { x, y };

}
