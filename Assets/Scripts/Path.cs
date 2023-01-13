using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] private Vector3[] _pathPoints;
    private Queue<Vector3> _route;

    private LineRenderer _lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = _pathPoints.Length;
        _lineRenderer.SetPositions(_pathPoints);

        _route = new Queue<Vector3>(_pathPoints);
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < _pathPoints.Length; i++)
        {
            Vector3 pos = new Vector3(_pathPoints[i].x, _pathPoints[i].y, 0);
            Gizmos.DrawWireSphere(pos, .3f);
            UnityEditor.Handles.Label(pos + new Vector3(0, 0, -1), i.ToString());
        }
    }
    public Queue<Vector3> GetRoute() => _route;
    public Vector3 GetFirstPoint() => _pathPoints[0];
}
