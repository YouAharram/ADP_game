using Mirror;

public abstract class CharacterDetector : NetworkBehaviour
{
    public abstract CharacterStats CharacterInRange(double chaseRange);
}
