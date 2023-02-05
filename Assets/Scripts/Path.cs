using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    private Vector3[] _pathPoints;
    [SerializeField] private PathPoint[] _pathPointsGrid;

    private LineRenderer _lineRenderer;
    private EdgeCollider2D _edgeCollider;

    private GridMap _gridMap;
    // Start is called before the first frame update
    void Awake()
    {
        _edgeCollider = GetComponent<EdgeCollider2D>();
        _lineRenderer = GetComponent<LineRenderer>();

        _lineRenderer.positionCount = _pathPointsGrid.Length;
        _pathPoints = PathPoints2vec3();
        _lineRenderer.SetPositions(_pathPoints);
        _edgeCollider.SetPoints(Vec3ToVec2(_pathPoints));

    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < _pathPointsGrid.Length; i++)
        {
            Vector3 pos = new Vector3(_pathPointsGrid[i].pos.x, _pathPointsGrid[i].pos.y, 0);
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
            positions[i] = _pathPointsGrid[i].pos;
        }
        return positions;
    }
    List<Vector2> Vec3ToVec2(Vector3[] vec3)
    {
        List<Vector2> res = new List<Vector2>();
        for (int i = 0; i < vec3.Length; i++)
        {
            res.Add(new Vector2(vec3[i].x, vec3[i].y));
        }
        return res;
    }
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

            _pathPointsGrid[i].pos = newPos;
        }
    }
}
[Serializable]
class PathPoint
{
    public int x;
    public int y;
    [HideInInspector] public Vector3 pos;
    public int[] ToArray() => new int[] { x, y };

}
