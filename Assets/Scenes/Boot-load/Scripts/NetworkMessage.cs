using Mirror;

public struct StartGameMessage : NetworkMessage
{
    public string playerName;
}

public struct StartPartyMessage : NetworkMessage
{
    public string playerName;
}