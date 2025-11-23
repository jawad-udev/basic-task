using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Difficulty
{
    Easy,     // 2x2 grid = 4 cards, 8 moves
    Medium,   // 2x3 grid = 6 cards, 12 moves
    Expert,   // 3x4 grid = 12 cards, 20 moves
    Hard,     // 4x4 grid = 16 cards, 25 moves
    Insane    // 4x5 or 5x6 grid = 20-30 cards, 35-40 moves
}

public class GameService : MonoBehaviour
{
    private static readonly (int, int, int)[] DifficultyMoveMap = new[]
    {
        (2, 2, 8),      // Easy: 2x2 grid, 8 moves
        (2, 3, 12),     // Medium: 2x3 grid, 12 moves
        (3, 4, 20),     // Expert: 3x4 grid, 20 moves
        (4, 4, 25),     // Hard: 4x4 grid, 25 moves
        (4, 5, 35),     // Insane: 4x5 grid, 35 moves
    };

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void SetGridSize(int sizeX, int sizeY)
    {
        PlayerPrefs.SetInt("GridSizeX", sizeX);
        PlayerPrefs.SetInt("GridSizeY", sizeY);
    }
    public (int, int) GetGridSize()
    {
        int sizeX = PlayerPrefs.GetInt("GridSizeX", 2); // Default to 2x2 (Easy)
        int sizeY = PlayerPrefs.GetInt("GridSizeY", 2);
        return (sizeX, sizeY);
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        int difficultyIndex = (int)difficulty;
        if (difficultyIndex >= 0 && difficultyIndex < DifficultyMoveMap.Length)
        {
            var (sizeX, sizeY, moves) = DifficultyMoveMap[difficultyIndex];
            SetGridSize(sizeX, sizeY);
            PlayerPrefs.SetInt("Moves", moves);
            PlayerPrefs.SetInt("Difficulty", difficultyIndex);
            PlayerPrefs.Save();
        }
    }

    public Difficulty GetDifficulty()
    {
        int difficultyIndex = PlayerPrefs.GetInt("Difficulty", 0); // Default to Easy
        return (Difficulty)Mathf.Clamp(difficultyIndex, 0, DifficultyMoveMap.Length - 1);
    }

    public int GetMovesForGridSize(int sizeX, int sizeY)
    {
        for (int i = 0; i < DifficultyMoveMap.Length; i++)
        {
            if (DifficultyMoveMap[i].Item1 == sizeX && DifficultyMoveMap[i].Item2 == sizeY)
            {
                return DifficultyMoveMap[i].Item3;
            }
        }
        // Default moves based on grid size if not found in map
        int totalCards = sizeX * sizeY;
        return Mathf.Max(totalCards / 2 + 5, 10);
    }

    public int GetMoves()
    {
        var (sizeX, sizeY) = GetGridSize();
        return PlayerPrefs.GetInt("Moves", GetMovesForGridSize(sizeX, sizeY));
    }
}
