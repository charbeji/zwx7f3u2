using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridAutoResizer : MonoBehaviour
{
    private GridLayoutGroup grid;
    private RectTransform rect;
    public int rows = 2;
    public int columns = 2;

    [Range(0f, 0.5f)]
    public float spacingRatio = 0.05f; // % of space used for spacing

    private void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
        rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        Resize();
    }

    private void OnRectTransformDimensionsChange()
    {
        if (grid != null && rect != null)
            Resize();
    }

    public void Resize()
    {
        if (grid == null || rect == null)
            return; 

        float width = rect.rect.width;
        float height = rect.rect.height;

        if (width <= 0 || height <= 0)
            return;

        float spacingX = width * spacingRatio;
        float spacingY = height * spacingRatio;

        grid.spacing = new Vector2(spacingX, spacingY);

        float availableWidth = width - (spacingX * (columns - 1));
        float availableHeight = height - (spacingY * (rows - 1));

        float cellWidth = availableWidth / columns;
        float cellHeight = availableHeight / rows;

        // Pick the smaller for perfect fitting
        float finalSize = Mathf.Min(cellWidth, cellHeight, 450f); // max cell size
        grid.cellSize = new Vector2(finalSize, finalSize);

    }

    // Call this when you generate a new grid (after rows/columns change)
    public void SetGrid(int r, int c)
    {
        rows = r;
        columns = c;
        Resize();
    }
}
