using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PatrolState : INPCState
{
    private NPCController npc;
    private AStarPathfinder pathfinder;
    private Tilemap tilemap;
    private WalkableGridGenerator walkableGridGenerator;

    private List<Vector3> pathWorld;
    private Vector3Int destinationCell;
    private int pathIndex;
    private float speed = 1f;
    private int patrolRadius = 4;

    public PatrolState(NPCController npc)
    {
        this.npc = npc;
    }

    public void Enter()
    {
        pathfinder = GameObject.Find("AStarPathfinder").GetComponent<AStarPathfinder>();
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        walkableGridGenerator = GameObject.Find("WalkableGridGenerator").GetComponent<WalkableGridGenerator>();
        pathWorld = new();

        Vector3Int npcCell = tilemap.WorldToCell(npc.transform.position);
        List<Vector3Int> destinationCells = walkableGridGenerator.GetRandomWalkableCellAround(npcCell, patrolRadius);

        foreach(var destination in destinationCells)
        {
            List<Vector3Int> path = pathfinder.FindPath(npcCell, destination);
            if (path != null)
            {
                pathWorld.Clear();
                foreach (var cell in path)
                    pathWorld.Add(tilemap.GetCellCenterWorld(cell));

                pathIndex = 0;
                destinationCell = destination;
                return;
            }
        }
    }

    public void Exit()
    {
        pathWorld.Clear();
    }

    public void Update()
    {
        MoveAlongPath();
    }

    void MoveAlongPath()
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
}
