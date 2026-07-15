using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlowManager : MonoBehaviour
{
    public static SceneFlowManager Instance;
    
    public string playerName;
    public string pendingIp;
    public int pendingPort;

    // Codice del party corrente. Vive qui (oggetto DontDestroyOnLoad) perché deve
    // sopravvivere al passaggio LobbyScene -> GameScene -> LobbyScene: a fine partita
    // il server ci riporta in lobby senza disconnetterci, quindi il party è ancora lo stesso.
    public string partyCode;

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