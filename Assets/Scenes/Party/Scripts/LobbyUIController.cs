using TMPro;
using UnityEngine;
using Mirror;

public class LobbyUIController : MonoBehaviour
{
    public TMP_InputField partyCodeInput;

    public void OnCreateParty()
    {
        Send(IntentMessage.IntentType.CreateParty);
    }

    public void OnJoinParty()
    {
        Send(IntentMessage.IntentType.JoinParty, partyCodeInput.text);
    }

    void Send(IntentMessage.IntentType intent, string code = "")
    {
        NetworkClient.Send(new IntentMessage
        {
            intent = intent,
            partyCode = code
        });
    }
}