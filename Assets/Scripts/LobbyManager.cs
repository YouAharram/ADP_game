    using UnityEngine;
    using Unity.Netcode;
    using Unity.Netcode.Transports.UTP;
    using Unity.Networking.Transport.Relay;
    using Unity.Services.Core;
    using Unity.Services.Authentication;
    using Unity.Services.Lobbies;
    using Unity.Services.Lobbies.Models;
    using Unity.Services.Relay;
    using Unity.Services.Relay.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TMPro;

    public class LobbyManager : MonoBehaviour
    {
        private Lobby lobbyAttuale;

        [Header("Impostazioni Lobby")]
        private float timerHeartbeat = 15f;

        public TMP_InputField campoCodiceDaInserire;
        public TextMeshProUGUI testoCodice;

        async void Start() {
            // inizializza tutti i servizi cloud configurati nel progetto
            await UnityServices.InitializeAsync();

            // si accede ad un'istanza di AuthenticationService (Singleton) e si
            // accede all'evento
            // quando il login sarà completato, esegui il codice (il log)
            // SignedIn è un evento, e il += aggiunge una funzione (lambda) da 
            // eseguire quando l'evento si verifica
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Connesso al Cloud di Unity con ID: " + AuthenticationService.Instance.PlayerId);
            };

            // autentica questo utente senza chiedere credenziali
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        public async void CreaLobbyPrivata() {
            try {
                // un'allocazione è uno spazio su un server Relay di Unity riservato per host e client
                // prenota un server su unity e prepara uno slot per 3+1 giocatori e restituisce dati della connessione
                Allocation allocazione = await RelayService.Instance.CreateAllocationAsync(3);

                // genera il codice come stringa
                string codiceRelay = await RelayService.Instance.GetJoinCodeAsync(allocazione.AllocationId);

                // crea le opzioni della lobby (CORRETTO: IsPrivate true e visibilità Member)
                CreateLobbyOptions opzioni = new CreateLobbyOptions{
                    IsPrivate = true,
                    Data = new Dictionary<string, DataObject> {
                        { "CodiceRelay", new DataObject(DataObject.VisibilityOptions.Member, codiceRelay) }
                    }
                };

                // crea la lobby
                lobbyAttuale = await LobbyService.Instance.CreateLobbyAsync("La mia lobby", 4, opzioni);

                // codice lobby (corretto: fallback su Id se LobbyCode non disponibile)
                string codiceLobbyCreato = lobbyAttuale.LobbyCode ?? lobbyAttuale.Id;

                if (testoCodice != null) {
                    testoCodice.text = "CODICE: " + codiceLobbyCreato;
                }

                // per utilizzare il relay di unity invece di ip
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                    allocazione.RelayServer.IpV4,
                    (ushort)allocazione.RelayServer.Port,
                    allocazione.AllocationIdBytes,
                    allocazione.Key,
                    allocazione.ConnectionData
                );

                // starta l'host
                NetworkManager.Singleton.StartHost();
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Errore creazione lobby: " + e);
            }
        }

        public async void PartecipaLobbyPrivata()
        {
            try
            {
                string codiceDaUsare = campoCodiceDaInserire.text;

                // trova la lobby in base al codice da usare
                lobbyAttuale = await LobbyService.Instance.JoinLobbyByCodeAsync(codiceDaUsare);
                string codiceRelay = lobbyAttuale.Data["CodiceRelay"].Value;

                // unity prende il codice relay, risale all'allocation creata dall'host
                // ti viene data una connessione verso quel server e restituisce i dati della rete
                JoinAllocation allocazioneJoin = await RelayService.Instance.JoinAllocationAsync(codiceRelay);

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                    allocazioneJoin.RelayServer.IpV4,
                    (ushort)allocazioneJoin.RelayServer.Port,
                    allocazioneJoin.AllocationIdBytes,
                    allocazioneJoin.Key,
                    allocazioneJoin.ConnectionData,
                    allocazioneJoin.HostConnectionData
                );

                // entra nella partita
                NetworkManager.Singleton.StartClient();
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Codice errato o lobby inesistente: " + e);
            }
        }

        private void Update() {
            GestisciHeartbeatLobby();
        }

        // per non far eliminare la lobby dopo un tot di tempo
        private async void GestisciHeartbeatLobby() {
            if (lobbyAttuale != null && NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer) {
                timerHeartbeat -= Time.deltaTime;

                if (timerHeartbeat < 0f) {
                    timerHeartbeat = 15f;

                    try {
                        // invio ping al server
                        await LobbyService.Instance.SendHeartbeatPingAsync(lobbyAttuale.Id);
                        Debug.Log("Ping Heartbeat inviato.");
                    }
                    catch (LobbyServiceException e) {
                        Debug.LogWarning("Errore invio Heartbeat: " + e);
                    }
                }
            }
        }


        public void AvviaGioco() {
            if(NetworkManager.Singleton.IsServer) {
                NetworkManager.Singleton.SceneManager.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
            } else {
                Debug.Log("Solo l'Host può avviare la partita!");
            }
        }
    }