using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    [SerializeField] private int _density;
    [SerializeField] private float _verticalStep, _horizontalStep;
    [SerializeField] Vector3 _gridStart;
    int[,] _grid;

    [SerializeField] Vector3 topRight;
    [SerializeField] private bool _showGrid;

    void Start()
    {
        RecalculateGrid();
        OccupyPathTiles();
    }

    private void Update()
    {
        Vector3 mousePos = CameraToWorld(Input.mousePosition);
        SnapToGrid(mousePos);
    }

    private Vector3 CameraToWorld(Vector3 vec)
    {
        Vector3 res = Camera.main.ScreenToWorldPoint(vec);
        res.z = 0;
        return res;
    }
    private void FillGrid()
    {
        for (int i = 0; i < _grid.GetLength(0); i++)
        {
            for (int x = 0; x < _grid.GetLength(1); x++)
            {
                _grid[i, x] = 1;
            }
        }
    }
    void SnapToGrid(Vector3 pos)
    {
        int[] gridNumber = GetGridNumber(pos);
        Vector3 clampedPos = new Vector3(gridNumber[0] * _horizontalStep, gridNumber[1] * _verticalStep);
        transform.position = _gridStart + clampedPos;
    }
    /// <summary>
    /// Returns grid number based on provided vector3 position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public int[] GetGridNumber(Vector3 pos)
    {
        Vector3 normPos = pos - _gridStart;
        int[] res =
        {
            (int)(normPos.x / _horizontalStep),
            (int)(normPos.y / _verticalStep)
        };
        res[0] = res[0] < 0 ? 0 : res[0];
        res[0] = res[0] > _grid.GetLength(0) - 1 ? _grid.GetLength(0) - 1 : res[0];

        res[1] = res[1] < 0 ? 0 : res[1];
        res[1] = res[1] > _grid.GetLength(1) - 1 ? _grid.GetLength(1) - 1 : res[1];

        return res;
    }

    public Vector3 GetPosAtGridCenter(int[] gridNum)
    {
        Vector3 res = new Vector3(gridNum[0] * _horizontalStep + _horizontalStep / 2, gridNum[1] * _verticalStep + _verticalStep / 2, 0);
        return res + _gridStart;
    }
    public Vector3 GetPosAtGridCenter(Vector3 pos)
    {
        int[] gridNum = GetGridNumber(pos);
        return GetPosAtGridCenter(gridNum);
    }

    public bool isCellEmpty(Vector3 pos)
    {
        return isCellEmpty(GetGridNumber(pos));
    }
    public bool isCellEmpty(int[] gridNumber)
    {
        return _grid[gridNumber[0], gridNumber[1]] == 1;
    }

    public void OccupyGridCell(Vector3 pos)
    {
        OccupyGridCell(GetGridNumber(pos));
    }
    public void OccupyGridCell(int[] gridNumber)
    {
        if (_grid[gridNumber[0], gridNumber[1]] == 0)
        {
            Debug.LogError("grid cell is already occupied!");
            return;
        }
        _grid[gridNumber[0], gridNumber[1]] = 0;
    }

    public void FreeGridCell(Vector3 pos)
    {
        FreeGridCell(GetGridNumber(pos));
    }
    public void FreeGridCell(int[] gridNumber)
    {
        _grid[gridNumber[0], gridNumber[1]] = 1;
    }


    [ContextMenu("Recalculate grid")]
    private void RecalculateGrid()
    {
        _grid = new int[_density, _density];
        FillGrid();

        Rect rect = Camera.main.pixelRect;
        _gridStart = CameraToWorld(new Vector3(rect.x, rect.y));
        topRight = CameraToWorld(new Vector3(rect.xMax, rect.yMax));

        _verticalStep = (topRight.y - _gridStart.y) / _density;
        _horizontalStep = (topRight.x - _gridStart.x) / _density;
    }

    /// <summary>
    /// Used to make set tiles on path to be occupied
    /// </summary>
    private void OccupyPathTiles()
    {

        Path path = FindObjectOfType<Path>();
        PathPoint[] pathPoints = path.GetPathPoints();
        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            if (pathPoints[i].x == pathPoints[i + 1].x)
            {
                int step = pathPoints[i].y > pathPoints[i + 1].y ? -1 : 1;
                for (int y = pathPoints[i].y; y != pathPoints[i + 1].y + step; y += step)
                {
                    _grid[pathPoints[i].x, y] = 0;
                }
            }
            else
            {
                int step = pathPoints[i].x > pathPoints[i + 1].x ? -1 : 1;
                for (int x = pathPoints[i].x; x != pathPoints[i + 1].x + step; x += step)
                {
                    _grid[x, pathPoints[i].y] = 0;
                }
            }
        }
    }


    private void OnDrawGizmos()
    {
        if (_showGrid)
        {
            if (_grid == null)
            {
                RecalculateGrid();
            }
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                for (int y = 0; y < _grid.GetLength(1); y++)
                {
                    if (_grid[x, y] == 1)
                    {
                        Gizmos.color = Color.red;
                    }
                    else
                    {
                        Gizmos.color = Color.white;
                    }
                    int[] pos = new int[] { x, y };
                    Gizmos.DrawWireCube(GetPosAtGridCenter(pos), new Vector3(_horizontalStep, _verticalStep));
                }
            }

        }
    }
}
