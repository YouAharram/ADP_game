using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public TMP_InputField nameInput;
    public PopupController popup;

    public void OnPlayPressed()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            popup.ShowPopup("Insert name");
            return;
        }

        SceneFlowManager.Instance.playerName = nameInput.text;

        // TODO: QuickMatch dopo
        Debug.Log("QuickMatch not implemented");
    }

    public void OnPartyPressed()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            popup.ShowPopup("Insert name");
            return;
        }

        SceneFlowManager.Instance.playerName = nameInput.text;
        SceneFlowManager.Instance.GoToLobby();
    }

    // --- NUOVA FUNZIONE: AZIONE PER IL TASTO QUIT ---
    public void OnQuitPressed()
    {
        Debug.Log("[MENU] Chiusura del gioco in corso...");

        #if UNITY_EDITOR
        // Se siamo dentro l'Editor di Unity, fermiamo la modalità Play
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // Se siamo nella build finale del gioco, chiudiamo l'applicazione eseguibile
        Application.Quit();
        #endif
    }
}