using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] private Vector3[] _pathPoints;

    private LineRenderer _lineRenderer;
    private EdgeCollider2D _edgeCollider;

    // Start is called before the first frame update
    void Start()
    {
        _edgeCollider = GetComponent<EdgeCollider2D>();
        _lineRenderer = GetComponent<LineRenderer>();

        _lineRenderer.positionCount = _pathPoints.Length;
        _lineRenderer.SetPositions(_pathPoints);
        _edgeCollider.SetPoints(Vec3ToVec2(_pathPoints));

        //_route = new Queue<Vector3>(_pathPoints);
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

    public Queue<Vector3> GetRoute() => new Queue<Vector3>(_pathPoints);
    public Vector3 GetFirstPoint() => _pathPoints[0];

    List<Vector2> Vec3ToVec2(Vector3[] vec3)
    {
        List<Vector2> res = new List<Vector2>();
        for (int i = 0; i < vec3.Length; i++)
        {
            res.Add(new Vector2(vec3[i].x, vec3[i].y));
        }
        return res;
    }
}
