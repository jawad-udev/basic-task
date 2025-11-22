using UnityEngine;
using System;

[System.Serializable]
public class UserProfile
{
    public string username = "";
    public string email = "";
    public string createdDate;

    public UserProfile()
    {
        createdDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class GameStats
{
    public int totalScore = 0;
    public int maxCombo = 0;
    public int gamesPlayed = 0;
    public int totalMatches = 0;
    public float totalPlayTime = 0f;
    public string lastUpdated;

    public GameStats()
    {
        lastUpdated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class UserData
{
    public UserProfile profile;
    public GameStats stats;

    public UserData()
    {
        profile = new UserProfile();
        stats = new GameStats();
    }
}

public class UserService : MonoBehaviour
{
    private UserData userData;
    private const string USER_DATA_KEY = "UserData";

    public UserData UserData => userData;
    public UserProfile Profile => userData.profile;
    public GameStats Stats => userData.stats;

    private void Awake()
    {
        LoadUserData();
    }

    public void LoadUserData()
    {
        string json = PlayerPrefs.GetString(USER_DATA_KEY, "");
        if (string.IsNullOrEmpty(json))
        {
            userData = new UserData();
            // Create a dummy user
            CreateDummyUser();
            SaveUserData();
            Debug.Log("No user data found, creating dummy user profile");
        }
        else
        {
            userData = JsonUtility.FromJson<UserData>(json);
            Debug.Log("User data loaded successfully");
        }
    }

    private void CreateDummyUser()
    {
        userData.profile.username = "Player_" + UnityEngine.Random.Range(1000, 9999);
        userData.profile.email = userData.profile.username + "@cardsGame.local";
        Debug.Log($"Dummy user created - Username: {userData.profile.username}, Email: {userData.profile.email}");
    }

    public void SaveUserData()
    {
        userData.stats.lastUpdated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string json = JsonUtility.ToJson(userData, true);
        PlayerPrefs.SetString(USER_DATA_KEY, json);
        PlayerPrefs.Save();
        Debug.Log("User data saved to PlayerPrefs");
    }

    public void SetUserProfile(string username, string email)
    {
        userData.profile.username = username;
        userData.profile.email = email;
        SaveUserData();
        Debug.Log($"User profile set - Username: {username}, Email: {email}");
    }

    public void AddScore(int points)
    {
        userData.stats.totalScore += points;
        SaveUserData();
        Debug.Log($"Score added: {points}. Total score: {userData.stats.totalScore}");
    }

    public void UpdateMaxCombo(int currentCombo)
    {
        if (currentCombo > userData.stats.maxCombo)
        {
            userData.stats.maxCombo = currentCombo;
            SaveUserData();
            Debug.Log($"New max combo: {userData.stats.maxCombo}");
        }
    }

    public void AddMatch()
    {
        userData.stats.totalMatches++;
        SaveUserData();
    }

    public void IncrementGamesPlayed()
    {
        userData.stats.gamesPlayed++;
        SaveUserData();
    }

    public void AddPlayTime(float seconds)
    {
        userData.stats.totalPlayTime += seconds;
        SaveUserData();
    }

    public void ResetGameStats()
    {
        userData.stats = new GameStats();
        SaveUserData();
        Debug.Log("Game stats reset");
    }

    public void ResetAllUserData()
    {
        userData = new UserData();
        PlayerPrefs.DeleteKey(USER_DATA_KEY);
        PlayerPrefs.Save();
        Debug.Log("All user data reset");
    }

    public string GetUserDataAsJson()
    {
        return JsonUtility.ToJson(userData, true);
    }
}
