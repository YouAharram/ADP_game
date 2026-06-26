using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;

    public Transform listParent;
    public GameObject entryPrefab;
    public Button startButton;

    private List<PlayerLobby> players = new List<PlayerLobby>();

    void Awake()
    {
        Instance = this;
    }

    public void Register(PlayerLobby player)
    {
        if (!players.Contains(player))
            players.Add(player);

        Refresh();
    }

    public void Unregister(PlayerLobby player)
    {
        players.Remove(player);
        Refresh();
    }

    public void Refresh()
    {
        foreach (Transform child in listParent)
            Destroy(child.gameObject);

        bool allReady = players.Count > 0;

        foreach (PlayerLobby player in players)
        {
            GameObject entry = Instantiate(entryPrefab, listParent);

            TMP_Text txt = entry.GetComponentInChildren<TMP_Text>();
            txt.text = player.playerName + " - " +
                       (player.isReady ? "READY" : "NOT READY");

            if (!player.isReady)
                allReady = false;
        }

        if (startButton != null)
            startButton.interactable = allReady;
    }
}