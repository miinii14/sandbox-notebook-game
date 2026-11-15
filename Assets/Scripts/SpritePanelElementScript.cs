using UnityEngine;

public class SpritePanelElementScript : MonoBehaviour
{
    public void SetCurrentObject()
    {
        ObjectSO currentObjectData = GetComponent<ObjectData>().objectData;
        TilemapPlacer.SetCurrentObject(currentObjectData);
    }
}
