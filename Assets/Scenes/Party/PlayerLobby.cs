using Mirror;

public class PlayerLobby : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;

    [SyncVar(hook = nameof(OnReadyChanged))]
    public bool isReady;

    public override void OnStartClient()
    {
        LobbyManager.Instance.Register(this);
    }

    public override void OnStopClient()
    {
        if (LobbyManager.Instance != null)
            LobbyManager.Instance.Unregister(this);
    }

    void OnNameChanged(string oldValue, string newValue)
    {
        LobbyManager.Instance?.Refresh();
    }

    void OnReadyChanged(bool oldValue, bool newValue)
    {
        LobbyManager.Instance?.Refresh();
    }

    [Command]
    public void CmdSetReady(bool ready)
    {
        isReady = ready;
    }
}