using Mirror;
using TMPro;
using UnityEngine;

public class LobbyPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;

    public TMP_Text nameText;

    public override void OnStartClient()
    {
        base.OnStartClient();
        UpdateName(playerName);
    }

    void OnNameChanged(string oldValue, string newValue)
    {
        UpdateName(newValue);
    }

    void UpdateName(string value)
    {
        if (nameText != null)
            nameText.text = value;
    }
}