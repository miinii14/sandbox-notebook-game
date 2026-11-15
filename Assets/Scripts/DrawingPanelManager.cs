using UnityEngine;
using UnityEngine.UI;

public class DrawingPanelManager : MonoBehaviour 
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private DrawingLineSO drawingLine;
    [SerializeField] private float pointOffset;

    [SerializeField] private ObjectFactory objectFactory;    
    
    private Texture2D texture;
    private RectTransform rectTransform;
    private static Color selectedColor;
    private Color clearColor;
    private Vector2? lastMousePosition;
    public static string currentType;
    
    private void Start()
    {
        drawingLine.color = Color.black;
        drawingLine.selectedColor = Color.black;
        drawingLine.size = 10;
        clearColor = Color.clear;
        rectTransform = rawImage.GetComponent<RectTransform>();
        texture = new Texture2D((int)rectTransform.rect.width, (int)rectTransform.rect.height, TextureFormat.RGBA32, true);
        texture.filterMode = FilterMode.Trilinear;
        texture.Apply();
        
        rawImage.texture = texture;
        
        ClearTexture();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, Camera.main, out mousePosition);
            mousePosition.x += rectTransform.rect.width / 2;
            mousePosition.y += rectTransform.rect.height / 2;

            if (mousePosition.x >= 0 && mousePosition.y >= 0 && mousePosition.x < rectTransform.rect.width &&
                mousePosition.y < rectTransform.rect.height)
            {
                if (lastMousePosition == null)
                {
                    DrawCircle((int)mousePosition.x, (int)mousePosition.y);
                }
                else if (Vector2.Distance((Vector2)lastMousePosition, mousePosition) >= pointOffset)
                {
                    DrawLine((Vector2)lastMousePosition, mousePosition);
                }
                
                lastMousePosition = mousePosition;
            }
            else
            {
                lastMousePosition = null;
            }
            
        }

        if (Input.GetMouseButtonUp(0))
        {
            lastMousePosition = null;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            drawingLine.ToggleEraserMode();
        }
    }

    private void DrawCircle(int x, int y)
    {
        for (int i = -drawingLine.size; i < drawingLine.size; i++)
        {
            for (int j = -drawingLine.size; j < drawingLine.size; j++)
            {
                if (i * i + j * j <= drawingLine.size * drawingLine.size)
                {
                    int pixelX = x + i;
                    int pixelY = y + j;
                    
                    if (pixelX >= 0 && pixelX < texture.width && pixelY >= 0 && pixelY < texture.height) {
                        texture.SetPixel(pixelX, pixelY, drawingLine.color);
                    }
                }
            }
        }
        
    }

    private void DrawLine(Vector2 start, Vector2 end)
    {
        float distance = Vector2.Distance(start, end);
        Vector2 direction = (end - start).normalized;

        for (float i = 0; i < distance; i += 5f)  { 
            Vector2 position = start + direction * i;
            DrawCircle((int)position.x, (int)position.y);
        }
        
        texture.Apply();
    }

    private void ClearTexture()
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, clearColor);
            }
        }
        
        texture.Apply();
    }

    public void CreateObject()
    {
        Texture2D textureCopy = FilterTexture(texture);

        currentType = currentType ?? "PhysicalObject";

        objectFactory.InitializeObject(textureCopy, currentType);

        ClearTexture();
    }

    private Texture2D FilterTexture(Texture2D source)
    {
        int width = source.width;
        int height = source.height;

        int xMin = width, xMax = 0, yMin = height, yMax = 0;

        Color[] pixels = source.GetPixels();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixel = pixels[y * width + x];
                if (pixel.a != 0)
                {
                    if (x < xMin) xMin = x;
                    if (x > xMax) xMax = x;
                    if (y < yMin) yMin = y;
                    if (y > yMax) yMax = y;
                }
            }
        }

        int newWidth = xMax - xMin + 1;
        int newHeight = yMax - yMin + 1;

        Color[] newPixels = source.GetPixels(xMin, yMin, newWidth, newHeight);

        Texture2D croppedTexture = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, true);
        croppedTexture.SetPixels(newPixels);
        croppedTexture.Apply();

        return croppedTexture;
    }

    public void setType(string type)
    {
        currentType = type;
    }
}
