using Mirror;
using UnityEngine;

public class ReadyButton : MonoBehaviour
{
    PlayerLobby localPlayer;

    public void SetLocal(PlayerLobby p)
    {
        localPlayer = p;
    }

    public void OnClick()
    {
        if (localPlayer != null)
            localPlayer.CmdSetReady(!localPlayer.isReady);
    }
}