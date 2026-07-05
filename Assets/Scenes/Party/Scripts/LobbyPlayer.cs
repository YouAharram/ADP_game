using Mirror;
using TMPro;
using UnityEngine;

public class LobbyPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;

    public TMP_Text nameText;

    void Start()
    {
        UpdateName(playerName);
    }

    void OnNameChanged(string oldVal, string newVal)
    {
        UpdateName(newVal);
    }

    void UpdateName(string value)
    {
        if (nameText != null)
            nameText.text = value;
    }
}