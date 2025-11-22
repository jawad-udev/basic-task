using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayScreen : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup cardGrid;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private TMP_Text gameStatusText;
    [SerializeField] private Button restartButton;

    // Grid size configuration
    [SerializeField] private int gridRows = 2;
    [SerializeField] private int gridColumns = 2;

    private void Start()
    {
        if (gridManager == null)
        {
            gridManager = GetComponent<GridManager>();
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        InitializeGame();
    }

    private void InitializeGame()
    {
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
    }

    private void RestartGame()
    {
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
    }
}
