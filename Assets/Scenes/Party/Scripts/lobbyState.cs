using Mirror;
using UnityEngine;

public class LobbyState : NetworkBehaviour
{
    [SyncVar] public bool isReady;

    [Command]
    public void CmdSetReady(bool value)
    {
        isReady = value;
    }
}