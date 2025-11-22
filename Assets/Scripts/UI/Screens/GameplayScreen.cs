using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayScreen : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private CustomGridLayout customGridLayout;
    [SerializeField] private TMP_Text gameStatusText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private ScoreUI scoreUI;
    [SerializeField] private ScoreManager scoreManager;

    // Grid size configuration
    [SerializeField] private int gridRows = 2;
    [SerializeField] private int gridColumns = 2;
    [SerializeField] private float spacingX = 10f;
    [SerializeField] private float spacingY = 10f;
    [SerializeField] private float paddingLeft = 20f;
    [SerializeField] private float paddingTop = 20f;
    [SerializeField] private float paddingRight = 20f;
    [SerializeField] private float paddingBottom = 20f;
    [SerializeField] private float cardWidth = 245f;
    [SerializeField] private float cardHeight = 400f;

    private void Start()
    {
        if (gridManager == null)
        {
            gridManager = GetComponent<GridManager>();
        }

        if (customGridLayout == null && gridManager != null)
        {
            customGridLayout = gridManager.GetComponent<CustomGridLayout>();
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        if (scoreUI != null && scoreManager != null)
        {
            Debug.Log("Linking ScoreUI and ScoreManager events");
            scoreManager.OnScoreChanged += scoreUI.UpdateScoreUI;
            scoreManager.OnComboChanged += scoreUI.UpdateComboUI;
        }
        InitializeGame();
    }

    private void InitializeGame()
    {
        // Configure custom grid layout
        if (customGridLayout != null)
        {
            customGridLayout.SetGridDimensions(gridColumns, gridRows);
            customGridLayout.SetSpacing(spacingX, spacingY);
            customGridLayout.SetPadding(paddingLeft, paddingTop, paddingRight, paddingBottom);
            customGridLayout.SetCardSize(cardWidth, cardHeight);
            customGridLayout.SetAutoFitCells(true);
            customGridLayout.SetMaintainAspectRatio(true);
        }

        // Initialize grid with specified dimensions
        gridManager.InitializeGrid(gridRows, gridColumns);
        gridManager.OnGameWon += OnGameWon;

        if (gameStatusText != null)
        {
            gameStatusText.text = $"Grid: {gridRows}x{gridColumns}";
        }
    }

    public void SetGridSize(int rows, int columns)
    {
        gridRows = rows;
        gridColumns = columns;

        // Update grid layout
        if (customGridLayout != null)
        {
            customGridLayout.SetGridDimensions(columns, rows);
        }

        // Initialize new grid
        gridManager.InitializeGrid(rows, columns);

        if (gameStatusText != null)
        {
            gameStatusText.text = $"Grid: {rows}x{columns}";
        }
    }

    private void OnGameWon()
    {
        if (gameStatusText != null)
        {
            gameStatusText.text = "You Won!";
        }
        Services.AudioService.PlayWinSound();
    }

    private void RestartGame()
    {
        // Reset grid layout
        if (customGridLayout != null)
        {
            customGridLayout.SetGridDimensions(gridColumns, gridRows);
            customGridLayout.SetSpacing(spacingX, spacingY);
            customGridLayout.SetPadding(paddingLeft, paddingTop, paddingRight, paddingBottom);
            customGridLayout.SetCardSize(cardWidth, cardHeight);
        }

        // Reinitialize grid
        gridManager.InitializeGrid(gridRows, gridColumns);

        if (gameStatusText != null)
        {
            gameStatusText.text = $"Grid: {gridRows}x{gridColumns}";
        }
    }

    private void OnDestroy()
    {
        if (gridManager != null)
        {
            gridManager.OnGameWon -= OnGameWon;
        }
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartGame);
        }
        if (scoreUI != null && scoreManager != null)
        {
            scoreManager.OnScoreChanged -= scoreUI.UpdateScoreUI;
            scoreManager.OnComboChanged -= scoreUI.UpdateComboUI;
        }
    }
}
