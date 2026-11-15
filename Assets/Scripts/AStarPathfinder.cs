using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarPathfinder : MonoBehaviour
{
    private const int StraightCost = 10;
    private const int DiagonalCost = 14;

    public Tilemap tilemap;

    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
    {
        var openSet = new PriorityQueue<Vector3Int>();
        var closedSet = new HashSet<Vector3Int>();
        var cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        var gScore = new Dictionary<Vector3Int, int> { [start] = 0 };

        openSet.Enqueue(start, Heuristic(start, goal));

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (!closedSet.Add(current))
                continue;

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            foreach (var neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor)) continue;
                if (!WalkableGridGenerator.Instance.IsWalkable(neighbor)) continue;

                int moveCost = IsDiagonal(current, neighbor) ? DiagonalCost : StraightCost;
                int tentativeG = gScore[current] + moveCost;

                if (!gScore.TryGetValue(neighbor, out int knownG) || tentativeG < knownG)
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    openSet.Enqueue(neighbor, tentativeG + Heuristic(neighbor, goal));
                }
            }
        }

        return null;
    }

    private List<Vector3Int> GetNeighbors(Vector3Int cell)
    {
        List<Vector3Int> neighbors = new();

        Vector3Int[] directions = {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right,
            new Vector3Int(-1, 1, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(-1, -1, 0),
            new Vector3Int(1, -1, 0)
        };

        foreach (var dir in directions)
        {
            Vector3Int neighbor = cell + dir;

            if (IsDiagonal(cell, neighbor))
            {
                Vector3Int d1 = new Vector3Int(neighbor.x, cell.y, 0);
                Vector3Int d2 = new Vector3Int(cell.x, neighbor.y, 0);

                if (!WalkableGridGenerator.Instance.IsWalkable(d1) ||
                    !WalkableGridGenerator.Instance.IsWalkable(d2))
                {
                    continue;
                }
            }

            neighbors.Add(neighbor);
        }

        return neighbors;
    }

    private int Heuristic(Vector3Int a, Vector3Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return StraightCost * (dx + dy) + (DiagonalCost - 2 * StraightCost) * Mathf.Min(dx, dy);
    }

    private bool IsDiagonal(Vector3Int a, Vector3Int b)
    {
        return a.x != b.x && a.y != b.y;
    }

    private List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        List<Vector3Int> path = new() { current };
        while (cameFrom.TryGetValue(current, out current))
        {
            path.Add(current);
        }
        path.Reverse();
        return path;
    }
}
