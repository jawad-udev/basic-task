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

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundSource;
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
}
