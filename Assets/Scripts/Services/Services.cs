using UnityEngine;

public class Services : SingletonMonobehaviour<Services>
{
    [SerializeField] private UIService uiService;
    [SerializeField] private GameService gameService;
    [SerializeField] private UserService userService;
    [SerializeField] private AudioService audioService;


    public static UIService UIService => instance.uiService;
    public static GameService GameService => instance.gameService;
    public static UserService UserService => instance.userService;
    public static AudioService AudioService => instance.audioService;
}
