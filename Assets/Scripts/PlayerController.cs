using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public float speed = 3f;
    public Tilemap tilemap;

    private AStarPathfinder pathfinder;
    private List<Vector3> pathWorld;
    private int pathIndex;
    private Camera mainCamera;

    void Start()
    {
        pathfinder = FindFirstObjectByType<AStarPathfinder>();
        pathWorld = new List<Vector3>();
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(mainCamera.transform.position.z);
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            Vector2 mousePos2D = new Vector2(worldPos.x, worldPos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider != null)
            {
                GameObject clickedObject = hit.collider.gameObject;
                if(clickedObject.name == "ObjectSprite")
                {
                    GameObject objectSprite = hit.collider.gameObject;
                    GameObject npc = objectSprite.transform.parent.gameObject;
                    if (npc.name == "NPC")
                    {
                        npc.GetComponent<NPCController>().stateMachine.ChangeState(new FleeState(npc.GetComponent<NPCController>()));
                    }
                }
            }

            Vector3Int destinationCell = tilemap.WorldToCell(worldPos);
            Vector3Int playerCell = tilemap.WorldToCell(transform.position);

            List<Vector3Int> path = pathfinder.FindPath(playerCell, destinationCell);

            if (path != null)
            {
                pathWorld.Clear();
                foreach (var cell in path)
                    pathWorld.Add(tilemap.GetCellCenterWorld(cell));

                pathIndex = 0;
            }
        }

        MoveAlongPath();
    }

    void MoveAlongPath()
    {
        if (pathWorld == null || pathIndex >= pathWorld.Count)
            return;


        Vector3 target = pathWorld[pathIndex];
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.05f)
            pathIndex++;
    }
}
