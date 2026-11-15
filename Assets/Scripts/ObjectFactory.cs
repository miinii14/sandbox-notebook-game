using System.Collections.Generic;
using UnityEngine;

public class ObjectFactory : MonoBehaviour
{
    [SerializeField] private ControlPanelManager controlPanelManager;

    public List<ObjectSO> objectSamples;
    public List<GameObject> objects;
    public void AddSample(ObjectSO sample)
    {
        objectSamples.Add(sample);
    }

    public void RemoveSample(ObjectSO sample)
    {
        objectSamples.Remove(sample);
    }
    
    public void InitializeObject(Texture2D texture, string type)
    {
        ObjectSO obj = ScriptableObject.CreateInstance<ObjectSO>();
        obj.sprite = ConvertToSprite(texture);
        obj.type = type;
            
        AddSample(obj);
        controlPanelManager.UpdateUI();
    }
    
    private static Sprite ConvertToSprite(Texture2D texture)
    {
        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f));
    }
}
