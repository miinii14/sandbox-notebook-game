using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeManager : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Canvas drawingCanvas;
    [SerializeField] private Canvas tilemapCanvas;
    [SerializeField] private GameObject newModelPanel;
    [SerializeField] private GameObject tilemapPlacer;
    [SerializeField] private ObjectFactory objectFactory;
    [SerializeField] WalkableGridGenerator walkableGridGenerator;

    private bool isSimulation = false;
    
    private void Start()
    {
        tilemapCanvas.gameObject.SetActive(true);
        grid.gameObject.SetActive(true);
        drawingCanvas.gameObject.SetActive(false);
        tilemapPlacer.SetActive(true);
        newModelPanel.SetActive(false);
    }    

    public void NewModelPanel()
    {
        newModelPanel.SetActive(true);
        tilemapPlacer.SetActive(false);
    }

    public void DrawingCanvas()
    {
        drawingCanvas.gameObject.SetActive(true);
        tilemapCanvas.gameObject.SetActive(false);
        grid.gameObject.SetActive(false);
        tilemapPlacer.SetActive(false);
        newModelPanel.SetActive(false);
    }

    public void TilemapCanvas()
    {
        drawingCanvas.gameObject.SetActive(false);
        tilemapCanvas.gameObject.SetActive(true);
        grid.gameObject.SetActive(true);
        tilemapPlacer.SetActive(true);
        newModelPanel.SetActive(false);
    }

    public void ToggleSimulationMode()
    {
        tilemapCanvas.gameObject.SetActive(!tilemapCanvas.gameObject.activeSelf);
        tilemapPlacer.SetActive(!tilemapCanvas.gameObject.activeSelf);
        newModelPanel.SetActive(false);
        tilemapPlacer.gameObject.SetActive(!tilemapPlacer.gameObject.activeSelf);

        ToggleObjectStates(objectFactory.objects);

        Text text = GameObject.Find("StartStopSimulation").GetComponentInChildren<Text>();
        text.text = isSimulation ? "Start" : "Stop";
        isSimulation = !isSimulation;

        walkableGridGenerator.GenerateWalkableGrid();
    }

    private void ToggleObjectStates(List<GameObject> objects)
    {
        foreach (GameObject obj in objects)
        {
            ObjectData data = obj.transform.Find("ObjectData").GetComponent<ObjectData>();
            if(data.objectData.type == "Player")
            {
                PlayerController controller = obj.GetComponent<PlayerController>();
                controller.enabled = !controller.enabled; 
            }
            if(data.objectData.type == "NPC")
            {
                NPCController controller = obj.GetComponent<NPCController>();
                controller.enabled = !controller.enabled;
                controller.Init();
            }
        }
    }
}
