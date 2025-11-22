using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    [SerializeField] private Button playButton;

    void Awake()
    {
        Assert.IsNotNull(playButton, "Play Button is not assigned in the inspector");
    }
    void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        Services.GameService.StartGame();
    }
}
