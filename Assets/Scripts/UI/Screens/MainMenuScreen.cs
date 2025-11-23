using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private ToggleGroup gridSizeToggleGroup;
    [SerializeField] private TMP_Text profileData;
    [SerializeField] private Button musicToggleButton;
    [SerializeField] private TMP_Text musicToggleButtonText;
    [SerializeField] private Button soundToggleButton;
    [SerializeField] private TMP_Text soundToggleButtonText;

    // Grid size presets: (rows, columns)
    private static readonly (int, int)[] GridSizePresets = new[]
    {
        (2, 2),   // Easy
        (2, 3),   // Medium
        (3, 4),   // Expert
        (4, 4),   // Expert
        (4, 5),   // Expert
        (5, 6),   // Expert
    };

    void Awake()
    {
        Assert.IsNotNull(playButton, "Play Button is not assigned in the inspector");
        Assert.IsNotNull(gridSizeToggleGroup, "Grid Size Toggle Group is not assigned in the inspector");
    }

    void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);

        // Set default grid size to 2x2
        Services.GameService.SetGridSize(2, 2);

        // Set up toggle listeners for grid size selection
        if (gridSizeToggleGroup != null)
        {
            Toggle[] toggles = gridSizeToggleGroup.GetComponentsInChildren<Toggle>();
            for (int i = 0; i < toggles.Length && i < GridSizePresets.Length; i++)
            {
                int index = i; // Capture for closure
                toggles[i].onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                    {
                        OnGridSizeToggleChanged(index);
                    }
                });
            }

            // Set first toggle as selected by default (2x2)
            if (toggles.Length > 0)
            {
                toggles[0].isOn = true;
            }
        }

        // Set up music and sound toggle buttons
        if (musicToggleButton != null)
        {
            musicToggleButton.onClick.AddListener(OnMusicToggleClicked);
        }

        if (soundToggleButton != null)
        {
            soundToggleButton.onClick.AddListener(OnSoundToggleClicked);
        }

        // Display player data in profile data text
        UpdateProfileData();
        UpdateAudioToggleButtons();
    }

    private void UpdateProfileData()
    {
        if (profileData == null)
            return;

        UserService userService = Services.UserService;
        UserProfile profile = userService.Profile;
        GameStats stats = userService.Stats;

        string profileText = $"Player: {profile.username}\n" +
                           $"Total Score: {stats.totalScore}\n" +
                           $"Games Played: {stats.gamesPlayed}\n" +
                           $"Max Combo: x{stats.maxCombo}";

        profileData.text = profileText;
    }

    private void OnGridSizeToggleChanged(int presetIndex)
    {
        if (presetIndex >= 0 && presetIndex < GridSizePresets.Length)
        {
            var (rows, columns) = GridSizePresets[presetIndex];
            Services.GameService.SetGridSize(columns, rows);
        }
    }

    private void OnPlayButtonClicked()
    {
        Services.GameService.StartGame();
    }

    private void OnMusicToggleClicked()
    {
        Services.AudioService.ToggleMusic();
        UpdateAudioToggleButtons();
    }

    private void OnSoundToggleClicked()
    {
        Services.AudioService.ToggleSound();
        UpdateAudioToggleButtons();
    }

    private void UpdateAudioToggleButtons()
    {
        if (musicToggleButtonText != null)
        {
            musicToggleButtonText.text = Services.AudioService.IsMusicEnabled ? "Music: ON" : "Music: OFF";
        }

        if (soundToggleButtonText != null)
        {
            soundToggleButtonText.text = Services.AudioService.IsSoundEnabled ? "Sound: ON" : "Sound: OFF";
        }
    }

    private void OnDestroy()
    {
        if (gridSizeToggleGroup != null)
        {
            Toggle[] toggles = gridSizeToggleGroup.GetComponentsInChildren<Toggle>();
            foreach (Toggle toggle in toggles)
            {
                toggle.onValueChanged.RemoveListener(_ => { });
            }
        }
        playButton.onClick.RemoveListener(OnPlayButtonClicked);
        if (musicToggleButton != null)
        {
            musicToggleButton.onClick.RemoveListener(OnMusicToggleClicked);
        }
        if (soundToggleButton != null)
        {
            soundToggleButton.onClick.RemoveListener(OnSoundToggleClicked);
        }
    }
}
