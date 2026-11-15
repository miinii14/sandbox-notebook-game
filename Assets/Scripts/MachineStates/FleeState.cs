using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FleeState : INPCState
{
    private NPCController npc;
    private AStarPathfinder pathfinder;
    private Tilemap tilemap;

    private List<Vector3Int> fleePath;
    private List<Vector3> pathWorld;
    private int pathIndex = 0;
    private float speed = 2f;

    public FleeState(NPCController npc)
    {
        this.npc = npc;
        this.pathfinder = GameObject.Find("AStarPathfinder").GetComponent<AStarPathfinder>();
        this.tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
    }

    public void Enter()
    {
        Vector3Int npcCell = tilemap.WorldToCell(npc.transform.position);
        Vector3Int threatCell = tilemap.WorldToCell(npc.player.transform.position);

        fleePath = CalculateFleePath(npcCell, threatCell);

        if (fleePath != null)
        {
            pathWorld = new List<Vector3>();
            foreach (var cell in fleePath)
                pathWorld.Add(tilemap.GetCellCenterWorld(cell));

            pathIndex = 0;
        }
        else
        {
            npc.stateMachine.ChangeState(new IdleState(npc));
        }
    }

    public void Exit() 
    {
    }

    public void Update()
    {
        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        if (pathWorld == null || pathIndex >= pathWorld.Count)
        {
            npc.stateMachine.ChangeState(new IdleState(npc));
            return;
        }

        Vector3 target = pathWorld[pathIndex];
        npc.transform.position = Vector3.MoveTowards(npc.transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(npc.transform.position, target) < 0.05f)
            pathIndex++;
    }

    private List<Vector3Int> CalculateFleePath(Vector3Int npc, Vector3Int threat)
    {
        Vector2 fleeDir = new Vector2(npc.x - threat.x, npc.y - threat.y).normalized;
        int radius = 6;

        List<Vector3Int> bestPath = null;
        float bestDistance = -1f;

        for (int angleOffset = -90; angleOffset <= 90; angleOffset += 15)
        {
            float angleRad = angleOffset * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(
                fleeDir.x * Mathf.Cos(angleRad) - fleeDir.y * Mathf.Sin(angleRad),
                fleeDir.x * Mathf.Sin(angleRad) + fleeDir.y * Mathf.Cos(angleRad)
            );

            Vector2 offset = dir * Random.Range(radius / 2f, radius);
            Vector3Int target = npc + new Vector3Int(Mathf.RoundToInt(offset.x), Mathf.RoundToInt(offset.y), 0);

            if (!WalkableGridGenerator.Instance.IsWalkable(target)) continue;

            var path = pathfinder.FindPath(npc, target);
            if (path != null && path.Count > 0)
            {
                float distToThreat = Vector3Int.Distance(target, threat);
                if (distToThreat > bestDistance)
                {
                    bestDistance = distToThreat;
                    bestPath = path;
                }
            }
        }

        return bestPath;
    }

}
