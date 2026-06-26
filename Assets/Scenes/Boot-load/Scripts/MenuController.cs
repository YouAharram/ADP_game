using Mirror;
using UnityEngine;
using TMPro;

public class MenuController : MonoBehaviour
{
    public TMP_InputField nameInput;
    public PopupController popup;
    public CanvasGroup menuGroup;

    public void OnPlayPressed()
    {
        SendRequest(true);
    }

    public void OnPartyPressed()
    {
        SendRequest(false);
    }

    void SendRequest(bool play)
    {
        if (!NetworkClient.isConnected)
        {
            popup.ShowPopup("Not connected to server");
            return;
        }

        string name = nameInput.text;

        if (string.IsNullOrWhiteSpace(name))
        {
            popup.ShowPopup("Name is required");
            return;
        }

        if (play)
        {
            NetworkClient.Send(new StartGameMessage
            {
                playerName = name
            });
        }
        else
        {
            NetworkClient.Send(new StartPartyMessage
            {
                playerName = name
            });
        }

        if (menuGroup != null)
        {
            menuGroup.interactable = false;
            menuGroup.blocksRaycasts = false;
        }
    }
}