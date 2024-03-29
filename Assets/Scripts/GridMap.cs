using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    [SerializeField] private int _density;
    [SerializeField] private float _step;
    [SerializeField] private Vector3 _gridStart;
    private int[,] _grid;

    private Vector3 _topRight;
    [SerializeField] private bool _showGrid; //enables or disables drawing grid gizmos


    void Start()
    {
        RecalculateGrid();
        OccupyEssentialTiles();
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

    public Vector3[] GetGridBoundaries() => new Vector3[] { _gridStart, _topRight };
    public int[] GetGridDimensions() => new int[] { _grid.GetLength(0), _grid.GetLength(1) };
    public float GetStep() => _step;

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
            (int)(normPos.x / _step),
            (int)(normPos.y / _step)
        };
        res[0] = res[0] < 0 ? 0 : res[0];
        res[0] = res[0] > _grid.GetLength(0) - 1 ? _grid.GetLength(0) - 1 : res[0];

        res[1] = res[1] < 0 ? 0 : res[1];
        res[1] = res[1] > _grid.GetLength(1) - 1 ? _grid.GetLength(1) - 1 : res[1];

        return res;
    }

    /// <summary>
    /// Returns position on the middle of the grid cell near provided position
    /// </summary>
    /// <param name="gridNum"></param>
    /// <returns></returns>
    public Vector3 GetPosAtGridCenter(int[] gridNum)
    {
        Vector3 res = new Vector3(gridNum[0] * _step + _step / 2, gridNum[1] * _step + _step / 2, 0);
        return res + _gridStart;
    }
    public Vector3 GetPosAtGridCenter(Vector3 pos)
    {
        int[] gridNum = GetGridNumber(pos);
        return GetPosAtGridCenter(gridNum);
    }

    /// <summary>
    /// Returns if grid cell is empty on provided position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool IsCellEmpty(Vector3 pos)
    {
        return IsCellEmpty(GetGridNumber(pos));
    }
    public bool IsCellEmpty(int[] gridNumber)
    {
        return _grid[gridNumber[0], gridNumber[1]] == 1;
    }

    /// <summary>
    /// Occupies grid cell on provided position
    /// </summary>
    /// <param name="pos"></param>
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

    /// <summary>
    /// Makes grid cell on provided position free
    /// </summary>
    /// <param name="pos"></param>
    public void FreeGridCell(Vector3 pos)
    {
        FreeGridCell(GetGridNumber(pos));
    }
    public void FreeGridCell(int[] gridNumber)
    {
        _grid[gridNumber[0], gridNumber[1]] = 1;
    }

    /// <summary>
    /// Recaclulates step for grid
    /// </summary>
    [ContextMenu("Recalculate grid")]
    private void RecalculateGrid()
    {

        Rect rect = Camera.main.pixelRect;
        _gridStart = CameraToWorld(new Vector3(rect.x, rect.y));
        _topRight = CameraToWorld(new Vector3(rect.xMax, rect.yMax));

        _step = (_topRight.y - _gridStart.y) / 9 / _density;
        _grid = new int[(int)((_topRight.x - _gridStart.x) / _step), (int)((_topRight.y - _gridStart.y) / _step) + 1];
        FillGrid();
    }


    /// <summary>
    /// Used to set tiles on path to be occupied
    /// </summary>
    private void OccupyEssentialTiles()
    {

        Path path = FindObjectOfType<Path>();
        PathPoint[] pathPoints = path.GetPathPoints();
        //Used to occupy tiles dedicated to path
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
        //Used to occupy tiles dedicated to shop
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                _grid[x, y] = 0;
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
                    Gizmos.DrawWireCube(GetPosAtGridCenter(pos), new Vector3(_step, _step));
                }
            }

        }
    }
    private void OnValidate()
    {
        RecalculateGrid();
    }
}
