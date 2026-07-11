using System;
using System.Collections.Generic;

public class Party
{
    public string code;
    public string leaderName;

    public List<string> players = new();
    public int maxPlayers;

    public Party(int maxPlayers)
    {
        this.maxPlayers = maxPlayers;
    }

    public int GetFreeSlots()
    {
        return maxPlayers - players.Count;
    }

    public bool AddPlayer(string playerName)
    {
        if (players.Count >= maxPlayers)
            return false;

        players.Add(playerName);
        return true;
    }

    public void RemovePlayer(string playerName)
    {
        players.Remove(playerName);
    }
}