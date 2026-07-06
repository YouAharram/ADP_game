using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName = "Player";

    // Variabile sincronizzata per lo stato di Ready
    [SyncVar(hook = nameof(OnReadyChanged))]
    public bool isReady = false;

    // Usiamo solo questo testo sia per il nome che per il colore dello stato
    public TMP_Text nameText; 

    private void Start()
    {
        UpdateUI();

        // Se questo è il mio giocatore locale, collego il bottone Ready della scena
        if (isLocalPlayer)
        {
            SetupReadyButton();
        }
    }

    void SetupReadyButton()
    {
        GameObject readyBtnObj = GameObject.Find("ReadyButton");
        if (readyBtnObj != null)
        {
            Button readyButton = readyBtnObj.GetComponent<Button>();
            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(ToggleReady);
            Debug.Log("[CLIENT] Bottone Ready collegato al player locale.");
        }
        else
        {
            Debug.LogWarning("[CLIENT] Bottone 'ReadyButton' non trovato nella scena!");
        }
    }

    public void ToggleReady()
    {
        if (!isLocalPlayer) return;
        CmdToggleReady(!isReady);
    }

    [Command]
    void CmdToggleReady(bool newReadyState)
    {
        isReady = newReadyState;
        
        if (NetworkManager.singleton is MyNetworkManager myNetManager)
        {
            myNetManager.CheckIfAllReady();
        }
    }

    // --- HOOKS DI SINCRONIZZAZIONE UI ---

    void OnNameChanged(string oldName, string newName) => UpdateUI();
    void OnReadyChanged(bool oldReady, bool newReady) => UpdateUI();

    void UpdateUI()
    {
        if (nameText != null)
        {
            // Impostiamo il testo con il nome del giocatore
            nameText.text = playerName;

            // Cambiamo semplicemente il colore del testo in base allo stato
            if (isReady)
            {
                nameText.color = Color.green; // Verde se pronto
            }
            else
            {
                nameText.color = Color.white; // Bianco (o il tuo colore di default) se NON pronto
            }
        }
    }
}