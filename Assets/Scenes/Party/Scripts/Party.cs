using Mirror;

public class Party
{
    public string code;
    public NetworkConnectionToClient leader;

    public NetworkConnectionToClient[] slots;

    public Party(int maxPlayers)
    {
        slots = new NetworkConnectionToClient[maxPlayers];
    }

    public int GetFreeSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                return i;
        }
        return -1;
    }

    public int AddPlayer(NetworkConnectionToClient conn)
    {
        int slot = GetFreeSlot();

        if (slot == -1)
            return -1;

        slots[slot] = conn;
        return slot;
    }

    public void RemovePlayer(NetworkConnectionToClient conn)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == conn)
            {
                slots[i] = null;
                break;
            }
        }
    }
}