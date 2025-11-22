using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int currentCombo = 0;
    private int currentScore = 0;
    private const int POINTS_PER_MATCH = 10;
    private const int COMBO_MULTIPLIER = 5;

    public int CurrentCombo => currentCombo;
    public int CurrentScore => currentScore;

    public System.Action<int> OnScoreChanged;
    public System.Action<int> OnComboChanged;

    private void Start()
    {
        currentScore = 0;
        currentCombo = 0;
    }

    public void AddMatchPoints()
    {
        currentCombo++;
        int basePoints = POINTS_PER_MATCH;
        int comboBonus = currentCombo > 1 ? (currentCombo - 1) * COMBO_MULTIPLIER : 0;
        int totalPoints = basePoints + comboBonus;

        currentScore += totalPoints;

        Debug.Log($"Match! Combo: {currentCombo} | Points: {totalPoints} (Base: {basePoints} + Bonus: {comboBonus}) | Total Score: {currentScore}");

        OnScoreChanged?.Invoke(currentScore);
        OnComboChanged?.Invoke(currentCombo);
    }

    public void ResetCombo()
    {
        if (currentCombo > 0)
        {
            Debug.Log($"Combo broken! Final combo: {currentCombo}");
            Services.UserService.UpdateMaxCombo(currentCombo);
            currentCombo = 0;
            OnComboChanged?.Invoke(currentCombo);
        }
    }

    public void FinishGame()
    {
        ResetCombo();
        Services.UserService.AddScore(currentScore);
        Services.UserService.IncrementGamesPlayed();
        Debug.Log($"Game finished! Final score: {currentScore}");
    }

    public void ResetRound()
    {
        currentScore = 0;
        currentCombo = 0;
        OnScoreChanged?.Invoke(currentScore);
        OnComboChanged?.Invoke(currentCombo);
    }
}
