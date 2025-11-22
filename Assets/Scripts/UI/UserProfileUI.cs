using TMPro;
using UnityEngine;

public class UserProfileUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI emailText;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI maxComboText;
    [SerializeField] private TextMeshProUGUI gamesPlayedText;
    [SerializeField] private TextMeshProUGUI totalMatchesText;

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        UserService userService = Services.UserService;
        UserProfile profile = userService.Profile;
        GameStats stats = userService.Stats;

        if (usernameText != null)
        {
            usernameText.text = $"Username: {(string.IsNullOrEmpty(profile.username) ? "Not Set" : profile.username)}";
        }

        if (emailText != null)
        {
            emailText.text = $"Email: {(string.IsNullOrEmpty(profile.email) ? "Not Set" : profile.email)}";
        }

        if (totalScoreText != null)
        {
            totalScoreText.text = $"Total Score: {stats.totalScore}";
        }

        if (maxComboText != null)
        {
            maxComboText.text = $"Max Combo: x{stats.maxCombo}";
        }

        if (gamesPlayedText != null)
        {
            gamesPlayedText.text = $"Games Played: {stats.gamesPlayed}";
        }

        if (totalMatchesText != null)
        {
            totalMatchesText.text = $"Total Matches: {stats.totalMatches}";
        }

        Debug.Log("User profile UI updated");
    }

    public void SetUserProfile(string username, string email)
    {
        Services.UserService.SetUserProfile(username, email);
        UpdateUI();
    }
}
