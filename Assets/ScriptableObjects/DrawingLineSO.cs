using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

[CreateAssetMenu(fileName = "DrawingLineSO", menuName = "Scriptable Objects/DrawingLineSO")]
public class DrawingLineSO : ScriptableObject
{
    public Color color = Color.black;
    public Color selectedColor = Color.black;
    public int size = 10;

    public void ChangeColorBlack()
    {
        this.color = Color.black;
        this.selectedColor = Color.black;
    }

    public void ChangeColorRed()
    {
        this.color = Color.red;
        this.selectedColor = Color.red;
    }

    public void ChangeColorBlue()
    {
        this.color = Color.blue;
        this.selectedColor = Color.blue;
    }
    public void ChangeColorGreen()
    {
        this.color = Color.green;
        this.selectedColor = Color.green;
    }
    public void ChangeColorYellow()
    {
        this.color = Color.yellow;
        this.selectedColor = Color.yellow;
    }
    public void ChangeColorBrown()
    {
        this.color = new Color32(165, 42, 42, 255);
        this.selectedColor = new Color32(165, 42, 42, 255); 
    }



    public void ToggleEraserMode()
    {
        this.color = this.color == Color.clear ? this.selectedColor : Color.clear;
    }
    
    public void ChangeSizeSmall()
    {
        this.size = 10;
    }

    public void ChangeSizeBig()
    {
        this.size = 25;
    }
    
}
