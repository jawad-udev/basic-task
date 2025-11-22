using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private ToggleGroup gridSizeToggleGroup;

    // Grid size presets: (rows, columns)
    private static readonly (int, int)[] GridSizePresets = new[]
    {
        (2, 2),   // Easy
        (2, 3),   // Medium
        (3, 3),   // Hard
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
    }
}
