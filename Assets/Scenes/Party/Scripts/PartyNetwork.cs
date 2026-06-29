using Mirror;
using TMPro;
using UnityEngine;

public class PartyNetwork : NetworkBehaviour
{
    public static PartyNetwork Instance;

    [SyncVar(hook = nameof(OnCodeChanged))]
    public string partyCode;

    public TMP_Text partyCodeText; // <-- DAL CANVAS

    public override void OnStartClient()
    {
        Instance = this;
    }

    void OnCodeChanged(string oldValue, string newValue)
    {
        if (partyCodeText != null)
            partyCodeText.text = "Party Code: " + newValue;
    }
}