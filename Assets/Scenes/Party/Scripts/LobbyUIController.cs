    using System.Collections;
    using System.Text;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Networking;
    using Mirror; 
    using kcp2k; // <--- AGGIUNTO: Necessario per configurare KcpTransport via codice

    public class LobbyUIController : MonoBehaviour
    {
        public TMP_InputField partyCodeInput;
        public TMP_Text partyCodeText;
        public PopupController popup;

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

            // --- CORREZIONE TEMPISMO ---
            // Diamo 1 secondo di tempo al sottoprocesso Unity sul server per fare il boot e aprire la porta KCP
            yield return new WaitForSeconds(1.0f);

            // Connetti il client Mirror al sottoprocesso della lobby generato
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

            // Per il Join non serve il ritardo di 1 secondo perché l'istanza è già attiva e aperta!
            ConnectToMirrorServer(res.ip, (ushort)res.port);
        }

        void ConnectToMirrorServer(string ip, ushort port)
        {
            NetworkManager.singleton.networkAddress = ip;
            
            // --- MODIFICATO PER KCP ---
            // Iniettiamo la porta dinamica ereditata in parallelo nel nuovo trasporto UDP KCP
            if (Transport.active is KcpTransport kcp)
            {
                kcp.Port = port;
                Debug.Log($"[CLIENT] Configurazione KCP completata su {ip}:{port}");
            }
            else
            {
                Debug.LogError("[CLIENT] Errore: KcpTransport non impostato come trasporto attivo in Mirror!");
            }
            
            // Avvia la connessione socket verso il sottoprocesso dedicato
            NetworkManager.singleton.StartClient();
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