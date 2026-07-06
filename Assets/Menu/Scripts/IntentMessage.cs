using Mirror;

public struct IntentMessage : NetworkMessage
{
    public string playerName;

    public IntentType intent;
    public string partyCode;

    public enum IntentType
    {
        QuickMatch,
        OpenLobby,
        CreateParty,
        JoinParty
    }
}