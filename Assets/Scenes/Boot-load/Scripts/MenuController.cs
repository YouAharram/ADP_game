using Mirror;
using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_InputField partyCodeInput;
    public PopupController popup;
    public CanvasGroup menuGroup;

    IntentMessage.IntentType pendingIntent;
    string pendingPartyCode;

    // -------------------------
    // PLAY
    // -------------------------
    public void OnPlayPressed()
    {
        StartFlow(IntentMessage.IntentType.QuickMatch);
    }

    // -------------------------
    // OPEN LOBBY
    // -------------------------
    public void OnPartyPressed()
    {
        StartFlow(IntentMessage.IntentType.OpenLobby);
    }



    // -------------------------
    // CORE FLOW
    // -------------------------
    void StartFlow(IntentMessage.IntentType intent)
    {
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            popup.ShowPopup("Insert name");
            return;
        }

        pendingIntent = intent;

        if (!NetworkClient.isConnected)
        {
            NetworkManager.singleton.StartClient();
            NetworkClient.OnConnectedEvent += OnConnected;
        }
        else
        {
            SendIntent();
        }

        SetInteractable(false);
    }

    void OnConnected()
    {
        NetworkClient.OnConnectedEvent -= OnConnected;
        SendIntent();
    }

    void SendIntent()
    {
        NetworkClient.Send(new IntentMessage
        {
            playerName = nameInput.text,
            intent = pendingIntent,
            partyCode = pendingPartyCode
        });

        pendingPartyCode = null;
    }

    void SetInteractable(bool value)
    {
        if (menuGroup == null) return;
        menuGroup.interactable = value;
        menuGroup.blocksRaycasts = value;
    }
}