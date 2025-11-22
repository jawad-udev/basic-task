using System;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;

    // public Action<int> OnScoreUpdated;
    // public Action<int> OnComboUpdated;

    // private void Start()
    // {
    //     OnScoreUpdated += UpdateScoreUI;
    //     OnComboUpdated += UpdateComboUI;
    // }

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

    // private void OnDestroy()
    // {
    //     OnScoreUpdated -= UpdateScoreUI;
    //     OnComboUpdated -= UpdateComboUI;
    // }
}
