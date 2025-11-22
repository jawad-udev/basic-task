using UnityEngine;

public class CustomGridLayout : MonoBehaviour
{
    [SerializeField] private Vector4 padding = new Vector4(10, 10, 10, 10); // left, top, right, bottom
    [SerializeField] private Vector2 spacing = Vector2.zero;
    [SerializeField] private float cardWidth = 100f;
    [SerializeField] private float cardHeight = 100f;
    [SerializeField] private bool maintainAspectRatio = true;
    [Space(10)]
    [SerializeField] private int columns = 2;
    [SerializeField] private int rows = 2;
    [SerializeField] private bool autoFitCells = true;

    private RectTransform rectTransform;
    private Vector2 cellSize;

    public int Columns => columns;
    public int Rows => rows;

    private void OnEnable()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        RebuildGrid();
    }

    public void SetGridDimensions(int newColumns, int newRows)
    {
        columns = newColumns;
        rows = newRows;
        RebuildGrid();
    }

    public void SetSpacing(float x, float y)
    {
        spacing = new Vector2(x, y);
        RebuildGrid();
    }

    public void SetCellSize(float width, float height)
    {
        cellSize = new Vector2(width, height);
        autoFitCells = false;
        RebuildGrid();
    }

    public void SetAutoFitCells(bool auto)
    {
        autoFitCells = auto;
        RebuildGrid();
    }

    public void SetPadding(float left, float top, float right, float bottom)
    {
        padding = new Vector4(left, top, right, bottom);
        RebuildGrid();
    }

    public void SetCardSize(float width, float height)
    {
        cardWidth = width;
        cardHeight = height;
        RebuildGrid();
    }

    public void SetMaintainAspectRatio(bool maintain)
    {
        maintainAspectRatio = maintain;
        RebuildGrid();
    }

    public void RebuildGrid()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        CalculateGridSize();
        LayoutChildren();
    }

    private void CalculateGridSize()
    {
        if (autoFitCells)
        {
            float containerWidth = rectTransform.rect.width;
            float containerHeight = rectTransform.rect.height;

            // Account for padding
            float usableWidth = containerWidth - padding.x - padding.z; // left - right
            float usableHeight = containerHeight - padding.y - padding.w; // top - bottom

            float cellWidth = (usableWidth - spacing.x * Mathf.Max(0, columns - 1)) / columns;
            float cellHeight = (usableHeight - spacing.y * Mathf.Max(0, rows - 1)) / rows;

            // Maintain aspect ratio (245x400)
            if (maintainAspectRatio)
            {
                float targetRatio = cardWidth / cardHeight;
                float currentRatio = cellWidth / cellHeight;

                if (currentRatio > targetRatio)
                {
                    // Width is too large, constrain by height
                    cellWidth = cellHeight * targetRatio;
                }
                else
                {
                    // Height is too large, constrain by width
                    cellHeight = cellWidth / targetRatio;
                }
            }

            cellSize = new Vector2(cellWidth, cellHeight);
        }
        else
        {
            cellSize = new Vector2(cardWidth, cardHeight);
        }
    }

    private void LayoutChildren()
    {
        int childCount = transform.childCount;
        int index = 0;

        // Calculate total grid size
        float gridWidth = columns * cellSize.x + (columns - 1) * spacing.x;
        float gridHeight = rows * cellSize.y + (rows - 1) * spacing.y;

        // Calculate container dimensions accounting for padding
        float containerWidth = rectTransform.rect.width;
        float containerHeight = rectTransform.rect.height;

        float usableWidth = containerWidth - padding.x - padding.z;
        float usableHeight = containerHeight - padding.y - padding.w;

        // Calculate offset to center the grid within usable area
        float startX = -gridWidth / 2f + (padding.x - padding.z) / 2f;
        float startY = gridHeight / 2f - (padding.y - padding.w) / 2f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (index >= childCount) break;

                Transform child = transform.GetChild(index);
                RectTransform childRect = child.GetComponent<RectTransform>();

                if (childRect != null)
                {
                    // Set anchor to center
                    childRect.anchorMin = new Vector2(0.5f, 0.5f);
                    childRect.anchorMax = new Vector2(0.5f, 0.5f);
                    childRect.pivot = new Vector2(0.5f, 0.5f);

                    float xPos = startX + col * (cellSize.x + spacing.x) + cellSize.x / 2f;
                    float yPos = startY - row * (cellSize.y + spacing.y) - cellSize.y / 2f;

                    childRect.anchoredPosition = new Vector2(xPos, yPos);
                    childRect.sizeDelta = cellSize;

                    Debug.Log($"Card {index} positioned at ({xPos}, {yPos}) with size {cellSize}");
                }

                index++;
            }
        }
    }
}
