using System;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI movesText;

    public void UpdateScoreUI(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    public void UpdateComboUI(int combo)
    {
        if (comboText != null)
        {
            if (combo > 0)
            {
                comboText.text = $"Combo: x{combo}";
                comboText.color = Color.black;
            }
            else
            {
                comboText.text = "Combo: x0";
                comboText.color = Color.red;
            }
        }
    }

    public void UpdateMovesUI(int movesRemaining)
    {
        if (movesText != null)
        {
            if (movesRemaining > 5)
            {
                movesText.text = $"Moves: {movesRemaining}";
                movesText.color = Color.black;
            }
            else if (movesRemaining > 0)
            {
                movesText.text = $"Moves: {movesRemaining}";
                movesText.color = Color.yellow;
            }
            else
            {
                movesText.text = "Moves: 0";
                movesText.color = Color.red;
            }
        }
    }
}
