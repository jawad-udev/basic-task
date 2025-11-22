using System;
using UnityEngine;
using UnityEngine.UI;

public class PlaySound : MonoBehaviour
{
    [SerializeField] private AudioType audioType;
    [SerializeField] private SoundType soundType;
    private Button button;
    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(Play);
        }
    }
    public void Play()
    {
        switch (audioType)
        {
            case AudioType.Sound:
                Services.AudioService.PlayButtonSound(soundType);
                break;
            case AudioType.Music:
                //Services.AudioService.PlayMusic(musicType);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
