using System.Collections.Generic;
using UnityEngine;

public class ControlPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject spritePanelElement;
    [SerializeField] private ObjectFactory objectFactory;
    [SerializeField] private TilemapPlacer tilemapPlacer;


    private List<ObjectSO> objects;

    private void Start()
    {
        objects = objectFactory.objectSamples;
    }
    public void UpdateUI()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (ObjectSO obj in objects)
        {
            GameObject spriteElement = Instantiate(spritePanelElement, transform);
            spriteElement.transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>().sprite = obj.sprite;
            spriteElement.transform.gameObject.GetComponent<ObjectData>().objectData = obj;
        }
    }
}
