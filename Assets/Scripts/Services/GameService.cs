using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameService : MonoBehaviour
{
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
        int sizeX = PlayerPrefs.GetInt("GridSizeX", 4); // Default to 4 if not set
        int sizeY = PlayerPrefs.GetInt("GridSizeY", 4); // Default to 4 if not set
        return (sizeX, sizeY);
    }
}
