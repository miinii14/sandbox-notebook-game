using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapPlacer : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] GameObject objectPrefab;
    [SerializeField] ObjectFactory objectFactory;

    private Camera mainCamera;
    private Dictionary<Vector3Int, GameObject> occupiedTiles;
    private static ObjectSO currentObject = null;
    private static Sprite currentSprite = null;
    private Vector3 playerPos;


    private void Start()
    {
        mainCamera = Camera.main;
        occupiedTiles = new Dictionary<Vector3Int, GameObject>();
        playerPos = Vector3.zero;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (currentObject)
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = Mathf.Abs(mainCamera.transform.position.z);
                Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

                Vector3Int cellPos = tilemap.WorldToCell(worldPos);
                Vector3 tileCenter = tilemap.GetCellCenterWorld(cellPos);

                if (!tilemap.cellBounds.Contains(cellPos) || !tilemap.HasTile(cellPos)) return;

                if (!occupiedTiles.ContainsKey(cellPos))
                {
                    if (currentObject.type == "Player")
                    {
                        if(playerPos != Vector3.zero)
                        {
                            return;
                        } else
                        {
                            playerPos = tileCenter;
                        }
                    }
                    
                    GameObject obj = Instantiate(objectPrefab);

                    switch(currentObject.type)
                    {
                        case "Player":
                            obj.AddComponent<PlayerController>();
                            obj.GetComponent<PlayerController>().enabled = false;
                            obj.name = "PLAYER";
                            foreach (Transform child in obj.transform)
                            {
                                child.gameObject.layer = LayerMask.NameToLayer("Default");
                            }
                            obj.layer = LayerMask.NameToLayer("Default");
                            break;

                        case "NPC":
                            obj.AddComponent<NPCController>();
                            NPCController npcController = obj.GetComponent<NPCController>();
                            npcController.enabled = false;
                            obj.name = "NPC";
                            foreach (Transform child in obj.transform)
                            {
                                child.gameObject.layer = LayerMask.NameToLayer("Default");
                            }
                            obj.layer = LayerMask.NameToLayer("Default");
                            break;

                        case "PhysicalObject":
                            obj.name = "PHYSICALOBJ";
                            break;
                    }
                    

                    SpriteRenderer spriteRenderer = obj.GetComponentInChildren<SpriteRenderer>();
                    spriteRenderer.sprite = currentSprite;
                    obj.GetComponentInChildren<ObjectData>().objectData = currentObject;
                    obj.GetComponentInChildren<BoxCollider2D>().size = spriteRenderer.bounds.size;

                    obj.transform.position = tileCenter;

                    Vector2 tileSize = new Vector2(1f, .5f);

                    float spriteUnitWidth = currentSprite.rect.width / currentSprite.pixelsPerUnit;
                    float spriteUnitHeight = currentSprite.rect.height / currentSprite.pixelsPerUnit;

                    float scaleX = tileSize.x / spriteUnitWidth;
                    float scaleY = tileSize.y / spriteUnitHeight;

                    float scale = Math.Min(scaleX, scaleY);

                    spriteRenderer.transform.localScale = new Vector3(scale, scale, 1f);

                    currentObject.position = obj.transform.position;
                    currentObject.localScale = scale;

                    occupiedTiles[cellPos] = obj;

                    objectFactory.objects.Add(obj);
                }
            }
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(mainCamera.transform.position.z);
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

            Vector3Int cellPos = tilemap.WorldToCell(worldPos);
            Vector3 tileCenter = tilemap.GetCellCenterWorld(cellPos);

            if (occupiedTiles.ContainsKey(cellPos))
            {
                if (tilemap.GetCellCenterWorld(cellPos) == playerPos)
                {
                    playerPos = Vector3.zero;
                }

                GameObject obj = occupiedTiles[cellPos];
                objectFactory.objects.Remove(obj);

                Destroy(occupiedTiles[cellPos]);
                occupiedTiles.Remove(cellPos);
            }
        }
    }

    public static void SetCurrentObject(ObjectSO objectData)
    {
        currentObject = objectData;
        currentSprite = objectData.sprite;
    }


}
