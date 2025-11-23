using UnityEngine;
using System;

public class MovesManager : MonoBehaviour
{
    private int totalMoves = 0;
    private int movesRemaining = 0;
    private bool isGameOver = false;

    public int TotalMoves => totalMoves;
    public int MovesRemaining => movesRemaining;
    public bool IsGameOver => isGameOver;

    public Action<int> OnMovesChanged;
    public Action OnGameOverFromMoves;

    public void InitializeMoves(int moves)
    {
        totalMoves = moves;
        movesRemaining = moves;
        isGameOver = false;
        OnMovesChanged?.Invoke(movesRemaining);
    }

    public void DecrementMoves()
    {
        if (movesRemaining > 0)
        {
            movesRemaining--;
            OnMovesChanged?.Invoke(movesRemaining);

            if (movesRemaining == 0)
            {
                EndGame();
            }
        }
    }

    public void ResetMoves()
    {
        movesRemaining = totalMoves;
        isGameOver = false;
        OnMovesChanged?.Invoke(movesRemaining);
    }

    private void EndGame()
    {
        isGameOver = true;
        OnGameOverFromMoves?.Invoke();
    }
}
