using System;
using UnityEngine;
public enum AudioType
{
    Sound,
    Music
}
public enum SoundType
{
    Button,
    BackgroundMusic
}
public enum MusicType
{
    MainMenu,
    InGame
}
public class AudioService : MonoBehaviour
{

    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private AudioClip bgMusic;
    [SerializeField] private AudioClip cardMatchSound;
    [SerializeField] private AudioClip cardMissmatchSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip gameoverSound;
    [SerializeField] private AudioClip cardFlipSound;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundSource;

    private bool isMusicEnabled = true;
    private bool isSoundEnabled = true;

    public bool IsMusicEnabled => isMusicEnabled;
    public bool IsSoundEnabled => isSoundEnabled;

    private void Awake()
    {
        LoadAudioPreferences();
    }

    private void LoadAudioPreferences()
    {
        isMusicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        isSoundEnabled = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;

        musicSource.mute = !isMusicEnabled;
        soundSource.mute = !isSoundEnabled;
    }

    public void ToggleMusic()
    {
        isMusicEnabled = !isMusicEnabled;
        musicSource.mute = !isMusicEnabled;
        PlayerPrefs.SetInt("MusicEnabled", isMusicEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleSound()
    {
        isSoundEnabled = !isSoundEnabled;
        soundSource.mute = !isSoundEnabled;
        PlayerPrefs.SetInt("SoundEnabled", isSoundEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void PlayButtonSound(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.Button:
                soundSource.PlayOneShot(buttonSound);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(soundType), soundType, null);
        }
    }

    public void PlayMusic(MusicType musicType)
    {
        switch (musicType)
        {
            case MusicType.MainMenu:
                musicSource.clip = bgMusic;
                musicSource.Play();
                break;
            case MusicType.InGame:
                // Add in-game music logic here
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(musicType), musicType, null);
        }
    }
    public void PlayMatchSound()
    {
        soundSource.PlayOneShot(cardMatchSound);
    }
    public void PlayMissmatchSound()
    {
        soundSource.PlayOneShot(cardMissmatchSound);
    }
    public void PlayWinSound()
    {
        soundSource.PlayOneShot(winSound);
    }
    public void PlayGameOverSound()
    {
        soundSource.PlayOneShot(gameoverSound);
    }

    public void PlayCardFlipSound()
    {
        soundSource.PlayOneShot(cardFlipSound);
    }
}
