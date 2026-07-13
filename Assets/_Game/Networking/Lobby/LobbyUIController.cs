using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Mirror;
using kcp2k;

public class LobbyUIController : MonoBehaviour
{
    public TMP_InputField partyCodeInput;
    public TMP_Text partyCodeText;
    public PopupController popup;

    // --- AGGIUNTO: Riferimenti ai pannelli UI per il reset ---
    public GameObject lobbyPanel; // Il pannello che mostra il codice e la lista giocatori
    public GameObject menuPanel;  // Il pannello iniziale con i tasti Crea/Partecipa

    const string baseUrl = "http://127.0.0.1:8000";
    string currentCode;

    [System.Serializable]
    class PartyResponse
    {
        public string code;
        public string ip;
        public int port;
    }

    public void OnCreateParty() => StartCoroutine(CreateParty());

    IEnumerator CreateParty()
    {
        string json = "{\"playerName\":\"" + SceneFlowManager.Instance.playerName + "\"}";
        var req = BuildRequest("/createParty", json);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            popup.ShowPopup("Error creating party");
            yield break;
        }

        var res = JsonUtility.FromJson<PartyResponse>(req.downloadHandler.text);
        currentCode = res.code;
        partyCodeText.text = "CODE: " + currentCode;

        yield return new WaitForSeconds(1.0f);
        ConnectToMirrorServer(res.ip, (ushort)res.port);
    }

    public void OnJoinParty() => StartCoroutine(JoinParty());

    IEnumerator JoinParty()
    {
        string code = partyCodeInput.text;
        string json = "{\"playerName\":\"" + SceneFlowManager.Instance.playerName + "\",\"code\":\"" + code + "\"}";
        var req = BuildRequest("/joinParty", json);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            popup.ShowPopup("Party not found");
            yield break;
        }

        var res = JsonUtility.FromJson<PartyResponse>(req.downloadHandler.text);
        currentCode = res.code;
        partyCodeText.text = "CODE: " + currentCode;

        ConnectToMirrorServer(res.ip, (ushort)res.port);
    }

    void ConnectToMirrorServer(string ip, ushort port)
    {
        NetworkManager.singleton.networkAddress = ip;

        if (Transport.active is KcpTransport kcp)
        {
            kcp.Port = port;
            Debug.Log($"[CLIENT] Configurazione KCP completata su {ip}:{port}");
        }

        NetworkManager.singleton.StartClient();

        // Attiviamo il pannello della lobby e nascondiamo il menu principale
        if (lobbyPanel != null) lobbyPanel.SetActive(true);
        if (menuPanel != null) menuPanel.SetActive(false);
    }

    // --- NUOVA FUNZIONE: AGGIUNTA PER ABBANDONARE IL PARTY ---
    public void OnLeaveParty()
    {
        Debug.Log("[CLIENT] Abbandono del party in corso...");

        // Se siamo connessi come client a Mirror, stoppiamo la connessione
        if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }

        // Resettiamo la UI locale del giocatore
        partyCodeText.text = "CODE: ----";
        currentCode = "";
        partyCodeInput.text = "";

        if (lobbyPanel != null) lobbyPanel.SetActive(false);
        if (menuPanel != null) menuPanel.SetActive(true);
    }

    UnityWebRequest BuildRequest(string endpoint, string json)
    {
        var req = new UnityWebRequest(baseUrl + endpoint, "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        return req;
    }
}
