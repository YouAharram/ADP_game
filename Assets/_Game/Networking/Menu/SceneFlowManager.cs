using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlowManager : MonoBehaviour
{
    public static SceneFlowManager Instance;
    
    public string playerName;
    public string pendingIp;
    public int pendingPort;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GoToLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void GoToGame(string ip, int port)
    {
        pendingIp = ip;
        pendingPort = port;
        SceneManager.LoadScene("GameScene");
    }
}