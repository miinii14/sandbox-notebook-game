using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WalkableGridGenerator : MonoBehaviour
{

    [SerializeField] ObjectFactory objectFactory;
    [SerializeField] Tilemap tilemap;
    [SerializeField] LayerMask obstacleLayer;
    public bool[,] walkableGrid;

    private Vector3Int gridOrigin;
    private Vector3Int gridSize;

    public static WalkableGridGenerator Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        CalculateGridBounds();
        GenerateWalkableGrid();
    }

    void CalculateGridBounds()
    {
        tilemap.CompressBounds();
        gridOrigin = tilemap.cellBounds.position;
        gridSize = tilemap.cellBounds.size;
    }

    public void GenerateWalkableGrid()
    {
        walkableGrid = new bool[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                walkableGrid[x, y] = true;
            }
        }


        foreach (GameObject obj in objectFactory.objects)
        {
            if (obj.GetComponentInChildren<ObjectData>().objectData.type != "PhysicalObject")
                continue;

            Vector3 worldPos = obj.transform.position;
            Vector3Int cell = tilemap.WorldToCell(worldPos);

            int x = cell.x - gridOrigin.x;
            int y = cell.y - gridOrigin.y;

            if (x >= 0 && y >= 0 && x < gridSize.x && y < gridSize.y)
            {
                walkableGrid[x, y] = false;
            }
        }
    }


    public bool IsWalkable(Vector3Int cell)
    {
        int x = cell.x - gridOrigin.x;
        int y = cell.y - gridOrigin.y;

        if (x < 0 || y < 0 || x >= gridSize.x || y >= gridSize.y)
            return false;

        return walkableGrid[x, y];
    }

    public List<Vector3Int> GetRandomWalkableCellAround(Vector3Int position, int radius)
    { 
        List<Vector3Int> candidates = new();

        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                Vector3Int candidate = new Vector3Int(position.x + dx, position.y + dy, 0);

                if (!IsWalkable(candidate)) continue;

                int x = candidate.x - gridOrigin.x;
                int y = candidate.y - gridOrigin.y;

                if (x >= 0 && y >= 0 && x < gridSize.x && y < gridSize.y)
                {
                    candidates.Add(candidate);
                }
            }
        }

        if (candidates.Count == 0)
        {
            return new List<Vector3Int> { position };
        }

        candidates = Shuffle(candidates);
        return candidates;
    }

    public List<Vector3Int> Shuffle(List<Vector3Int> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }

        return ts;
    }


    private void OnDrawGizmosSelected()
    {
        if (walkableGrid == null || tilemap == null)
            return;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3Int cell = new Vector3Int(x + gridOrigin.x, y + gridOrigin.y, 0);
                Vector3 worldPos = tilemap.GetCellCenterWorld(cell);
                Gizmos.color = walkableGrid[x, y] ? Color.green : Color.red;
                Gizmos.DrawWireCube(worldPos, tilemap.cellSize * 0.9f);
            }
        }
    }
}
